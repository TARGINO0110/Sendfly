using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sendfly.Models.Auth;
using Sendfly.Models.Mail;
using Sendfly.Services.MailServices;
using Sendfly.Services.MailServices.Interfaces;
using Sendfly.Services.ServiceAuth;
using Sendfly.Services.ServiceAuth.Interfaces;
using Sendfly.Services.Trataments;
using Sendfly.Services.Trataments.Interfaces;
using Sendfly.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SENDFLY",
        Version = "v1",
        Description = "Microservice send E-mail",
    });
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                    }
        });
});
builder.Services.AddCors();

// *************** SERVICES DI - DEPENDENCES INJECTION ***************
builder.Services.AddTransient<ICredentialRepository, CredentialRepository>();
builder.Services.AddTransient<IJWTService, JWTService>();
builder.Services.AddTransient<IMailSenderService, MailSenderService>();
builder.Services.AddTransient<ITratamentMail, TratamentMail>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//********************* SETTINGS JWT  **********************
//Add Secret Key in SettingsJWT.cs file in Utils folder
var key = Encoding.ASCII.GetBytes(SettingsJWT.SecretKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        RequireExpirationTime = true,
    };
});

//********************* SETTINGS SEND GRID  **********************
builder.Services.Configure<SendMail>(opt =>
{
    opt.ApiKey = builder.Configuration.GetSection("SendGrid:Key").Value;
    opt.SenderMail = builder.Configuration.GetSection("SendGrid:Mail").Value;
    opt.SenderName = builder.Configuration.GetSection("SendGrid:NameContact").Value;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt => opt
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseRouting();

var authEndpoint = app.MapGroup("/api/v1/auth/");

var sendMail = app.MapGroup("/api/v1/sendMail/");

authEndpoint.MapPost("login", [AllowAnonymous] ([FromBody] User user, ICredentialRepository credentialRepository) =>
{
    try
    {
        var result = credentialRepository.PrepareCredentials(user);
        if (result is null)
            return Results.Unauthorized();

        return Results.Ok(result);
    }
    catch (Exception) { throw; }
})
.WithName("Auth")
.WithOpenApi();

sendMail.MapPost("PostSendMail", [Authorize] async ([FromBody] PostMail postMail, IMailSenderService mailSenderService, ITratamentMail tratamentMail) =>
{
    try
    {
        return Results.Ok(await mailSenderService.SendMailAsync(tratamentMail.ConfigDataMail(postMail)));
    }
    catch (Exception) { throw; }
})
.WithName("SendMail")
.WithOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
