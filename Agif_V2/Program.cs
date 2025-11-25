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

var configration = builder.Configuration;
builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(configration.GetConnectionString("AgifConnection")));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-GB", "en-US" };
    options.SetDefaultCulture(supportedCultures[0])
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);

    // Force the request culture to always use "en-GB"
    options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
    {
        return Task.FromResult(new ProviderCultureResult("en-GB"));
    }));
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    option.Password.RequireNonAlphanumeric = true;
    option.Password.RequireUppercase = true;
    option.Password.RequireDigit = true;
    option.Password.RequiredLength = 8;
    option.Password.RequiredUniqueChars = 1;
    option.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    // Add font MIME types
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "font/woff",
        "font/woff2",
        "application/font-woff",
        "application/font-woff2",
        "application/vnd.ms-fontobject",
        "font/ttf",
        "font/otf",
        "application/font-sfnt"
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("http://localhost", "*")
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;  // Strong CSRF protection
});

var app = builder.Build();

app.UseRequestLocalization();
app.UseSession();

//app.UseResponseCompression();
// Configure the HTTP request pipeline.
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
//app.Use(async (context, next) =>
//{
//    context.Response.Headers["Content-Security-Policy"] =
//        "script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self';";

//    await next();
//});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");


app.Run();
