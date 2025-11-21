using Agif_V2.Controllers;
using Agif_V2.Helpers;
using Agif_V2.Middleware;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ============================
// DATABASE
// ============================
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("AgifConnection")));

// ============================
// CULTURE
// ============================
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("en-GB")
           .AddSupportedCultures("en-GB", "en-US")
           .AddSupportedUICultures("en-GB", "en-US");

    options.RequestCultureProviders.Insert(0,
        new CustomRequestCultureProvider(ctx =>
            Task.FromResult(new ProviderCultureResult("en-GB"))));
});

// ============================
// IDENTITY CONFIG
// ============================
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

// ============================
// RESPONSE COMPRESSION
// ============================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "font/woff", "font/woff2", "application/font-woff", "application/font-woff2",
        "application/vnd.ms-fontobject", "font/ttf", "font/otf", "application/font-sfnt"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// ============================
// DEPENDENCY INJECTION
// ============================
builder.Services.AddTransient<IOnlineApplication, OnlineApplicationDL>();
builder.Services.AddScoped<IClaimOnlineApplication, ClaimOnlineApplicationDL>();
builder.Services.AddTransient<IAppointment, AppointmentDL>();
builder.Services.AddTransient<IMasterOnlyTable, MasterOnlyTable>();
builder.Services.AddTransient<ICar, CarDL>();
builder.Services.AddTransient<IHba, HbaDL>();
builder.Services.AddTransient<IPca, PcaDL>();
builder.Services.AddTransient<IAddress, AddressDL>();
builder.Services.AddTransient<IAccount, AccountDL>();
builder.Services.AddTransient<IEducation, EducationDL>();
builder.Services.AddTransient<IMarraige, MarraigeDL>();
builder.Services.AddTransient<IProperty, PropertyDL>();
builder.Services.AddTransient<ISpecial, SpecialDL>();
builder.Services.AddTransient<IClaimAccount, ClaimAccountDL>();
builder.Services.AddTransient<IClaimAddress, ClaimAddressDL>();
builder.Services.AddTransient<IArmyPrefixes, ArmyPrefixesDL>();
builder.Services.AddTransient<IDoucmentupload, DocumentUploadDL>();
builder.Services.AddTransient<IClaimDocumentUpload, ClaimDocumentUploadDL>();
builder.Services.AddTransient<IApplication, Application>();
builder.Services.AddTransient<IUserProfile, UserProfileDL>();
builder.Services.AddTransient<IUserMapping, UserMappingDL>();
builder.Services.AddTransient<IUsersApplications, UsersApplicationDL>();
builder.Services.AddTransient<IDefault, DefaultDL>();
builder.Services.AddTransient<IHome, HomeDL>();
builder.Services.AddTransient<PdfGenerator>();
builder.Services.AddTransient<ClaimPdfGenerator>();
builder.Services.AddTransient<PdfUpload>();
builder.Services.AddTransient<MergePdf>();
builder.Services.AddTransient<OnlineApplicationController>();
builder.Services.AddScoped<IErrorLog, ErrorLogDL>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<FileUtility>();
builder.Services.AddTransient<IClaimApplication, ClaimDigitalDL>();
builder.Services.AddTransient<Watermark>();
builder.Services.AddTransient<IClaimCalculator, ClaimCalculatorDL>();

// ============================
// CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",builder =>
        builder.WithOrigins("http://localhost", "*")
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// ============================
// MVC & SESSION
// ============================
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// ============================
// LOCALIZATION
// ============================
app.UseRequestLocalization();

// ============================
// SESSION
// ============================
app.UseSession();

// ============================
// ERROR HANDLING
// ============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// ============================
// SECURITY HEADERS — CSP
// ============================
// Restrict inline JS and CSS
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self';";

    await next();
});

// ============================
// STATIC FILES (AFTER CSP)
// ============================
app.UseHttpsRedirection();
app.UseStaticFiles();

// ============================
// ROUTING & CORS
// ============================
app.UseRouting();
app.UseCors("CorsPolicy");

// ============================
// REFERER VALIDATION
// ============================
app.Use(async (context, next) =>
{
    var referer = context.Request.Headers["Referer"].ToString();
    var path = context.Request.Path.Value;

    if (string.IsNullOrEmpty(referer) &&
        !path.StartsWith("/Default/Index", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith("/css") &&
        !path.StartsWith("/js"))
    {
        context.Response.Redirect("/Default/Index");
        return;
    }

    await next();
});

// ============================
// AUTH
// ============================
app.UseAuthorization();

// ============================
// ROUTES
// ============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");

app.Run();
