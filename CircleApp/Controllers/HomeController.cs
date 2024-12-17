using CircleApp.Data;
using CircleApp.Data.Helpers;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
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
        private readonly IPostsService _postService;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IPostsService postService)
        {
            _logger = logger;
            _appDbContext = context;
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            int loggedInUserId = 1;
            var allPosts = await _postService.getAllPostsAsync(loggedInUserId);

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

            await _postService.createPostAsync(newPost, post.Image);


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

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            int loggedInUserId = 1;
            await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            int loggedInUserId = 1;
            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUserId);

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

            await _postService.AddPostCommentAsync(newComment);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            //logged in
            int loggedInUserId = 1;

            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
        {
            await _postService.RemovePostAsync(removeCommentVM.CommentId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveVM postRemoveVM)
        {
            await _postService.RemovePostAsync(postRemoveVM.PostId);

                //update hashtags
                //var postHashtags = HashtagHelper.GetHashtags(postDb.Content);
                //foreach (var hashtag in postHashtags)
                //{
                //    var hashtagDb = await _appDbContext.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
                //    if (hashtagDb != null)
                //    {
                //        hashtagDb.Count -= 1;
                //        hashtagDb.DateUpdated = DateTime.UtcNow;

                //        _appDbContext.Hashtags.Update(hashtagDb);
                //        await _appDbContext.SaveChangesAsync();
                //    }
                //}
            return RedirectToAction("Index");
        }
    }
}