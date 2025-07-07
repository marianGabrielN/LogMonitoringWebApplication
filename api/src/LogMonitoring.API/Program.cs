using LogMonitoring.Application.Interfaces;
using LogMonitoring.Application.Services;
using LogMonitoring.Infrastructure;

var localClientPolicyName = "AllowLocaLAngularClient";

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS services and define the policy.
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: localClientPolicyName,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application and infrastructure services
builder.Services.AddScoped<IJobProcessor, JobProcessor>();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(localClientPolicyName);
app.UseAuthorization();
app.MapControllers();

app.Run();
