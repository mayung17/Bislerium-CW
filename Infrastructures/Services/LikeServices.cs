using Application.Interfaces;
using Azure;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Services
{
    public class LikeServices : ILike
    {
        private readonly ApplicationDbContext _context;

        public LikeServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Like> GetUsersLike(Guid id, string u_id)
        {
            var res = await _context.Likes.FirstOrDefaultAsync(x => x.Blog == id && x.User == u_id);
            return res;

        }



        public async Task<Like> AddDownVote(Like like)
        {
            if (like.ReactionType == false)
            {
                BlogServices blog = new BlogServices(_context);


                var lik_pre = await GetUsersLike(like.Blog, like.User);
                var blogs = await blog.GetBlogById(like.Blog);


                if (lik_pre != null)
                {
                    if (lik_pre.ReactionType == false)
                    {
                        var likes = await _context.Likes.FirstOrDefaultAsync(x => x.Id == lik_pre.Id);
                        if (likes != null)
                        {

                            _context.Likes.Remove(likes);
                            await _context.SaveChangesAsync();
                        }
                        blogs.DislikeCount -= 1;
                        blogs.Popularity = (2 * blogs.LikeCount) + (-1 * blogs.DislikeCount) + (1 * blogs.CommentCount);

                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    _context.Likes.Add(like);
                    blogs.DislikeCount += 1;
                    blogs.Popularity = (2 * blogs.LikeCount) + (-1 * blogs.DislikeCount) + (1 * blogs.CommentCount);


                }


                //await _context.Likes.AddAsync(like);
                _context.Blogs.Update(blogs);
                await _context.SaveChangesAsync();
                return like;
            }
            else
            {
                return null;
            }

        }

        public async Task<Like> AddUpvote(Like like)

        {
            if (like.ReactionType == true)
            {
                BlogServices blog = new BlogServices(_context);


                var lik_pre = await GetUsersLike(like.Blog, like.User);
                var blogs = await blog.GetBlogById(like.Blog);

                

                if (lik_pre != null)
                {
                    if (lik_pre.ReactionType ==true)
                    {
                        var likes = await _context.Likes.FirstOrDefaultAsync(x => x.Id == lik_pre.Id);
                        if (likes != null)
                        {

                            _context.Likes.Remove(likes);
                            await _context.SaveChangesAsync();
                        }
                        blogs.LikeCount -= 1;
                        blogs.Popularity = (2 * blogs.LikeCount) + (-1 * blogs.DislikeCount) + (1 * blogs.CommentCount);
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    _context.Likes.Add(like);
                    blogs.LikeCount += 1;
                    blogs.Popularity = (2 * blogs.LikeCount) + (-1 * blogs.DislikeCount) + (1 * blogs.CommentCount);


                }

                //await _context.Likes.AddAsync(like);
                _context.Blogs.Update(blogs);
                await _context.SaveChangesAsync();
                return like;
            }
            else
            {
                return null;
            }

        }

        public async Task DeleteVote(Guid id)
        {
            var likes = await _context.Likes.FirstOrDefaultAsync(x => x.Id == id);
            if (likes != null)
            {

                _context.Likes.Remove(likes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
