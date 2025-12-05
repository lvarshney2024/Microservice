using MicroService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind Mongo settings from configuration or use defaults
var mongoConnection = builder.Configuration["Mongo:ConnectionString"];
var mongoDatabase = builder.Configuration["Mongo:Database"];

// Register MongoDbService as singleton
builder.Services.AddSingleton(new MongoDbService(mongoConnection, mongoDatabase));

// Register repositories and services
builder.Services.AddScoped<MicroService.Repositories.IOrderRepository, MicroService.Repositories.OrderRepository>();
builder.Services.AddScoped<MicroService.Services.IOrderService, MicroService.Services.OrderService>();

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
