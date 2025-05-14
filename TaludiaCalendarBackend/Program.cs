using TaludiaCalendar.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyectamos nuestro servicio
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint API separado en otro archivo
app.MapCalendarEndpoints();

app.Run();
