using GrpcServerWithASPNet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Share.Models;
using Share.Utility;
using System.Text;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

//auth 추가
var config = CustomConfigurationHelper.CreateConfigurationBuilder("./CommonConfiguration/Configuration.json");
config = CustomConfigurationHelper.CreateConfigurationBuilder("./Configuration/Configuration.json");
builder.Services.Configure<TokenManagement>(config.GetSection("TokenManagement"));

var token = config.GetSection("TokenManagement").Get<TokenManagement>();
var secret = Encoding.ASCII.GetBytes(token.Secret);
//var secret = Encoding.UTF8.GetBytes(token.Secret);

builder.Services.AddAuthentication(x =>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{

  x.SaveToken = true;
  x.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
    ValidIssuer = token.Issuer,
    ValidAudience = token.Audience,
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero,

  };
});
//



builder.Services.AddAuthorization();

builder.Services.AddGrpcReflection();

//auth추가
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
  builder.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()
         .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));
//

var app = builder.Build();

if (app.Environment.IsDevelopment())
  app.MapGrpcReflectionService();

//auth 추가
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");
//

// Configure the HTTP request pipeline.

//auth 추가
//app.MapGrpcService<GreeterService>();
app.MapGrpcService<GreeterService>().RequireCors("AllowAll");
app.MapGrpcService<UserService>().RequireCors("AllowAll");
//

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
