using UserService.Repositories;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// InfluxDB settings (3.x)
var influxUrl = builder.Configuration["Influx:Url"] ?? "http://localhost:8086";
var influxToken = builder.Configuration["Influx:Token"] ?? string.Empty;
var influxDatabase = builder.Configuration["Influx:Database"] ?? "Student";

// Register a named HttpClient for talking to InfluxDB REST API
builder.Services.AddHttpClient("Influx", client =>
{
    client.BaseAddress = new Uri(influxUrl);
    if (!string.IsNullOrWhiteSpace(influxToken))
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", influxToken);
    }
});

// Register repository and service
builder.Services.AddScoped<IUserRepository>(sp =>
    new InfluxUserRepository(
        sp.GetRequiredService<System.Net.Http.IHttpClientFactory>().CreateClient("Influx"),
        influxUrl,
        influxToken,
        influxDatabase));
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
