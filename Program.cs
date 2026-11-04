using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Services;
using PawCareApi.Diagnostics;
using PawCareApi.Domain.Interfaces;
using PawCareApi.HealthChecks;
using PawCareApi.Infrastructure.Data;
using PawCareApi.Infrastructure.NoSql;
using PawCareApi.Infrastructure.Repositories;
using PawCareApi.Infrastructure.Security;
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
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ==========================================
// 3. Banco de Dados Relacional (EF Core / Oracle / In-Memory)
// ==========================================
builder.Services.AddDbContext<PawCareContext>(options =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

    if (useInMemory || string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("SECRETO"))
    {
        options.UseInMemoryDatabase("PawCareIntegrationDb");
    }
    else
    {
        options.UseOracle(connectionString);
    }
});

// ==========================================
// 4. Banco de Dados NoSQL (MongoDB)
// ==========================================
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// ==========================================
// 5. Injeção de Dependência (Clean Architecture)
// ==========================================
// Repositórios Concretos (Infrastructure)
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<IClinicaRepository, ClinicaRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProntuarioRepository, ProntuarioRepository>();

// Serviços de Aplicação (Application)
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<ITutorService, TutorService>();
builder.Services.AddScoped<IClinicaService, ClinicaService>();
builder.Services.AddScoped<IProntuarioService, ProntuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHateoasService, HateoasService>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// ==========================================
// 6. Autenticação e Autorização JWT Bearer
// ==========================================
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "MinhaChaveSuperSecretaParaPawCareSprint4Fiap2026_AssinaturaJWT_123456789";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PawCareApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PawCareApi_Clients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            Log.Information("[JWT] Header recebido: '{AuthHeader}'", authHeader ?? "(null)");
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                var cleanToken = authHeader.Trim();
                while (cleanToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    cleanToken = cleanToken.Substring(7).Trim();
                }
                context.Token = cleanToken;
                Log.Information("[JWT] Token extraído: '{Token}'", cleanToken.Length > 20 ? cleanToken.Substring(0, 20) + "..." : cleanToken);
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Log.Warning(context.Exception, "[JWT] Falha de autenticação: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claims = string.Join(", ", context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>());
            Log.Information("[JWT] Token validado com sucesso! Claims: {Claims}", claims);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Log.Warning("[JWT] Challenge acionado. Error: '{Error}', Description: '{Description}'", context.Error, context.ErrorDescription);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("VetOrAdmin", policy => policy.RequireRole("Admin", "Veterinario"));
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

// ==========================================
// 7. Health Checks (Correção do Feedback Sprint 3: Tag Live Registrada)
// ==========================================
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks()
    // Self check registrado com a tag 'live' (Liveness Probe para Kubernetes/Docker)
    .AddCheck("self", () => HealthCheckResult.Healthy("API operacional e responsiva"), tags: new[] { "live" })
    // Database check registrado com a tag 'ready' (Readiness Probe)
    .AddDbContextCheck<PawCareContext>("oracle-database", tags: new[] { "ready", "db" })
    // MongoDB NoSQL check registrado com a tag 'ready'
    .AddCheck<MongoHealthCheck>("mongodb", tags: new[] { "ready", "nosql" })
    // Verificação de conectividade com serviços externos registrado com a tag 'ready'
    .AddCheck<ExternalServicesHealthCheck>("external-services", tags: new[] { "ready", "external" });

// ==========================================
// 8. Tracing & Métricas Distribuídas (OpenTelemetry)
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
// 9. Documentação Detalhada com Swagger / OpenAPI & JWT
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PawCare API - Gestão Veterinária & Cuidados Pet",
        Version = "v1",
        Description = "API RESTful Corporativa para gestão abrangente de cuidados veterinários, pets, tutores, clínicas, históricos clínicos e prontuários NoSQL (MongoDB). Desenvolvida para a disciplina Advanced Business Development with .NET na FIAP.",
        Contact = new OpenApiContact
        {
            Name = "Equipe PawCare - FIAP 2026",
            Email = "contato@pawcare.fiap.com.br"
        }
    });

    options.EnableAnnotations();

    // Configuração do Bearer Token no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT retornado no login (ex: Bearer {token})",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });

    options.OperationFilter<AllowAnonymousOperationFilter>();

    // Habilita inclusão dos comentários XML gerados no build
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Inicialização automática e seed de dados iniciais
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PawCareContext>();
    await context.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(context);
}

// ==========================================
// 10. Pipeline de Middlewares
// ==========================================
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PawCare API v1");
    c.DocumentTitle = "PawCare API - Documentação Interativa";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// 11. Mapeamento de Endpoints de Health Check
// ==========================================
// Endpoint principal com diagnóstico completo em JSON
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

// Endpoint de Liveness (sonda rápida: verifica se a aplicação está viva vinculada à tag 'live')
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

// Endpoint de Readiness (verifica dependências como DB relacional, MongoDB e serviços externos)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapControllers();

app.Run();

// Necessário para testes de integração com WebApplicationFactory<Program>
public partial class Program { }

public class AllowAnonymousOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowAnonymous = (context.MethodInfo.DeclaringType != null &&
            context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
            || context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (allowAnonymous)
        {
            operation.Security = new List<OpenApiSecurityRequirement>();
        }
    }
}


