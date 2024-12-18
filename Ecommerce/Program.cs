using Ecommerce.DatabaseFunctions;
using Ecommerce.Filters;
using Ecommerce.Repository;
using Ecommerce.Services;
using Ecommerce.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<DapperContext>();            //Registering DapperContext
//one instance of the service will be created and shared throughout the entire application lifetime
//regardless of the number of HTTP requests.
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();        // Registering Repository
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); // Add AuthenticationService
                                                                             // a new instance of the AuthenticationService is created per request           
                                                                             // You don't keep services alive unnecessarily, avoiding memory leaks.   


builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();   // For getting claims

// Add controllers
builder.Services.AddControllers();

//Adding Validator
builder.Services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<UserOrderValidator>());

//Adding filter:
builder.Services.AddScoped<OrderValidationFilter>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});



var app = builder.Build();

//These are the middlewares:

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Using custom middleware
app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors(opt =>
{
    opt.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin();
});


app.UseHttpsRedirection();  //This middleware enforces HTTPS by redirecting HTTP requests to HTTPS.

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
app.Run();