using Application.Service;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Application.Keyboard;
using Application.Interface;
using Application.Service.Auth;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Bot tokenini olish
var token = builder.Configuration["TelegramBot:Token"];
var webhookUrl = builder.Configuration["TelegramBot:WebhookUrl"];

// Add DbContext to services container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
/*
builder.Services.AddSingleton(p => new TelegramBotClient(token)); // Telegram Bot Client
builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>(); // Handler
builder.Services.AddHostedService<BotBackGroundService>(); // Botni backgroundda ishga tushurish
builder.Services.AddSingleton<InlineKeyboards>();
builder.Services.AddSingleton<AuthService>();*/
//builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton(p => new TelegramBotClient(token)); // Telegram Bot Client
builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>(); // Handler
builder.Services.AddHostedService<BotBackGroundService>(); // Botni backgroundda ishga tushurish
builder.Services.AddSingleton<InlineKeyboards>();
builder.Services.AddScoped<IPasswordHasher<Domain.Class.Users>, PasswordHasher<Domain.Class.Users>>();


var app = builder.Build();
app.Run();
