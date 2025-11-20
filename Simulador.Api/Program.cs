using Microsoft.EntityFrameworkCore;
using Simulador.Api.Data;
using Simulador.Api.Logic.Repositories;
using Simulador.Api.Logic.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Services
builder.Services.AddScoped<IPerfilRiscoService, PerfilRiscoService>();
builder.Services.AddScoped<ISimuladorService, SimuladorService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
builder.Services.AddScoped<IInvestimentoService, InvestimentoService>();

builder.Services.AddScoped<ISimuladorRepository, SimuladorRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IInvestimentoRepository, InvestimentoRepository>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

//garante que o banco vai ser criado
var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
dbContext.Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
