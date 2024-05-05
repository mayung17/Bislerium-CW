using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            List<Blog> blogList = new List<Blog>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7250/GetBlogs"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogList = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
            return View(blogList);

        }

        public IActionResult CreateBlog()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateBlog(Blog blog,IFormFile? ImageFile)
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
                return Json(blog);

                StringContent content = new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/AddBlog", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }
            return RedirectToAction("Index", "Blog");

        }
    }
}
