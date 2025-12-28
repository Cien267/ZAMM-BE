using Microsoft.EntityFrameworkCore;
using Zamm.Infrastructure.DataContext;
using Zamm.Application.Constants;
using Zamm.Application.InterfaceService;
using Zamm.Application.ImplementService;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Domain.Entities;
using Zamm.Infrastructure.ImplementRepositories;
using Zamm.Application.Handle.HandleEmail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Zamm.Application;
using Zamm.Data;
using Zamm.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(Constant.AppSettingKeys.DEFAULT_CONNECTION),
        b => b.MigrationsAssembly("Zamm")));

builder.Services.AddScoped<IDbContext, AppDbContext>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBaseRepository<ConfirmEmail>, BaseRepository<ConfirmEmail>>();
builder.Services.AddScoped<IBaseRepository<RefreshToken>, BaseRepository<RefreshToken>>();
builder.Services.AddScoped<IBaseRepository<Permissions>, BaseRepository<Permissions>>();
builder.Services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Register Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Person
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IBaseRepository<Person>, BaseRepository<Person>>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Register Dependent
builder.Services.AddScoped<IDependentService, DependentService>();
builder.Services.AddScoped<IBaseRepository<Dependent>, BaseRepository<Dependent>>();

// Register Address
builder.Services.AddScoped<IBaseRepository<Address>, BaseRepository<Address>>();
builder.Services.AddScoped<IAddressService, AddressService>();

// Register Asset
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IBaseRepository<Asset>, BaseRepository<Asset>>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IBaseRepository<AssetPerson>, BaseRepository<AssetPerson>>();
builder.Services.AddScoped<IBaseRepository<AssetCompany>, BaseRepository<AssetCompany>>();
builder.Services.AddScoped<IBaseRepository<LiabilityAsset>, BaseRepository<LiabilityAsset>>();

// Register Company
builder.Services.AddScoped<IBaseRepository<Company>, BaseRepository<Company>>();

// Register Liability
builder.Services.AddScoped<ILiabilityService, LiabilityService>();
builder.Services.AddScoped<IBaseRepository<Liability>, BaseRepository<Liability>>();
builder.Services.AddScoped<IBaseRepository<FixedRatePeriod>, BaseRepository<FixedRatePeriod>>();
builder.Services.AddScoped<IBaseRepository<LiabilityPerson>, BaseRepository<LiabilityPerson>>();
builder.Services.AddScoped<IBaseRepository<LiabilityCompany>, BaseRepository<LiabilityCompany>>();

// Register Lender
builder.Services.AddScoped<IBaseRepository<Lender>, BaseRepository<Lender>>();

// Register Loan
builder.Services.AddScoped<IBaseRepository<Loan>, BaseRepository<Loan>>();

// Register InterestRate
builder.Services.AddScoped<IBaseRepository<InterestRate>, BaseRepository<InterestRate>>();

// Register Brokerage
builder.Services.AddScoped<IBaseRepository<Brokerage>, BaseRepository<Brokerage>>();
builder.Services.AddScoped<IBaseRepository<BrokerageLogo>, BaseRepository<BrokerageLogo>>();
builder.Services.AddScoped<IBaseRepository<Invitation>, BaseRepository<Invitation>>();
builder.Services.AddScoped<IBrokerageService, BrokerageService>();

// Register Event
builder.Services.AddScoped<IBaseRepository<Event>, BaseRepository<Event>>();
builder.Services.AddScoped<IBaseRepository<EventFile>, BaseRepository<EventFile>>();
builder.Services.AddScoped<IEventService, EventService>();

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Auth API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter the token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type =ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        await context.Database.MigrateAsync();
        
        await DbInitializer.SeedRoles(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
