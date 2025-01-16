using DIS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenAI;
using OpenAI.Managers;
using Qdrant.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var openaiKey = builder.Configuration["OpenAiKey"];
var qdrantKey = builder.Configuration["QdrantKey"]; 
var qdrantHost = builder.Configuration["QdrantHost"];
var redisConnectionString = builder.Configuration["RedisConnectionString"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
});
builder.Services.AddSingleton(new OpenAIService(new OpenAiOptions()
{
	ApiKey = openaiKey
}));

builder.Services.AddSingleton(new QdrantClient(
  host: qdrantHost,
  https: true,
  apiKey: qdrantKey
));
builder.Services.AddScoped<ITextAnalysisService, TextAnalysisService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<VetorialDataBaseService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login"; 
                    options.AccessDeniedPath = "/access-denied"; 
                });

var app = builder.Build();

app.UseCors(policyBuilder =>
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
