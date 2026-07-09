using Microsoft.EntityFrameworkCore;
using ControleGastos.Api.Data;
using ControleGastos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Injeção de dependência (DI) ---

// Configura o EF Core para usar SQLite, salvando o banco em um arquivo
// físico "gastos.db" na raiz do projeto. Isso é o que garante a
// persistência dos dados mesmo depois de fechar a aplicação.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=gastos.db"));

// Registra os services (lógica de negócio) para serem injetados nos controllers.
builder.Services.AddScoped<PessoaService>();
builder.Services.AddScoped<TransacaoService>();
builder.Services.AddScoped<TotaisService>();

// Controllers (endpoints da API) + serialização de enums como texto
// (ex.: "Despesa"/"Receita" em vez de 0/1), o que deixa o JSON mais legível.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Swagger: gera uma documentação/interface interativa da API em /swagger,
// útil para testar os endpoints manualmente durante o desenvolvimento.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: libera o front-end React (rodando em outra porta, ex. 5173)
// para conseguir chamar essa API.
const string CorsPolicy = "FrontEndPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Garante que o banco e as tabelas existam ao iniciar a aplicação ---
// EnsureCreated() cria o arquivo .db e as tabelas na primeira execução,
// caso ainda não existam (suficiente para o escopo deste desafio).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
