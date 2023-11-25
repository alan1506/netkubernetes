using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetKubernetes.Data;
using NetKubernetes.Data.Inmuebles;
using NetKubernetes.Data.Usuarios;
using NetKubernetes.Middelware;
using NetKubernetes.Models;
using NetKubernetes.Profiles;
using NetKubernetes.Token;

//Agregamos la cadena de concexion y logs de las consultas -----------
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
     LogLevel.Information).EnableSensitiveDataLogging();
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});

//var connectionMySqlString = builder.Configuration.GetConnectionString("MySqlConnection");
//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseMySql(connectionMySqlString, ServerVersion.AutoDetect(connectionMySqlString));
//});

builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();

// Add services to the container.
//Agregamos permisos del conttolador -----------------
builder.Services.AddControllers(opt => {
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//referenciamos el mapper para configurarlo -----------
var mapperConfig = new MapperConfiguration( mc =>{
    mc.AddProfile(new InmuebleProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//Referencimaos el builder Security------
var builderSecurity = builder.Services.AddIdentityCore<UsuarioModel>();
var identityBuilder = new IdentityBuilder(builderSecurity.UserType, builder.Services);
identityBuilder.AddEntityFrameworkStores<AppDbContext>();
identityBuilder.AddSignInManager<SignInManager<UsuarioModel>>();
builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddScoped<IJwtGenerador, JwtGenerador>();
builder.Services.AddScoped<IUsuarioSesion, UsuarioSesion>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

//Se agrega la seguridad del token---------
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer( opt => {
            opt.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuer = false
            };
        });

//agregar cors para las peticiones---------
builder.Services.AddCors(o => o.AddPolicy("corsapp", builder => {
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ManagerMiddelware>();

app.UseAuthentication();
//agregar el cors ------------------
app.UseCors("corsapp");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//agregar la migracion para que se generen las tablas en la bd---------
using(var ambiente = app.Services.CreateScope())
{
    var services = ambiente.ServiceProvider;
    try{
        var userManager = services.GetRequiredService<UserManager<UsuarioModel>>();
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await LoadDataBase.InsertData(context, userManager);
    }catch(Exception ex){
        var loggin = services.GetRequiredService<ILogger<Program>>();
        loggin.LogError(ex, "Ocurrio un error en la migracion");
    }
}

app.Run();
