using Domain.Entity;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Security.Claims;
using System.Text;

namespace Front_End.Controllers
{
    public class BlogController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public BlogController(IWebHostEnvironment env)
        {
            _env = env;

        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Blog> blogList = new List<Blog>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7250/api/Blog/GetBlogs"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogList = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
            blogList = blogList.Where(b => b.User == userId ).ToList();

            return View(blogList);

        }
        public async Task<IActionResult> LatestBlog()
        {
      
            List<Blog> blogList = new List<Blog>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7250/api/Blog/GetBlogs"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogList = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
            blogList = blogList
 .OrderByDescending(b => b.CreatedDate)
    .Take(4)
    .ToList();


            return View(blogList);

        }
        public async Task<IActionResult> Blogs(string sortField="random", int pageNumber = 1, int pageSize = 9 )
        {
            
            List<Blog> blogList = new List<Blog>();
            string apiUrl = $"https://localhost:7250/api/Blog/GetBlogspag?pageNumber={pageNumber}&pageSize={pageSize}&sortField={sortField}";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogList = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            var totalBlogsCount = await GetTotalBlogsCount();
            ViewBag.PageCount = (int)Math.Ceiling((double)totalBlogsCount / pageSize);

            return View(blogList);
        }
        private async Task<int> GetTotalBlogsCount()
        {
            var apiUrl = "https://localhost:7250/api/Blog/GetBlogs";

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response into a collection of objects
                        var blogs = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);

                        // Get the count of objects
                        int count = blogs.Count;

                        // You can return the count or do any other processing with it
                        return count;
                    }
                    else
                    {
                        // Handle error if needed
                        return 0;
                    }
                }
            }

        }
        public async Task<IActionResult> BlogDetail(Guid id)
        {
            Blog blog;
            List<Comment> comments;

            // Fetch the blog data
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7250/api/Blog/GetBlog?id={id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }

            // Fetch the comments associated with the blog
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7250/api/Comment/GetComments"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    comments = JsonConvert.DeserializeObject<List<Comment>>(apiResponse);
                }
                comments = comments.Where(b => b.BlogId == blog.Id).ToList();
            }
            var model = new BlogDetailViewModel
            {
                Blog = blog,
                Comments = comments
            };


            return View(model);
        }



        public IActionResult CreateBlog()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateBlog(Blog blog, IFormFile? ImageFile)
        {
            // blog.Id = Guid.NewGuid();
            if (ImageFile != null)
            {
                string filename = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                string imgpath = Path.Combine(_env.WebRootPath, "Images/Blogs/", filename);
                using (FileStream streamread = new FileStream(imgpath, FileMode.Create))
                {
                    ImageFile.CopyTo(streamread);
                }
                blog.ImageName = filename;
            }

            using (var httpClient = new HttpClient())
            {
                // return Json(blog);

                StringContent content = new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/Blog/AddBlog", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }
            return RedirectToAction("Index", "Blog");

        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid Id)
        {
            Blog reservation = new Blog();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7250/api/Blog/GetBlog?id=" + Id))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<Blog>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
            }
            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Blog blog, IFormFile? ImageFile)
        {
            if (ImageFile != null)
            {
                string filename = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                string imgpath = Path.Combine(_env.WebRootPath, "Images/Blogs/", filename);
                using (FileStream streamread = new FileStream(imgpath, FileMode.Create))
                {
                    ImageFile.CopyTo(streamread);
                }
                blog.ImageName = filename;
            }

            using (var httpClient = new HttpClient())
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync("https://localhost:7250/api/Blog/UpdateBlog", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }
            return RedirectToAction("Index", "Blog");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7250/api/Blog/DeleteBlog?Id=" + Id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("Index");

        }


        [HttpPost]
        public async Task<IActionResult> UpvoteLike(Like like)
        {
            like.ReactionType = true;

            using (var httpClient = new HttpClient())
            {


                StringContent content = new StringContent(JsonConvert.SerializeObject(like), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/LikeBlog/Upvote", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                }
            }
            return RedirectToAction("BlogDetail", "Blog", new { id = like.Blog });

        }

        [HttpPost]
        public async Task<IActionResult> DownvoteLike(Like like)
        {
            like.ReactionType = false;

            using (var httpClient = new HttpClient())
            {


                StringContent content = new StringContent(JsonConvert.SerializeObject(like), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/LikeBlog/Downvote", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                }
            }
            return RedirectToAction("BlogDetail", "Blog", new { id = like.Blog });

        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            

            // Add the comment to the database
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/Comment/AddComment", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Comment added successfully, redirect to the blog page
                        return RedirectToAction("BlogDetail", "Blog", new { id = comment.BlogId });
                    }
                    else
                    {
                        // Handle error
                        ModelState.AddModelError(string.Empty, "Failed to add comment");
                    }
                }
            }

            // If the comment was not added successfully, return to the blog page
            return RedirectToAction("BlogDetail", "Blog", new { id = comment.BlogId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid Id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7250/api/Comment/DeleteComment?Id=" + Id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("SingleBlog", "Blog", new { id = Id });
            //return View();
        }



        [HttpGet]
        public async Task<IActionResult> UpdateComment(Guid Id)
        {
            Comment reservation = new Comment();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7250/api/Comment/GetComment?id=" + Id))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<Comment>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
            }
            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComment(Comment comment)
        {
            using (var httpClient = new HttpClient())
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync("https://localhost:7250/api/Comment/UpdateComment", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }
            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public async Task<IActionResult> UpvoteCommentLike(LikeComment likeComment,Guid blogId)
        {
            likeComment.ReactionType = true;
            Guid id= blogId;


            using (var httpClient = new HttpClient())
            {


                StringContent content = new StringContent(JsonConvert.SerializeObject(likeComment), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/LikeComment/UpvoteCmt", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                }
            }
            return RedirectToAction("BlogDetail", "Blog", new { id = id });

        }

        [HttpPost]
        public async Task<IActionResult> DownvoteCommentLike(LikeComment likeComment, Guid blogId)
        {
            likeComment.ReactionType = false;
            Guid id = blogId;

            using (var httpClient = new HttpClient())
            {


                StringContent content = new StringContent(JsonConvert.SerializeObject(likeComment), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/LikeComment/DownvoteCmt", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                }
            }
            return RedirectToAction("BlogDetail", "Blog", new { id = id });

        }
    }

}