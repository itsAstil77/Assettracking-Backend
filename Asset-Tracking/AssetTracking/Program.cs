using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;
using AssetTrackingAuthAPI.Models;
using YourNamespace.Models;
using YourNamespace.Services;
using YourNamespace.Controllers;


var builder = WebApplication.CreateBuilder(args);

// === Configure MongoDB Settings ===
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<MongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// === Configure Email Settings ===
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);

// === Configure JWT Settings ===
// builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
// var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

// === Enable CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// === Add Authentication ===
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero,
//         ValidIssuer = jwtSettings.Issuer,
//         ValidAudience = jwtSettings.Audience,
//         IssuerSigningKey = new SymmetricSecurityKey(key)
//     };
// });

// === Add Authorization ===
// builder.Services.AddAuthorization(options =>
// {
//     options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
//         .RequireAuthenticatedUser()
//         .Build();
// });

// === Add Swagger with JWT support ===
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Asset Tracking API", Version = "v1" });
  c.CustomSchemaIds(type => type.FullName);



    // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    // {
    //     Description = "Enter 'Bearer {your token}'",
    //     Name = "Authorization",
    //     In = ParameterLocation.Header,
    //     Type = SecuritySchemeType.ApiKey,
    //     Scheme = "Bearer",
    //     BearerFormat = "JWT"
    // });

    // c.AddSecurityRequirement(new OpenApiSecurityRequirement
    // {
    //     {
    //         new OpenApiSecurityScheme
    //         {
    //             Reference = new OpenApiReference
    //             {
    //                 Type = ReferenceType.SecurityScheme,
    //                 Id = "Bearer"
    //             }
    //         },
    //         new string[] { }
    //     }
    // });
});

// === Register Services ===
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<RoleAccessService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<GroupService>();
builder.Services.AddSingleton<DepartmentService>();
builder.Services.AddSingleton<CustodianService>();
builder.Services.AddSingleton<SupplierService>();
builder.Services.AddSingleton<SectorService>();
builder.Services.AddSingleton<CompanyService>();
builder.Services.AddSingleton<SiteService>();
builder.Services.AddSingleton<BuildingService>();
builder.Services.AddSingleton<FloorService>();
builder.Services.AddSingleton<RoomService>();
builder.Services.AddSingleton<HierarchyService>();
builder.Services.AddSingleton<MainCategoryService>();
builder.Services.AddSingleton<SubCategoryService>();
builder.Services.AddSingleton<SubSubCategoryService>();
builder.Services.AddSingleton<BrandService>();
builder.Services.AddSingleton<ModelService>();
builder.Services.AddSingleton<HierarchyCategoryService>();
builder.Services.AddSingleton<AssetService>();
//builder.Services.AddSingleton<RoleService>();

builder.Services.AddSingleton<AssetAuditService>();
builder.Services.AddSingleton<AssetMovementService>();
builder.Services.AddSingleton<AssetDisposalService>();
builder.Services.AddSingleton<AssetCheckoutService>();
builder.Services.AddSingleton<AssetCheckinService>();
builder.Services.AddSingleton<AssetReportService>();
builder.Services.AddSingleton<PurchaseInfoService>();
builder.Services.AddSingleton<ApprovalWorkflowService>();

// === Add Controllers ===
builder.Services.AddControllers();

var app = builder.Build();

// === Use CORS ===
app.UseCors("AllowAll");

// === Swagger UI ===
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Asset Tracking API V1");
    c.RoutePrefix = string.Empty;
});

// === Authorization Header Fix Middleware ===
// app.Use(async (context, next) =>
// {
//     if (context.Request.Headers.ContainsKey("Authorization"))
//     {
//         var authHeader = context.Request.Headers["Authorization"].ToString();
//         if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//         {
//             context.Request.Headers["Authorization"] = "Bearer " + authHeader;
//         }
//     }

//     await next();
// });

// === HTTPS, Auth, Routing Middleware ===
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
