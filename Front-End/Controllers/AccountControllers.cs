using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Domain.ViewModel;

namespace Front_End.Controllers
{
    public class AccountControllers : Controller

    {
        private readonly IWebHostEnvironment _env;
        public AccountControllers(IWebHostEnvironment env)
        {
            _env = env;

        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel1 registerModel)
        {

            using (var httpClient = new HttpClient())
            {
                // return Json(blog);

                StringContent content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/Account/register", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Registration successful, redirect to login page
                        return RedirectToAction("Login", "AccountControllers");
                    }
                    else
                    {
                        return RedirectToAction("Register", "AccountControllers");
                    }

                }
            }
            return View("Error");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7250/api/Account/LoginUser", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(apiResponse);

                    var token = dict["token"];


                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                    // Extract claims from the JWT
                    var claims = jsonToken?.Claims;

                    if (claims != null)
                    {
                        // Create a claims identity
                        var claimsIdentity = new ClaimsIdentity(claims, "login");

                        // Create authentication properties
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1), // Set expiration time for the cookie
                            IsPersistent = true
                        };

                        // Sign in the user
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    }

                }
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect the user to the home page or any other desired page after logout
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> ProfileView(string Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IdentityUser reservation = new IdentityUser();
            List<Blog> blogList = new List<Blog>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7250/api/Account/get/{Id}"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<IdentityUser>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
                using (var response = await httpClient.GetAsync("https://localhost:7250/api/Blog/GetBlogs"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    blogList = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                }
            }
                blogList = blogList.Where(b => b.User == userId).ToList();
            var viewModel = new ProfileViewModel
            {
                User = reservation,
                AuthoredBlogPosts = blogList,

            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileUpdate(string Id)
        {
        
            IdentityUser reservation = new IdentityUser();
         
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://localhost:7250/api/Account/get/{Id}"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<IdentityUser>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
               
            }
     
           
            return View(reservation);
        }



        [HttpPost]
        public async Task<IActionResult> ProfileUpdate(UpdateModel user,string id)
        {
        

            using (var httpClient = new HttpClient())
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync($"https://localhost:7250/api/Account/update/{id}" , content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }
            return RedirectToAction("ProfileView", "AccountControllers", new { Id = id });

        }

        public IActionResult ChangePassword()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword user, string id)
        {


            using (var httpClient = new HttpClient())
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync($"https://localhost:7250/api/Account/change?Id=" + id, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //blog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                }
            }
            return RedirectToAction("ProfileView", "AccountControllers", new { Id = id });

        }

    }
}