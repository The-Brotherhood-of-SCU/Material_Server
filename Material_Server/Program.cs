using Material_Server;
using Microsoft.AspNetCore.HttpLogging;

Assets.Init();

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseUrls("http://0.0.0.0:5000");
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allow all",
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader();
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(o => {
    o.LoggingFields = HttpLoggingFields.All;
});
var app = builder.Build();
app.UseCors("allow all");
app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
