using AMB.API.Filters;
using AMB.API.Middlewares;
using AMB.Application.Dtos;
using AMB.Application.Interfaces;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Services;
using AMB.Application.Validators;
using AMB.Infra.BackgroundServices;
using AMB.Infra.DBContexts;
using AMB.Infra.Identity;
using AMB.Infra.Notifications;
using AMB.Infra.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "V1",
        Title = "Ambrosia API",
        Description = "API for Ambrosia"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter bearer token",
        Name = "Access Token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type=ReferenceType.SecurityScheme, Id="Bearer" }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<AMBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IAuthHelper, Auth0Service>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICalenderExclusionRepository, CalenderExclusionRepository>();
builder.Services.AddScoped<ICalendarExclusionService, CalenderExclusionService>();

builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<ITableService, TableService>();

builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<IConfigService, ConfigService>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();

builder.Services.AddScoped<IPurchaseRequestRepositoy, PurchaseRequestRepository>();
builder.Services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();

builder.Services.AddScoped<IGoodReceiptNoteRepository, GoodReceiptNoteRepository>();
builder.Services.AddScoped<IGoodReceiptNoteService, GoodReceiptNoteService>();

builder.Services.AddScoped<IGoodsIssueRepository, GoodsIssueRepository>();
builder.Services.AddScoped<IGoodsIssueService, GoodsIssueService>();
builder.Services.AddScoped<IWastageRecordRepository, WastageRecordRepository>();
builder.Services.AddScoped<IWastageRecordService, WastageRecordService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();

builder.Services.AddTransient<IEmailService, ResendEmailService>();

builder.Services.AddHostedService<GlobalWaiterAssignmentService>();

builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken =
        builder.Configuration["Resend_API_Token"] ??
        Environment.GetEnvironmentVariable("Resend_API_Token") ??
        string.Empty;
});

builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.Authority = $"https://{builder.Configuration["Authentication:Domain"]}/";
    options.Audience = builder.Configuration["Authentication:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = $"https://{builder.Configuration["Authentication:Domain"]}/",
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true
    };
})
.AddJwtBearer("GuestBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:GuestIssuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Authentication:GuestAudience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:GuestSecret"] ?? "default_fallback_secret_key_if_missing")),
        ValidateLifetime = true
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"].FirstOrDefault();
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/order-status"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GuestPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add("GuestBearer");
        policy.RequireRole("GuestUser");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000").AllowCredentials();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();

builder.Services.AddScoped<IOrderingSessionService, OrderingSessionService>();

builder.Services.AddSignalR();

// Add Rate Limiting for public endpoints protecting against scraping
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = default;
    options.AddPolicy("QrScanLimit", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 20,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many scan requests. Please try again later.", cancellationToken: token);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseAuthentication();
app.UseMiddleware<ActiveUserMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AMB.Application.Hubs.OrderingHub>("/hubs/order-status");

app.MapGet("/", () => "Ambrosia Backend is Live!");

app.Run();
