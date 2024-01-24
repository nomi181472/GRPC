namespace GRPCWebAPI.Data.Models
{
    public class User
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } //we are not use any encryption with salt etc. we should remain on the main topic.

    }
}
