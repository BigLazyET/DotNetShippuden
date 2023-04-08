var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/", () => Console.WriteLine("Hello"));

app.MapGet("/foo", () =>
{
    Console.WriteLine("Hello, i am foo");
    return $"\"Hello, i am foo\"";
});

app.MapPost("/bar", () => Console.WriteLine("Hello, i am bar"));

app.Map("/all", () => Console.WriteLine("Hello, i am all"));

app.Run();
