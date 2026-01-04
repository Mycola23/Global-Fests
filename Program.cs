using GlobalFests.Data;
using GlobalFests.EFModels;
using GlobalFests.Repositories;
using GlobalFests.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using static GlobalFests.Repositories.UserRepository;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<GlobalFestsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<GlobalFestsContext>());

// Register Repositories
builder.Services.AddScoped<ICRUD<Country>, CountryRepository>();
builder.Services.AddScoped<ICRUD<EventType>, EventTypeRepository>();
builder.Services.AddScoped<ICRUD<Genre>, GenreRepository>();
builder.Services.AddScoped<IPerformerRepository, PerformerRepository>();
builder.Services.AddScoped<ICRUD<Role>, RoleRepository>();
builder.Services.AddScoped<ICRUD<Ticket>, TicketRepository>();
builder.Services.AddScoped<IUserRepo, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();


// Register Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IOrganizerStatsService, OrganizerStatsService>();
builder.Services.AddScoped<AdminManageItemsService>();
// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // to show not just "" but real field name that invalid 
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (value, fieldName) => $"The {fieldName} field is required.");

    // 2. Handles unknown value failures
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
        (value) => "Please select a valid option.");
});





var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
