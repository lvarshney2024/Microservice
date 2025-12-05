using InfluxDB.Client;
using UserService.Repositories;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// InfluxDB settings
var influxUrl = builder.Configuration["Influx:Url"] ?? "http://localhost:8086";
var influxToken = builder.Configuration["Influx:Token"] ?? "your-influxdb-token";
var influxOrg = builder.Configuration["Influx:Org"] ?? "your-org";
var influxBucket = builder.Configuration["Influx:Bucket"] ?? "UserServiceBucket";

// Register InfluxDB client
builder.Services.AddSingleton(new InfluxDBClient(influxUrl, influxToken));

// Register repository and service
builder.Services.AddScoped<IUserRepository>(sp =>
    new InfluxUserRepository(
        sp.GetRequiredService<InfluxDBClient>(),
        influxBucket,
        influxOrg));
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
