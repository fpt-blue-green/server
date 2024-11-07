using Server.Hubs;
using Server.Models;
using Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
	{
        builder.WithOrigins("http://localhost:3000", "http://localhost:7070","*")
			  .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
	});
});
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opt =>
    new Dictionary<string, UserConnection>());
builder.Services.AddSingleton<IDictionary<string, GroupUserConnection>>(opt =>
	new Dictionary<string, GroupUserConnection>());
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ICampaignChatService, CampaignChatService>();
builder.Services.AddScoped<IChatMemberService, ChatMemberService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");
app.MapHub<ChatHub>("/chat");
app.MapHub<GroupChatHub>("/groupchat");



app.Run();
