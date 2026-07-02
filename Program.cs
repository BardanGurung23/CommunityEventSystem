using CommunityEvent.Components;
using CommunityEvent.Data;
using CommunityEvent.Interfaces;
using CommunityEvent.Repositories;
using CommunityEvent.Services;
using CommunityEvent.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
        options.AccessDeniedPath = "/account/login";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IActivityService, ActivityService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/account/login-admin", async (HttpContext httpContext) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();

    if (!string.Equals(email, "admin@community.test", StringComparison.OrdinalIgnoreCase) || password != "admin123")
    {
        return Results.Redirect("/account/login");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, "Community Admin"),
        new(ClaimTypes.Email, email),
        new(ClaimTypes.Role, AuthConstants.AdminRole)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Redirect("/admin/dashboard");
});

app.MapPost("/account/login-participant", async (HttpContext httpContext, AppDbContext dbContext) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var participant = await dbContext.Participants.FirstOrDefaultAsync(p => p.Email == email && p.IsActive);

    if (participant is null)
    {
        return Results.Redirect("/account/login");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, participant.FullName),
        new(ClaimTypes.Email, participant.Email),
        new(ClaimTypes.Role, AuthConstants.ParticipantRole),
        new(AuthConstants.ParticipantIdClaim, participant.Id.ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Redirect("/my-registrations");
});

app.MapGet("/account/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();