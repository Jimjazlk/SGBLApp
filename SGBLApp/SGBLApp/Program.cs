using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGBLApp.Infraestructure.Persistence.Context;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Infraestructure.Persistence.Services;
using SGBLApp.Core.Application.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Repositories;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Services;
using SGBLApp.Core.Application.Mappings;
using SGBLApp.Infrastructure.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

// Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IBookFeedbackService, BookFeedbackService>();



builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<LoanSettings>(builder.Configuration.GetSection("LoanSettings"));
builder.Services.AddHostedService<LoanStatusService>();


// Mapping
builder.Services.AddAutoMapper(typeof(GeneralProfile));

#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false; // Permitir contraseñas sin símbolos

    // Configuración de usuario
    options.User.RequireUniqueEmail = true; // Emails únicos

    // Configuración de bloqueo (para intentos fallidos)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Confirmación de email (si usarás correos)
    options.SignIn.RequireConfirmedEmail = false; // No requiere confirmación por correo.
})
.AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders(); // Necesario para generación de tokens (ej. reset de contraseña)


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Tiempo de expiración de la sesión
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LibrarianOnly", policy => policy.RequireRole("Librarian"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});
#endregion

#region Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
#endregion

var app = builder.Build();

#region DefaultAdmin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Crear rol "Admin" si no existe
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("Librarian"))
    {
        await roleManager.CreateAsync(new IdentityRole("Librarian"));
    }

    // Crear rol "User" si no existe
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    // Crear usuario admin si no existe
    var adminUser = await userManager.FindByEmailAsync("admin@biblioteca.com");
    if (adminUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = "admin@biblioteca.com",
            Email = "admin@biblioteca.com",
            Name = "Administrador",
            LibraryId = 1,
            CreationDate = DateTime.Now
        };

        var result = await userManager.CreateAsync(user, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
    var librarianUser = await userManager.FindByEmailAsync("librarian@biblioteca.com");
    
    if (librarianUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = "librarian@biblioteca.com",
            Email = "librarian@biblioteca.com",
            Name = "Test Librarian",
            LibraryId = 1,
            CreationDate = DateTime.Now
        };

        var resultLib = await userManager.CreateAsync(user, "Librarian123!");
        if (resultLib.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Librarian");
        }
    }
}
#endregion


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
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
