using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GG.Models
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext appDbContext;

        public AppUserRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IEnumerable<AppUser> AllAppUsers => appDbContext.AppUsers;
    }

}    