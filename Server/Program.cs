using Server.Hubs;
using Server.Models;
using Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opt =>
    new Dictionary<string, UserConnection>());
builder.Services.AddSingleton<IDictionary<string, GroupUserConnection>>(opt =>
	new Dictionary<string, GroupUserConnection>());
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChatHub>("/chat");

app.Run();
