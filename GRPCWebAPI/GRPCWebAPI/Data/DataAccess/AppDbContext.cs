using Bogus;
using GRPCWebAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GRPCWebAPI.Data.DataAccess
{
    public class AppDbContext:DbContext
    {

        public DbSet<User> UserTBL { set; get; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename = ./Data/DataStore/Dummy.db");// you fetch from appsettings
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<User> Users = GetSeedData();

            modelBuilder.Entity<User>().HasData(Users);

            base.OnModelCreating(modelBuilder);
        }

        private static List<User> GetSeedData()
        {
            var faker = new Faker<User>();
            faker.RuleFor(x => x.Name, x => x.Person.FullName);
            faker.RuleFor(x => x.Id, x => Guid.NewGuid().ToString());
            faker.RuleFor(x => x.Email, x => x.Person.Email);
            faker.RuleFor(x => x.Password, x => "TestGRP");
            var Users = faker.Generate(30);
            return Users;
        }
    }
}
