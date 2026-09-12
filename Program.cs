using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using PawCareApi.Data;
using PawCareApi.Diagnostics;
using PawCareApi.HealthChecks;
using PawCareApi.Middlewares;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Configuração de Logging Estruturado (Serilog)
// ==========================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "PawCareApi")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/pawcare-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ==========================================
// 2. Controllers & JSON Options
// ==========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ==========================================
// 3. Banco de Dados (Oracle / DbContext)
// ==========================================
builder.Services.AddDbContext<PawCareContext>(options =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemory)
    {
        options.UseInMemoryDatabase("PawCareIntegrationDb");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("OracleConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            options.UseOracle(connectionString);
        }
    }
});

// ==========================================
// 4. Health Checks (API, Banco de Dados, Serviços Externos)
// ==========================================
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PawCareContext>(
        name: "oracle-database",
        tags: new[] { "db", "ready" })
    .AddCheck<ExternalServicesHealthCheck>(
        name: "external-services",
        tags: new[] { "external", "ready" });

// ==========================================
// 5. Tracing & Métricas Distribuídas (OpenTelemetry)
// ==========================================
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddSource("PawCareApi")
            .AddAspNetCoreInstrumentation(opts =>
            {
                opts.RecordException = true;
            })
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddMeter(PawCareMetrics.MeterName)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

// ==========================================
// 6. Documentação Detalhada com Swagger / OpenAPI
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PawCare API - Gestão Veterinária & Cuidados Pet",
        Version = "v1",
        Description = "API RESTful para gestão abrangente de cuidados veterinários, pets, tutores, clínicas e históricos clínicos. Desenvolvida para a disciplina Advanced Business Development with .NET na FIAP.",
        Contact = new OpenApiContact
        {
            Name = "Equipe PawCare",
            Email = "contato@pawcare.fiap.com.br"
        }
    });

    // Habilita inclusão dos comentários XML gerados no build
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// ==========================================
// Pipeline de Middlewares
// ==========================================

// Correlation ID para rastreamento de requisições
app.UseMiddleware<CorrelationIdMiddleware>();

// Logging de requisições HTTP via Serilog
app.UseSerilogRequestLogging();

// Swagger disponível para testes e documentação
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PawCare API v1");
    c.DocumentTitle = "PawCare API - Documentação Interativa";
});

app.UseHttpsRedirection();
app.UseAuthorization();

// ==========================================
// Mapeamento de Endpoints de Health Check
// ==========================================
// Endpoint principal com diagnóstico completo em JSON
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

// Endpoint de Liveness (sonda rápida se a aplicação está viva)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

// Endpoint de Readiness (verifica dependências como DB e serviços externos)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapControllers();

app.Run();

// Necessário para testes de integração com WebApplicationFactory<Program>
public partial class Program { }