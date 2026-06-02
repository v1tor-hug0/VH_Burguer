using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VHBurguer.Applications.Autenticacao;
using VHBurguer.Applications.ContentSafety;
using VHBurguer.Applications.Services;
using VHBurguer.Contexts;
using VHBurguer.Interfaces;
using VHBurguer.Repositories;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Value: Bearer TokenJWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// chamar nossa conexao com o banco aqui na program
builder.Services.AddDbContext<VH_BurguerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Usuario
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// Produto
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

// Categoria
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<CategoriaService>();

// Promocao
builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<PromocaoService>();

// Log de Alteracao
builder.Services.AddScoped<ILogAlteracaoProdutoRepository, LogAlteracaoProdutoRepository>();
builder.Services.AddScoped<LogAlteracaoProdutoService>();

// JWT
builder.Services.AddScoped<GeradorTokenJwt>();
builder.Services.AddScoped<AutenticacaoService>();

builder.Services.AddScoped<IContentSafetyRepository, ContentSafetyService>();

// Configura o sistema de autenticacao da aplicacao.
// Aqui estamos dizendo que o tipo de autenticacao padrao sera JWT Bearer.
// Ou seja: a API vai esperar receber um Token JWT nas requisicoes.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    // Adiciona o suporte para autenticacao usando JWT.
    .AddJwtBearer(options =>
    {
        // Le a chave secreta definida no appsettings.json.
        // Essa chave e usada para ASSINAR o token quando ele e gerado
        // e tambem para VALIDAR se o token recebido e verdadeiro.
        var chave = builder.Configuration["Jwt:Key"]!;

        // Quem emitiu o token (ex: nome da sua aplicacao).
        // Serve para evitar aceitar tokens de outro sistema.
        var issuer = builder.Configuration["Jwt:Issuer"]!;

        // Para quem o token foi criado (normalmente o frontend ou a propria API).
        // Tambem ajuda a garantir que o token pertence ao seu sistema.
        var audience = builder.Configuration["Jwt:Audience"]!;

        // Define as regras que serao usadas para validar o token recebido.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Verifica se o emissor do token e valido
            // (se bate com o issuer configurado).
            ValidateIssuer = true,

            // Verifica se o destinatario do token e valido
            // (se bate com o audience configurado).
            ValidateAudience = true,

            // Verifica se o token ainda esta dentro do prazo de validade.
            // Se ja expirou, a requisiçao sera negada.
            ValidateLifetime = true,

            // Verifica se a assinatura do token e valida.
            // Isso garante que o token nao foi alterado.
            ValidateIssuerSigningKey = true,

            // Define qual emissor a considerado valido.
            ValidIssuer = issuer,

            // Define qual audience e considerado valido.
            ValidAudience = audience,

            // Define qual chave ser usada para validar a assinatura do token.
            // A mesma chave usada na geracaoo do JWT deve estar aqui.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(chave)
            )
        };
    });

// Adiciona CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
