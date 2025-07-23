
using ConcertTicketManagement.Application.Events.Services;
using ConcertTicketManagement.Application.Payment;
using ConcertTicketManagement.Application.Tickets.Services;
using ConcertTicketManagement.Repositories.Events;
using ConcertTicketManagement.Repositories.Tickets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddSingleton<IEventRepository, InMemoryEventRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

// TODO: need to be implemented
// app.UseMiddleware<ValidationMappingMiddleware>();

app.MapControllers();

app.Run();
