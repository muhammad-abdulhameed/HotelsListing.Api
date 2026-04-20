using HotelListing.Api.Constants;
using HotelsListing.Api.Contructs;
using HotelsListing.Api.Data;
using HotelsListing.Api.DomainObj;
using HotelsListing.Api.Handlers;
using HotelsListing.Api.MappingProfiles;
using HotelsListing.Api.Middleware;
using HotelsListing.Api.services;
using HotelsListing.common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;

// note we do that cause we can't use .net logger  here ILogger (we integrate serialog with i looger oin app)
// here init the logger before app run 
Log.Logger = new LoggerConfiguration()
    //here make the minamal of logs that we want show in log sink 
    .MinimumLevel.Information()
    // we can extend  the logging sec in settings.json but here we override it 
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // target log file 
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try // here we put it in try so we can trace any exption happen while excution
{
    Log.Information("app starts");
    var builder = WebApplication.CreateBuilder(args);


    // here instade of have tow log sys (serilog ilogger ) hre integrate them together 
    builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
);

    // here we get the connecttion strings that we make on configration json file 
    var connectionStrings = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
    /*
     * those to lins is to add db to app using ef

     */
    // that's ad obj of db context then pass option obj and till use sql server and pass connection strings(configration)
    builder.Services.AddDbContext<HotelsListingApiDataContext>(options => options.UseSqlServer(connectionStrings));
    // this custom class to add some property to than base class Identity 
    builder.Services.AddIdentityApiEndpoints<AppUser>(/*opt=>opt. here we can add more polices to complicate auth   */  )
        // here add role for Authraization
        .AddRoles<IdentityRole>()
        // here pass our ef class that's connected with database to make identity user  stored in 
        .AddEntityFrameworkStores<HotelsListingApiDataContext>();
    // this to make ioc inject HttpContext With data of current request
    builder.Services.AddHttpContextAccessor();

    // here impl option pattern by bind (JwtSettings) as config class 
    // Register JwtSettings with the Options Pattern and bind it to the "JwtSettings" section
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    // here we get the binded class to access it's props
    // Manually bind the "JwtSettings" section to a JwtSettings object
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
    if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    {
        Log.Fatal("JwtSettings: Key is not configured.");
        // we put a log to know  where  problem happen
        throw new InvalidOperationException("JwtSettings:Key is not configured.");
    }

    builder.Services.AddAuthentication(options =>
    {
        /* options.DefaultScheme = AuthenticationDefaults.BasicScheme;
         options.DefaultChallengeScheme = AuthenticationDefaults.BasicScheme;*/
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {/// hree give the prammeter the token must validate 
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            // here get values for validator parmeter 
            ValidIssuer = jwtSettings.Issuer, // builder.Configuration["JwtSettings:Issuer"]//comment this cause i use option pattern  ,
            ValidAudience = jwtSettings.Audience, //builder.Configuration["JwtSettings:Audince"],
                                                  // symetric key to encrypt                  // and you encode your string cause jwt works in bytes 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key /*builder.Configuration["JwtSettings:SecretKey"]*/)),
            ClockSkew = TimeSpan.Zero// that's for  give more addtion time after token expiry (zero no more time after expiry )
        };
    })
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { });

    builder.Services.AddAuthorization();

    //this make add configration from assemply
    builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
    /*builder.Services.AddAutoMapper(cfj=>
    {
        cfj.AddProfile<HotelsMappingProfile>();
        cfj.AddProfile<AuthMAppingProfile>();
        cfj.AddProfile<CountryMappingProfile>();
        });*/

    // Add services to IOC the container. to regestier our dependncy for di 
    builder.Services.AddScoped<ICountryService, CountryService>();
    builder.Services.AddScoped<IHotelService, HotelService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IBookingService, BookingService>();
    // here add global exption handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddMemoryCache();


    var app = builder.Build();
    // add middleware fore exption handler
    app.UseExceptionHandler();
    // midlle ware that's to add default end point for auth if useing builder.Services.AddIdentityApiEndpoints 
    app.MapGroup("api/auth").MapIdentityApi<AppUser>();// map group to add path names

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("pass all piplins");

    app.Run();


}
catch (Exception ex) 
{
    Log.Fatal(ex,"pass all piplins");
}

