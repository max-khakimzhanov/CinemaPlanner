using Asp.Versioning;
using CinemaPlanner.Web.Data;
using CinemaPlanner.Web.Filters;
using CinemaPlanner.Web.Middleware;
using CinemaPlanner.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddDbContext<CinemaPlannerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<SeatLayoutService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<BookingEventSubscriber>();
builder.Services.AddScoped<IBookingAppService, BookingAppService>();
builder.Services.AddScoped<IBookingNotifier, LoggingBookingNotifier>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IScreeningService, ScreeningService>();
builder.Services.AddScoped<IHomeDashboardService, HomeDashboardService>();
builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddScoped<MinioStorageService>();
builder.Services.AddScoped<IPosterStorage>(sp => sp.GetRequiredService<MinioStorageService>());
builder.Services.AddScoped<IReceiptStorage>(sp => sp.GetRequiredService<MinioStorageService>());
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePages();
app.UseGlobalErrorHandling();
app.UseRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapGet("/api/min/movies", async (IMovieService movieService) =>
{
    return Results.Ok(await movieService.GetAllAsync());
}).WithName("GetMoviesMinimalList");

app.MapGet("/api/min/movies/{id:int}", async (int id, IMovieService movieService) =>
{
    var movie = await movieService.GetByIdAsync(id);
    return movie is null ? Results.NotFound() : Results.Ok(movie);
}).WithName("GetMovieMinimal");

app.Run();
