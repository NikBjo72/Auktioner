using System.Collections.Generic;

namespace GG.Models
{
    public interface IAppUserRepository
    {
        IEnumerable<AppUser> AllAppUsers { get; }
    }
}