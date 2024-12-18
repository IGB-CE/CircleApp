﻿using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{
    public class PostsService : IPostsService
    {
        private readonly AppDbContext _context;
        public PostsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> getAllPostsAsync(int loggedInUserId)
        {
            var allPosts = await _context.Report
                .Where(n => (!n.isPrivate || n.UserId == loggedInUserId) && n.Reports.Count <= 5 && !n.isDeleted)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .Include(n => n.Reports)
                .OrderByDescending(n => n.Id)
                .ToListAsync();
            return allPosts;
        }

        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> createPostAsync(Post post, IFormFile image)
        {

            //Check and save the image
            if (image != null && image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/posts");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await image.CopyToAsync(stream);

                    //Set the Url to the new post object
                    post.ImageUrl = "/images/posts/" + fileName;
                }
            }
            await _context.Report.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }


        public async Task RemovePostAsync(int postId)
        {
            var postDb = await _context.Report.FirstOrDefaultAsync(n => n.Id == postId);

            if (postDb != null)
            {
                //_context.Posts.Remove(postDb);
                postDb.isDeleted = true;
                _context.Report.Update(postDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePostCommentAsync(int commentId)
        {
            var commentDb = _context.Comments.FirstOrDefault(n => n.Id == commentId);

            if (commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReportPostAsync(int postId, int userId)
        {
            var newReport = new Report()
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.UtcNow,
            };

            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();
        }

        public async Task TogglePostFavoriteAsync(int postId, int userId)
        {
            //check if user has already favorited the post
            var favorite = await _context.Favorites
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postId,
                    UserId = userId,
                };
                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task TogglePostLikeAsync(int postId, int userId)
        {
            //check if user has already liked the post
            var like = await _context.Likes
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postId,
                    UserId = userId,
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }
        }

        public async Task TogglePostVisibilityAsync(int postId, int userId)
        {
            //get post by id and logged in user id
            var post = await _context.Report
                .FirstOrDefaultAsync(l => l.Id == postId && l.UserId == userId);

            if (post != null)
            {
                post.isPrivate = !post.isPrivate;
                _context.Report.Update(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
