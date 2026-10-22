var builder = WebApplication.CreateBuilder(args);

// Controllers verwerken straks de HTTP-verzoeken voor projecten en blogposts.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger is alleen beschikbaar tijdens het lokaal ontwikkelen.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
