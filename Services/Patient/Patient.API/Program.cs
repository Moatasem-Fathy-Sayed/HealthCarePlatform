using Microsoft.OpenApi;
using Patient.API.Middleware;
using Patient.Application.Common.Interfaces;
using Patient.Infrastructure;
using Patient.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Patient.API",
        Version = "v1"
    });

    //options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    //{
    //    Name = "Authorization",
    //    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
    //    Scheme = "Bearer",
    //    BearerFormat = "JWT",
    //    In = Microsoft.OpenApi.ParameterLocation.Header,
    //    Description = "Enter 'Bearer' [space] and then your valid token."
    //});
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "YourSuperSecretKeyHere_EnsureItIsLongEnough"))
    };
});

//builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Patient.Application.Patients.Commands.CreatePatient.CreatePatientCommand).Assembly));

builder.Services.AddPatientInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
//app.UseAuthorization();

app.MapControllers();

app.Run();