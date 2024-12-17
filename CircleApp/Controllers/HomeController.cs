using CircleApp.Data;
using CircleApp.Data.Helpers;
using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _appDbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            int loggedInUserId = 1;

            var allPosts = await _appDbContext.Posts
                .Where(n => (!n.isPrivate || n.UserId == loggedInUserId) && n.Reports.Count <= 5 && !n.isDeleted)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .Include(n => n.Reports)
                .OrderByDescending(n => n.Id)
                .ToListAsync();
            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            //Get the logged in user
            int loggedInUser = 1;

            //Create a new post
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = "",
                NrOfReports = 0,
                UserId = loggedInUser
            };

            //Check and save the image
            if (post.Image != null && post.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/posts");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await post.Image.CopyToAsync(stream);

                    //Set the Url to the new post object
                    newPost.ImageUrl = "/images/posts/" + fileName;
                }
            }

            //Add the post to database
            await _appDbContext.Posts.AddAsync(newPost);
            await _appDbContext.SaveChangesAsync();

            //Find and store the hashtags
            var postHashtags = HashtagHelper.GetHashtags(post.Content);
            foreach (var hashTag in postHashtags)
            {
                var hashtagDb = await _appDbContext.Hashtags.FirstOrDefaultAsync(n => n.Name == hashTag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count += 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;

                    _appDbContext.Hashtags.Update(hashtagDb);
                    await _appDbContext.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new Hashtag()
                    {
                        Name = hashTag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                    };
                    await _appDbContext.Hashtags.AddAsync(newHashtag);
                    await _appDbContext.SaveChangesAsync();
                }
            }
            //Redirect to the index page
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            int loggedInUserId = 1;

            //check if user has already liked the post
            var like = await _appDbContext.Likes
                .Where(l => l.PostId == postLikeVM.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                _appDbContext.Likes.Remove(like);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postLikeVM.PostId,
                    UserId = loggedInUserId,
                };
                await _appDbContext.Likes.AddAsync(newLike);
                await _appDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            int loggedInUserId = 1;

            //check if user has already favorited the post
            var favorite = await _appDbContext.Favorites
                .Where(l => l.PostId == postFavoriteVM.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _appDbContext.Favorites.Remove(favorite);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postFavoriteVM.PostId,
                    UserId = loggedInUserId,
                };
                await _appDbContext.Favorites.AddAsync(newFavorite);
                await _appDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            int loggedInUserId = 1;
            //get post by id and logged in user id
            var post = await _appDbContext.Posts
                .FirstOrDefaultAsync(l => l.Id == postVisibilityVM.PostId && l.UserId == loggedInUserId);

            if (post != null)
            {
                post.isPrivate = !post.isPrivate;
                _appDbContext.Posts.Update(post);
                await _appDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            //logged in
            int loggedInUserId = 1;

            //create a post object
            var newComment = new Comment()
            {
                UserId = loggedInUserId,
                PostId = postCommentVM.PostId,
                Content = postCommentVM.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };
            await _appDbContext.Comments.AddAsync(newComment);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            //logged in
            int loggedInUserId = 1;

            //create a post object
            var newReport = new Report()
            {
                UserId = loggedInUserId,
                PostId = postReportVM.PostId,
                DateCreated = DateTime.UtcNow,
            };
            await _appDbContext.Reports.AddAsync(newReport);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
        {
            var commentDb = await _appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == removeCommentVM.CommentId);

            if (commentDb != null)
            {
                _appDbContext.Comments.Remove(commentDb);
                await _appDbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveVM postRemoveVM)
        {
            var postDb = await _appDbContext.Posts.FirstOrDefaultAsync(c => c.Id == postRemoveVM.PostId);

            if (postDb != null)
            {
                postDb.isDeleted = true;
                _appDbContext.Posts.Update(postDb);
                await _appDbContext.SaveChangesAsync();

                //update hashtags
                var postHashtags = HashtagHelper.GetHashtags(postDb.Content);
                foreach (var hashtag in postHashtags)
                {
                    var hashtagDb = await _appDbContext.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
                    if (hashtagDb != null)
                    {
                        hashtagDb.Count -= 1;
                        hashtagDb.DateUpdated = DateTime.UtcNow;

                        _appDbContext.Hashtags.Update(hashtagDb);
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}