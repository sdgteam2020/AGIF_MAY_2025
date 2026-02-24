using Agif_V2.Controllers;
using Agif_V2.Helpers;
using Agif_V2.Middleware;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DocumentFormat.OpenXml.Math;
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
builder.Services.AddTransient<AsymmetricEncryption>();
builder.Services.AddTransient<IClaimCalculator, ClaimCalculatorDL>();
builder.Services.AddTransient<IModelStateLogger, ModelStateLogger>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("http://localhost", "*")
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.HeaderName = "RequestVerificationToken";
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
}).AddRazorRuntimeCompilation();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.CookieTempDataProviderOptions>(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});



builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;  // Strong CSRF protection
});

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365); // 1 year = 31536000 seconds
});

var app = builder.Build();

app.UseRequestLocalization();
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Use(async (ctx, next) =>
{
    var blockedMethods = new[] { "OPTIONS", "TRACE", "TRACK", "CONNECT" };

    if (blockedMethods.Contains(ctx.Request.Method, StringComparer.OrdinalIgnoreCase))
    {
        ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        ctx.Response.Headers["Allow"] = "GET, HEAD, POST";
        await ctx.Response.WriteAsync("Method Not Allowed");
        return;
    }

    ctx.Response.OnStarting(() =>
    {
        ctx.Response.Headers.Remove("Server");
        ctx.Response.Headers.Remove("X-Powered-By");
        ctx.Response.Headers.Remove("x-aspnet-version");
        return Task.CompletedTask;
    });

    var isDev = app.Environment.IsDevelopment();

    string defaultSrc = "default-src 'self' blob:; ";
    string scriptSrc =  "script-src 'self'; ";
    string styleSrc = "style-src 'self'; "; // Bootstrap/JQuery often need unsafe-inline
    string imgSrc = "img-src 'self' data: blob:; "; // Added 'data:' and 'blob:' explicitly
    string fontSrc = "font-src 'self' data:; ";
    string connectSrc = isDev
        ? "connect-src 'self' https://dgisapp.army.mil:55102 ws://localhost:* wss://localhost:*; "
        : "connect-src 'self' https://dgisapp.army.mil:55102; ";
    string other = "frame-ancestors 'none'; base-uri 'self'; object-src 'self' blob:; form-action 'self';";

    ctx.Response.Headers["Content-Security-Policy"] = $"{defaultSrc}{scriptSrc}{styleSrc}{imgSrc}{fontSrc}{connectSrc}{other}";

    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";

    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");


app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");


app.Run();
