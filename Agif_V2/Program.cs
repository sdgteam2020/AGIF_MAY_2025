using Agif_V2.Controllers;
using Agif_V2.Helpers;
using Agif_V2.Middleware;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configration = builder.Configuration;
builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(configration.GetConnectionString("AgifConnection")));
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-GB", "en-US" }; // Use en-GB for dd-MM-yyyy format
    options.SetDefaultCulture(supportedCultures[0])
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
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

builder.Services.AddTransient<IOnlineApplication, OnlineApplicationDL>();
builder.Services.AddScoped<IClaimOnlineApplication, ClaimOnlineApplicationDL>();
builder.Services.AddTransient<IAppointment, AppointmentDL>();
builder.Services.AddTransient<IMasterOnlyTable, MasterOnlyTable>();
builder.Services.AddTransient<ICar, CarDL>();
builder.Services.AddTransient<IHba, HbaDL>();
builder.Services.AddTransient<IPca, PcaDL>();
builder.Services.AddTransient<IEducation,EducationDL>();
builder.Services.AddTransient<IMarraige, MarraigeDL>();
builder.Services.AddTransient<IProperty, PropertyDL>();
builder.Services.AddTransient<ISpecial, SpecialDL>();
builder.Services.AddTransient<IArmyPrefixes, ArmyPrefixesDL>();
builder.Services.AddTransient<IDoucmentupload, DocumentUploadDL>();
builder.Services.AddTransient<IClaimDocumentUpload, ClaimDocumentUploadDL>();
builder.Services.AddTransient<IApplication, Application>();
builder.Services.AddTransient<IUserProfile, UserProfileDL>();
builder.Services.AddTransient<IUserMapping, UserMappingDL>();
builder.Services.AddTransient<IUsersApplications, UsersApplicationDL>();
builder.Services.AddTransient<PdfGenerator>();
builder.Services.AddTransient<MergePdf>();
builder.Services.AddTransient<OnlineApplicationController>();
builder.Services.AddScoped<IErrorLog, ErrorLogDL>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


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
});
var app = builder.Build();
app.UseRequestLocalization();
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
// Add our custom session check middleware before routing
app.UseMiddleware<SessionCheckMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");


app.Run();
