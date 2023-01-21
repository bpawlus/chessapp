using ChessWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using ChessWebApp.Core;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MvcGameContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MvcGameContext")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".ChessWebApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    Seed.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseSession();

app.UseWebSockets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

GameFinder.HostGameIfPossible();
app.Run();


/*
using ChessWebApp.Core;

//ClientWebSocket ws1 = null;
ChessWebApp.Models.User u1 = new ChessWebApp.Models.User();
u1.Id = 0;
u1.Name = "user1";
u1.Password = "user1";
u1.Description = "TestPlayer1";

u1.VariantPawn1 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn2 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn3 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn4 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn5 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn6 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn7 = (short)ChessPiecesEnum.ClassicPawn;
u1.VariantPawn8 = (short)ChessPiecesEnum.ClassicPawn;

u1.VariantQueen = (short)ChessPiecesEnum.ClassicQueen;
u1.VariantKing = (short)ChessPiecesEnum.ClassicKing;
u1.VariantBishopLeft = u1.VariantBishopRight = (short)ChessPiecesEnum.ClassicBishop;
u1.VariantKnightLeft = u1.VariantKnightRight = (short)ChessPiecesEnum.ClassicKnight;
u1.VariantRookLeft = u1.VariantRookRight = (short)ChessPiecesEnum.ClassicRook;

Console.WriteLine((short)ChessPiecesEnum.ClassicPawn);
*/

//ChessGameController cgc = ChessGameController(new ChessPlayer(new ChessWebApp.Models.User(), ChessPlayer bottomPlayer)
