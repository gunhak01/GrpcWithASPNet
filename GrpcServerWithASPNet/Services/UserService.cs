using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcInterface;
using Microsoft.AspNetCore.Authorization;
using Share.Utility;

namespace GrpcServerWithASPNet.Services
{
  [Authorize]
  public class UserService : User.UserBase
  {
    private readonly ILogger<UserService> _logger;
    private readonly JwtTokenHelper _jwtTokenHelper;
    public UserService(ILogger<UserService> logger)
    {
      _logger = logger;
      _jwtTokenHelper = new JwtTokenHelper();
    }
    [AllowAnonymous]
    public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
      var httpContext = context.GetHttpContext();
      return Task.FromResult(new LoginResponse
      {
        //id와 pw와 맞다면 토큰을 전달하는 코드 추가해야함
        Result = true,
        AccessToken = _jwtTokenHelper.GenerateToken(request.Id)
      });
    }
    public override Task<AuthHelloResponse> AuthHello(Empty request, ServerCallContext context)
    {
      var httpContext = context.GetHttpContext();

      var nameClaim = httpContext.User?.Claims?.SingleOrDefault(c => c.Type == "Name");
      return Task.FromResult(new AuthHelloResponse
      {
        Message = $"Auth Success Hello {nameClaim.Value}",
        Result = true
      });
    }
  }
}