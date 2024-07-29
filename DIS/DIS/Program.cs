using DIS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Milvus.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var milvusHost = builder.Configuration["Milvus:Host"];
var milvusPort = int.Parse(builder.Configuration["Milvus:Port"]);
builder.Services.AddSingleton(new MilvusClient(milvusHost, milvusPort));
builder.Services.AddScoped<DocumentService>();

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
