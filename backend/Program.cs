var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// itemsディレクトリ内のすべてのjsonファイルを読み込んで一覧APIとして返す
app.MapGet("/api/items", () =>
{
    var dirPath = Path.Combine(AppContext.BaseDirectory, "items");
    var files = Directory.GetFiles(dirPath, "*.json");
    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var items = files.Select(file =>
    {
        var json = System.IO.File.ReadAllText(file);
        return System.Text.Json.JsonSerializer.Deserialize<Item>(json, options);
    }).Where(x => x != null).ToList();
    return items;
}).WithName("GetItems");

// サンプル（テンプレートのまま）
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record Item(int Id, string Name, int Price);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
