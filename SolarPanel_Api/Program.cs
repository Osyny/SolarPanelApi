using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Azure;
using SolarPanel_Api.Services.AzureTable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsOrigins = builder.Configuration["App:CorsOrigins"];
builder.Services.AddCors(
     options => options.AddPolicy(
         "Default Policy",
         builder => builder
         .WithOrigins(
                 // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                 corsOrigins?
                     .Split(",", StringSplitOptions.RemoveEmptyEntries)
                     .ToArray()
             )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()


     ));

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddTableServiceClient(builder.Configuration["ConnectionStrings:table"]);
});
builder.Services.AddScoped<ISolarPanelTableService, SolarPanelTableService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Default Policy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
