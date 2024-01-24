//UserService.cs // in this service we will fetch data from dbcontext and add our logics
using GRPCWebAPI.Data.DataAccess;
using GRPCWebAPI.Data.Models;
using GRPCWebAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GRPCWebAPI.Services
{
    public interface IUserService
    {
        Task<UserLoginResponse> Login(UserLoginRequest request);
        Task<UserRegistrationResponse> Registration(UserRegistrationRequest request);
        Task<IEnumerable<User>> GetUsers();
    }
    public class UserService : IUserService
    {
        readonly AppDbContext _appDbContext;
        readonly IJWTService _jwt;
        public UserService(AppDbContext appDbContext,IJWTService jWT)
        {
            _appDbContext = appDbContext;
            _jwt = jWT;
        }
        public async Task<UserLoginResponse> Login(UserLoginRequest request)
        {
            User? user = await GetUserByEmail(request.Email);
            if (user == null || !user.Password.Equals(request.Password))
            {
                return new UserLoginResponse()
                {
                    Success = false,
                    Token = String.Empty,
                };
            }
            else
            {
                string jwtToken = GetJWTToken(user);
                return new UserLoginResponse()
                {
                    Success = true,
                    Token = jwtToken
                };
            }

        }
        public async Task<UserRegistrationResponse> Registration(UserRegistrationRequest request)
        {
            User? user = await GetUserByEmail(request.Email);
            if (user == null )
            {
                user= new User()
                {
                    Email = request.Email,
                    Name = request.Name,
                    Password = request.Password,
                    Id = Guid.NewGuid().ToString()
                };
                await _appDbContext.UserTBL.AddAsync(user);
                await _appDbContext.SaveChangesAsync();
                string jwtToken = GetJWTToken(user);
                return new UserRegistrationResponse()
                {
                    Success = true,
                    Token = jwtToken,
                };
            }
            else
            {

                return new UserRegistrationResponse()
                {
                    Success = false,
                    Token = String.Empty,
                };
            }
        }
        private string GetJWTToken(User user)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add($"{nameof(user.Id)}", user.Id);
            dict.Add($"{nameof(user.Email)}", user.Email);
            string jwtToken = _jwt.GenerateJWTToken(dict);
            return jwtToken;
        }
        private async Task<User?> GetUserByEmail(string email)
        {
            return await _appDbContext.UserTBL.Where(x => x.Email.ToLower().Equals(email.ToLower())).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return (await _appDbContext.UserTBL.ToListAsync()).Select(x => new User { Email = x.Email, Name = x.Name, Id = x.Id, Password = x.Password });
        }
    }
}
