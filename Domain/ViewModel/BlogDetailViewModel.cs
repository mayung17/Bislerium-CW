using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class BlogDetailViewModel
    {
        public Blog Blog { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
