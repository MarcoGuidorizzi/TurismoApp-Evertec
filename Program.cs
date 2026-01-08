using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TurismoApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("ibge", c =>
{
    c.BaseAddress = new Uri("https://servicodados.ibge.gov.br");
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/PontosTuristicos/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PontosTuristicos}/{action=Index}/{id?}");

app.MapGet("/api/municipios/{uf}", async (string uf, IHttpClientFactory httpClientFactory) =>
{
    if (string.IsNullOrWhiteSpace(uf))
        return Results.BadRequest(new { error = "UF � obrigat�ria" });

    var client = httpClientFactory.CreateClient("ibge");
    var response = await client.GetAsync($"/api/v1/localidades/estados/{uf}/municipios");

    if (!response.IsSuccessStatusCode)
        return Results.StatusCode((int)response.StatusCode);

    using var stream = await response.Content.ReadAsStreamAsync();
    using var doc = await JsonDocument.ParseAsync(stream);
    var nomes = doc.RootElement.EnumerateArray()
        .Select(e => e.GetProperty("nome").GetString() ?? string.Empty)
        .OrderBy(n => n)
        .ToArray();

    return Results.Json(nomes);
});

app.MapGet("/api/cep/{cep}", async (string cep, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var onlyDigits = new string(cep.Where(char.IsDigit).ToArray());
    if (onlyDigits.Length != 8) return Results.BadRequest(new { error = "CEP inv�lido" });

    var resp = await client.GetAsync($"https://viacep.com.br/ws/{onlyDigits}/json/");
    if (!resp.IsSuccessStatusCode) return Results.StatusCode((int)resp.StatusCode);
    var json = await resp.Content.ReadAsStringAsync();
    return Results.Content(json, "application/json");
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var conn = db.Database.GetDbConnection();

        Console.WriteLine("Provider: " + db.Database.ProviderName);
        Console.WriteLine("DataSource: " + conn.DataSource);
        Console.WriteLine("Database: " + conn.Database);
        Console.WriteLine("ConnectionString usada na inicializa��o: " + conn.ConnectionString);
        logger.LogInformation("Tentando conectar ao banco de dados...");

        bool canConnect = false;
        try
        {
            canConnect = db.Database.CanConnect();
        }
        catch (Exception exCan)
        {
            Console.Error.WriteLine("Exce��o ao verificar CanConnect():");
            Console.Error.WriteLine(exCan.ToString());
            logger.LogError(exCan, "Erro em CanConnect()");
        }

        Console.WriteLine("CanConnect() = " + canConnect);

        logger.LogInformation("Tentando EnsureCreated()...");
        bool ensured = false;
        try
        {
            ensured = db.Database.EnsureCreated();
            Console.WriteLine("EnsureCreated() retornou: " + ensured);
            logger.LogInformation("EnsureCreated() retornou: {Ensured}", ensured);
        }
        catch (Exception exEnsure)
        {
            Console.Error.WriteLine("Exce��o ao executar EnsureCreated():");
            Console.Error.WriteLine(exEnsure.ToString());
            logger.LogError(exEnsure, "Erro em EnsureCreated()");
            throw;
        }

        if (canConnect)
        {
            try
            {
                var count = db.PontosTuristicos.Count();
                Console.WriteLine($"PontosTuristicos existentes no banco: {count}");
            }
            catch (Exception exCount)
            {
                Console.Error.WriteLine("Falha ao contar registros de PontosTuristicos:");
                Console.Error.WriteLine(exCount.ToString());
            }
        }

        logger.LogInformation("Inicializa��o do banco conclu�da com sucesso.");
        Console.WriteLine("Inicializa��o do banco conclu�da com sucesso.");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Erro durante inicializa��o do banco de dados:");
        Console.Error.WriteLine(ex.ToString());
        var inner = ex.InnerException;
        while (inner != null)
        {
            Console.Error.WriteLine("Inner exception:");
            Console.Error.WriteLine(inner.ToString());
            inner = inner.InnerException;
        }

        try
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Erro ao inicializar o banco de dados");
        }
        catch { /* ignore logger errors */ }

        throw;
    }
}

app.Run();

