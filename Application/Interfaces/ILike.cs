using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILike
    {
        Task<Like> AddUpvote(Like like);
        Task<Like> AddDownVote( Like like);
        Task<Like> GetUsersLike(Guid id, string u_id);
        Task DeleteVote(Guid id);

    }
}
