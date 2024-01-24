//UserServiceImpl.cs // in this file we will catch our grpc based request and fetch data from service layer
using Grpc.Core;
using GRPCWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using UserGRPC;

namespace GRPCWebAPI.GRPCControllers
{
    [Authorize]
    public class UserServiceImpl: UserGRPC.UserService.UserServiceBase
    {
        readonly IUserService _userService;
        public UserServiceImpl(IUserService userService)
        {
                _userService = userService;
        }
        [AllowAnonymous]
        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var result = await _userService.Login(new DTOs.UserLoginRequest()
            {
                Email = request.Email,
                Password= request.Password,
            });
            if (result.Success)
            {
                return new LoginResponse()
                {
                    StatusCode = 200,
                    Success = result.Success,
                    Token = result.Token
                };
            }
            else
            {
                return new LoginResponse()
                {
                    StatusCode = 400,
                    Token = "",
                    Success = result.Success
                };
            }
            
        }
        [AllowAnonymous]
        public override async Task<RegistrationResponse> Registration(RegistrationRequest request, ServerCallContext context)
        {
            var result = await _userService.Registration(new DTOs.UserRegistrationRequest()
            {
                Email = request.Email,
                Password = request.Password,
                Name = request.Name,

            });
            if (result.Success)
            {
                return new RegistrationResponse()
                {
                    StatusCode = 200,
                    Success = result.Success,
                    Token = result.Token
                };
            }
            else
            {
                return new RegistrationResponse()
                {
                    StatusCode = 400,
                    Token = "",
                    Success = result.Success
                };
            }

        }

        public override async  Task<UsersResponse> GetUsers(Dummy request, ServerCallContext context)
        {
            UsersResponse users = new UsersResponse();
            
           foreach (var user in await _userService.GetUsers())
            {
                users.UserList.Add(new UserResponse()
                {
                    Email= user.Email,
                    Id = user.Id,
                    Name= user.Name,
                    Password= user.Password,
                });
            }
            return users;
        }

    }
}
