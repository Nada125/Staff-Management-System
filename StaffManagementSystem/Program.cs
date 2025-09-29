using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Application.Mappings;
using StaffManagementSystem.Infrastructures.DBContext;
using StaffManagementSystem.Infrastructures.Repositories;
using StaffManagementSystem.Application.Services;
using Microsoft.AspNetCore.Identity;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Domain.Entities;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace StaffManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Staff Management",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         new string[] {}
                    }
                });
            });


            builder.Services.AddDataProtection();

            builder.Services.AddIdentity<Employee, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -_";
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequiredUniqueChars = 0;
                opt.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JwtConfig"));
            builder.Services.AddAutoMapper(typeof(EmployeeProfile).Assembly);
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IEmployeeTaskService, EmployeeTaskService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IReportAiService, ReportAiService>();

            // Bind Google AI config
            builder.Services.Configure<GoogleAiOptions>(builder.Configuration.GetSection("GoogleAI"));

            // Register HttpClient for Google AI
            builder.Services.AddHttpClient("google-ai");



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme =
                options.DefaultForbidScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,


                    ClockSkew = TimeSpan.Zero

                };

            });
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            //    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                   ?? builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string not found!");
            }


            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Retry up to 5 times, max delay 10 seconds between retries
                npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
            }));

            var app = builder.Build();


            // Update-Database with retry
            using (var scope = app.Services.CreateAsyncScope())
            {
                var services = scope.ServiceProvider;
                var _dbContext = services.GetRequiredService<AppDbContext>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();


                var retries = 3;
                for (int attempt = 1; attempt <= retries; attempt++)
                {
                    try
                    {
                       await _dbContext.Database.MigrateAsync();
                        break; 
                    }
                    catch (Exception ex)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(ex, "Error applying migration. Attempt {Attempt} of {Retries}", attempt, retries);

                        if (attempt == retries) throw; 
                        await Task.Delay(2000); 
                    }
                }

            }


            // Seeding Roles
            using (var s = app.Services.CreateScope())
            {
                var roleManager = s.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Employee", "Manager"};

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseCors();
            

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
