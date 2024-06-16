using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Explicit Kestrel configuration
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5000); // Listen for HTTP
    serverOptions.Listen(IPAddress.Any, 2083, listenOptions => // Listen for HTTPS
    {
        var certificate = LoadCertificate("/etc/ssl/certs/boxpvp.top.pem", "/etc/ssl/private/boxpvp.top.key");
        listenOptions.UseHttps(new HttpsConnectionAdapterOptions
        {
            ServerCertificate = certificate
        });
    });
});

// Method to load the certificate and private key
X509Certificate2 LoadCertificate(string certPath, string keyPath)
{
    string certificateText = File.ReadAllText(certPath);
    string privateKeyText = File.ReadAllText(keyPath);
    using var publicKey = X509Certificate2.CreateFromPem(certificateText);
    using var rsaPrivateKey = RSA.Create();
    rsaPrivateKey.ImportFromPem(privateKeyText.ToCharArray());
    return publicKey.CopyWithPrivateKey(rsaPrivateKey);
}

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://www.boxpvp.top", "https://boxpvp.top", "https://boxpvp.top/*")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
