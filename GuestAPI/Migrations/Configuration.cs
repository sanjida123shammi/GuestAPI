namespace GuestAPI.Migrations
{
    using GuestAPI.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GuestAPI.Models.GuestDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(GuestAPI.Models.GuestDbContext context)
        {
            context.Users.AddOrUpdate(u => u.UserId,
                new User { UserId = 1, Username = "shammi", Password = "1234", Email = "shammi@gmail.com", Roles = "Admin, User" },
                new User { UserId = 2, Username = "shashi", Password = "1234", Email = "shashi@gmail.com", Roles = "User" }
            );
            context.Facilities.AddOrUpdate(f => f.FacilityId,
                new Facility { FacilityId = 1, FacilityName = "Fitness" },
                new Facility { FacilityId = 2, FacilityName = "Swimming Pool" },
                new Facility { FacilityId = 3, FacilityName = "Meeting room" }
            );
        }
    }
}
