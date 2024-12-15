using CircleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Helpers
{
    public static class DbInitializer
    {

        public static async Task SeedAsync(AppDbContext appDbContext)
        {
            if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            {
                var newUser = new User()
                {
                    FullName = "Igli Braho",
                    ProfilePictureUrl = "https://images.pexels.com/photos/14653174/pexels-photo-14653174.jpeg"
                };
                await appDbContext.Users.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();

                var newPostWithoutImage = new Post()
                {
                    Content = "This is the first post loaded from the database and it has been created" +
                    "using the test user.",
                    ImageUrl = "",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                var newPostwithImage = new Post()
                {
                    Content = "This is the first post loaded from the database and it has been created" +
                    "using the test user. this post has an image",
                    ImageUrl = "https://wallpapers.com/images/featured/cool-profile-picture-87h46gcobjl5e4xu.jpg",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                await appDbContext.AddRangeAsync(newPostWithoutImage, newPostwithImage);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
