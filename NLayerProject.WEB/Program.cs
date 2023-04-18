using Autofac.Extensions.DependencyInjection;
using Autofac;
using NLayerProject.WEB.Modules.AutoFacModule;
using Microsoft.EntityFrameworkCore;
using NLayerProject.Repository.Context;
using System.Reflection;
using NLayerProject.Service.Mapping;
using FluentValidation.AspNetCore;
using NLayerProject.Service.Validations.FluentValidation;
using NLayerProject.WEB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>()); ;

builder.Services.AddAutoMapper(typeof(MapProfile));


builder.Services.AddDbContext<AppDbContext>(x =>
{
    //*Fetch the SqlConnection one
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        //*type safe call
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});

builder.Services.AddScoped(typeof(NotFoundFilter<>));


builder.Host.UseServiceProviderFactory
    (new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));



var app = builder.Build();
app.UseExceptionHandler("/Home/Error");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
