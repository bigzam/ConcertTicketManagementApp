
using System.Text;
using ConcertTicketManagement.Api.Auth;
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Application.Payment;
using ConcertTicketManagement.Application.Tickets.Services;
using ConcertTicketManagement.Repositories.Events;
using ConcertTicketManagement.Repositories.Tickets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("Key:TBD")),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = "Issuer:TBD",
        ValidAudience = "Audience:TBD",
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy(AuthConstants.EventsAdminUserPolicyName,
        p => p.RequireClaim(AuthConstants.EventsAdminUserPolicyName, "true"));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddSingleton<IEventRepository, InMemoryEventRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
