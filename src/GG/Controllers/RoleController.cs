using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GG.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GG.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        
        [Authorize]
        public async Task <IActionResult> ChangeRole()
        {   
            var user = await this.userManager.GetUserAsync(User);
            IList<string> roles = await this.userManager.GetRolesAsync(user);

            if (roles.Count() != 0)
            {
                if (roles.Any(r => r == "Purchaser"))
                {
                    await this.userManager.RemoveFromRoleAsync(user, "Purchaser");
                    await this.userManager.AddToRoleAsync(user, "Auctioneer");
                    await this.signInManager.SignInAsync(user, isPersistent: false);
                }
                else if (roles.Any(r => r == "Auctioneer"))
                {
                    await this.userManager.AddToRoleAsync(user, "Purchaser");
                    await this.userManager.RemoveFromRoleAsync(user, "Auctioneer");
                    await this.signInManager.SignInAsync(user, isPersistent: false);
                }
            }
            if (roles.Count() == 0)
            {
                await this.userManager.AddToRoleAsync(user, "Purchaser");
                await this.signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("InRole", "Role");
        }

        public async Task <IActionResult> InRole()
        {
            var user = await this.userManager.GetUserAsync(User);
            IList<string> roles = await this.userManager.GetRolesAsync(user);
            if (roles[0].ToString() == "Auctioneer")
            {
                ViewBag.role = "Auktionsutropare";
            }
            else
            {
                ViewBag.role = "Inköpare";
            }
            return View();
        }

    }
}
