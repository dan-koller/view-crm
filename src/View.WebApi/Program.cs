using Microsoft.AspNetCore.Mvc.Formatters; // IOutputFormatter, OutputFormatter
using View.Shared; // Ticket, ApplicationUser
using View.WebApi.Repositories; // ITicketRepository, TicketRepository
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using View.WebApi.Data;

var MyAllowSpecificOrigins = "_allowReactApp"; // CORS

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddViewContext(); // Connect to SqlServer Ticket database

builder.Services.AddControllers(options =>
{
    WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter is null)
        {
            WriteLine($"  {formatter.GetType().Name}");
        }
        else // OutputFormatter class has SupportedMediaTypes
        {
            WriteLine("  {0}, Media types: {1}",
              arg0: mediaFormatter.GetType().Name,
              arg1: string.Join(", ",
                mediaFormatter.SupportedMediaTypes));
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();

// Add ASP.NET Core Identity support
builder.Services.AddUserContext(); // Connect to SqlServer User database

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<UserContext>();

// Add Authentication services & JWT middlewares
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["JwtSettings:SecurityKey"]))
    };
});

builder.Services.AddScoped<JwtHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          //policy.WithOrigins("http://localhost:3000",
                          //                   "https://localhost:5001",
                          //                   "https://localhost:44440")
                          policy.AllowAnyOrigin() // Just for development
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Add HTTP request logging
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // default 32kb
    options.ResponseBodyLogLimit = 4096; // default 32kb
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeSharing Service API v1");
        c.SupportedSubmitMethods(new[] {
            SubmitMethod.Get, SubmitMethod.Post,
            SubmitMethod.Put, SubmitMethod.Delete });
    });
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

// app.UseHttpLogging(); // only for development

app.MapControllers();

app.Run();
