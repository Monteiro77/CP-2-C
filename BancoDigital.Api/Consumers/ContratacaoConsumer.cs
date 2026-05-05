using BancoDigital.Api.Data;
using BancoDigital.Api.Domain.Entities;
using BancoDigital.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BancoDigital.Api.Consumers;

public class ContratacaoConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContratacaoConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private const string QueueName = "contratacoes";

    public ContratacaoConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ContratacaoConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            _logger.LogInformation("ContratacaoConsumer conectado ao RabbitMQ.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("RabbitMQ indisponível, consumer não iniciado: {Message}", ex.Message);
        }
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null) return Task.CompletedTask;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            if (long.TryParse(message, out var contratacaoId))
            {
                await ProcessarContratacaoAsync(contratacaoId);
            }

            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessarContratacaoAsync(long contratacaoId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BancoContext>();

        var contratacao = await db.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == contratacaoId);

        if (contratacao is null) return;

        contratacao.Status = StatusContratacao.EmAnalise;
        await db.SaveChangesAsync();

        await Task.Delay(TimeSpan.FromSeconds(2));

        var score = CalcularScore(contratacao.Cliente, contratacao.ValorSolicitado);
        contratacao.ScoreCredito = score;
        contratacao.DataProcessamento = DateTime.UtcNow;

        if (contratacao.Produto is Emprestimo emprestimo)
        {
            if (contratacao.ValorSolicitado < emprestimo.ValorMinimo || contratacao.ValorSolicitado > emprestimo.ValorMaximo)
            {
                contratacao.Status = StatusContratacao.Reprovada;
                contratacao.ObservacaoProcessamento = $"Valor fora do intervalo permitido ({emprestimo.ValorMinimo:C} – {emprestimo.ValorMaximo:C}).";
            }
            else if (score >= 600)
            {
                contratacao.Status = StatusContratacao.Aprovada;
                var taxa = score switch
                {
                    >= 800 => 0.0199m,
                    >= 700 => 0.0249m,
                    _ => 0.0299m
                };
                contratacao.ObservacaoProcessamento = $"Aprovado. Score: {score}. Taxa mensal aplicada: {taxa:P2}.";
            }
            else
            {
                contratacao.Status = StatusContratacao.Reprovada;
                contratacao.ObservacaoProcessamento = $"Score insuficiente ({score}). Mínimo exigido: 600.";
            }
        }
        else
        {
            contratacao.Status = score >= 500 ? StatusContratacao.Aprovada : StatusContratacao.Reprovada;
            contratacao.ObservacaoProcessamento = $"Score: {score}.";
        }

        await db.SaveChangesAsync();
        _logger.LogInformation("Contratação {Id} processada: {Status}", contratacaoId, contratacao.Status);
    }

    private static int CalcularScore(Cliente cliente, decimal valorSolicitado)
    {
        var baseScore = Math.Abs(cliente.Id.GetHashCode() % 401) + 400; // 400–800

        if (cliente is PessoaFisica pf)
        {
            var idade = DateTime.Today.Year - pf.DataNascimento.Year;
            if (idade is >= 25 and <= 65) baseScore += 100;
            if (valorSolicitado > 30000m) baseScore -= 50;
        }
        else
        {
            baseScore += 80;
            if (valorSolicitado > 40000m) baseScore -= 40;
        }

        return Math.Clamp(baseScore, 300, 900);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
