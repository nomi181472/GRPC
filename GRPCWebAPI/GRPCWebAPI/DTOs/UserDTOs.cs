namespace GRPCWebAPI.DTOs
{
    public class UserLoginRequest
    {
        public string Email { get; set; }   
        public string Password { get; set; }
    }
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }

    }
    public class UserRegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserRegistrationResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }

    }
}
