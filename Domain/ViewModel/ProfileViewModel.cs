using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ProfileViewModel
    {
        public IdentityUser User { get; set; }
        public List<Blog> AuthoredBlogPosts { get; set; }
    }

}
