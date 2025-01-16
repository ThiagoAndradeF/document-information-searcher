using DIS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenAI;
using OpenAI.Managers;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);
var openaiKey = builder.Configuration["OpenAiKey"];
var qdrantKey = builder.Configuration["QdrantKey"]; 
var qdrantHost = builder.Configuration["QdrantHost"];
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
