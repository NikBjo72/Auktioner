using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GG.Models;
using GG.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace GG.Controllers
{
    public class AuctionItemController : Controller
    {
        private readonly IAuctionItemRepository auctionItemRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AuctionItemController(IAuctionItemRepository auctionItemRepository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            this.auctionItemRepository = auctionItemRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.webHostEnvironment = webHostEnvironment;

        }


        [Authorize(Roles = "Purchaser")]
        public IActionResult CreateNewAuctionItem()
        {  
            ViewBag.sideHeader = "Skapa nytt auktionsobjekt";
            return View();
        }


        [Authorize(Roles = "Purchaser")]
        [HttpPost]
        public IActionResult CreateNewAuctionItem(CreateAuctionItemViewModel model)
        {
            model.UserId = userManager.GetUserId(User);

            char[] decadeArray = model.Decade.ToString().ToCharArray(); // *** Kan göras till CustomValidation ***
            int numberOfChars = decadeArray.Count();

            if (numberOfChars != 0 && decadeArray[numberOfChars-1] != '0')
            {
                ViewBag.notAZeroMessage = "Ditt årtionde behöver sluta med en nolla (0).";
                return View(model);
            }

            if (model.AuctionItemId == null)
            {
               string newAuctionItemId = this.auctionItemRepository.GenerateAuctionItemId();
               model.AuctionItemId = newAuctionItemId;
            }
            else
            {
                try
                {
                    this.auctionItemRepository.CeckIfAuctionItemIdIsValid(model.AuctionItemId);
                }
                catch(Exception ex)
                {
                    ViewBag.idNotOk = ex.Message;
                    return View(model);
                } 
                bool exists = this.auctionItemRepository.CheckIfAuctionItemIdExists(model.AuctionItemId);   
                if (exists)
                {
                    ViewBag.auctionItenIdExistsMessage = "Id:t på auktionsobjektet finns redan på ett annat objekt.";
                    return View(model);
                }
            } 

            if (ModelState.IsValid)
            {

                AuctionItem createdAuctionItem = new AuctionItem()
                {
                    AuctionItemId = model.AuctionItemId,
                    Definition = model.Definition,
                    Decade = model.Decade,
                    PurchaseCost = model.PurchaseCost,
                    StartingPrice = model.StartingPrice,
                    Description = model.Description,
                    Tags = model.Tags,
                    UserId = model.UserId
                };

                string uniqueFileName = null;
                if (model.Image != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    createdAuctionItem.ImageUrl = "~/img/"+ uniqueFileName;
                }

                this.auctionItemRepository.CreateNewAuctionItem(createdAuctionItem);
                return RedirectToAction("NewAuctionItemCreated");  
            }

            return View(model);
        }

        [Authorize(Roles = "Auctioneer")]
        public IActionResult EditAuctionItem(string id)
        {
            AuctionItem retrievedAuctionItem = this.auctionItemRepository.GetAuctionItemFromId(id);
            ViewBag.sideHeader = "Ändra auktionsobjekt";
            var editAuctionItemModel = new EditAuctionItemViewModel()
            {
                AuctionItemId = retrievedAuctionItem.AuctionItemId,
                Definition = retrievedAuctionItem.Definition,
                Decade = retrievedAuctionItem.Decade,
                PurchaseCost = retrievedAuctionItem.PurchaseCost,
                StartingPrice = retrievedAuctionItem.StartingPrice,
                SalePrice= retrievedAuctionItem.SalePrice,
                Sold = retrievedAuctionItem.Sold,
                Delivered = retrievedAuctionItem.Delivered,
                Description = retrievedAuctionItem.Description,
                Tags = retrievedAuctionItem.Tags,
                ImageUrl = retrievedAuctionItem.ImageUrl
            };

            return View(editAuctionItemModel);
        }   

        [Authorize(Roles = "Auctioneer")]
        [HttpPost]
        public IActionResult EditAuctionItem(EditAuctionItemViewModel editAuctionItemViewModel)
        {
            char[] decadeArray = editAuctionItemViewModel.Decade.ToString().ToCharArray(); // *** Kan göras till CustomValidation ***
            int numberOfChars = decadeArray.Count();

            if (!editAuctionItemViewModel.Sold && editAuctionItemViewModel.Delivered)
            {
                ViewBag.mustSetToMessage = "Objektet behöver vara sålt innan det kan levereras.";
                ViewBag.sideHeader = "Ändra auktionsobjekt";
                return View(editAuctionItemViewModel);
            }   

            if (ModelState.IsValid)
            {
                AuctionItem editedAuctionItem = auctionItemRepository.GetAuctionItemFromId(editAuctionItemViewModel.AuctionItemId);
                editedAuctionItem.AuctionItemId = editAuctionItemViewModel.AuctionItemId;
                editedAuctionItem.SalePrice= editAuctionItemViewModel.SalePrice;
                editedAuctionItem.Sold = editAuctionItemViewModel.Sold;
                editedAuctionItem.Delivered = editAuctionItemViewModel.Delivered;
                editedAuctionItem.Description = editAuctionItemViewModel.Description;
                editedAuctionItem.Tags = editAuctionItemViewModel.Tags;
                editedAuctionItem.ImageUrl = editAuctionItemViewModel.ImageUrl;

                string uniqueFileName = null;
                if (editAuctionItemViewModel.Image != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + editAuctionItemViewModel.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    editAuctionItemViewModel.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    editedAuctionItem.ImageUrl = "~/img/"+ uniqueFileName;
                }

                this.auctionItemRepository.EditAuctionItem(editedAuctionItem);
                return RedirectToAction("NewAuctionItemCreated");  
            }

            return View(editAuctionItemViewModel);
        }

        public IActionResult NewAuctionItemCreated()
        {                
            ViewBag.answerMessage = "Ditt auktionsobjekt är sparat!";
            return View();
        }

        public async Task <ViewResult> ListAuctionItems(string id)
        {
            IEnumerable<AuctionItem> auctionItems;
            string currentTag;
            string currentUserId = userManager.GetUserId(User);
            var user = await userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(id))
            {   
                if (currentUserId != null)
                {
                    IList<string> roles = await userManager.GetRolesAsync(user);
                    if(roles.Any(r => r == "Auctioneer"))
                    {
                        auctionItems = this.auctionItemRepository.AllAuctionItems
                        .OrderBy(i => i.Decade);
                        currentTag = "Alla auktionsobjekt";
                    }
                    else
                    {
                        auctionItems = this.auctionItemRepository.AllAuctionItems
                        .Where(i => i.UserId == currentUserId)
                        .OrderBy(i => i.Decade);
                        currentTag = "Alla auktionsobjekt";
                    }
                }
                else
                {
                    auctionItems = this.auctionItemRepository.AllAuctionItems
                    .OrderBy(i => i.Decade);
                    currentTag = "Alla auktionsobjekt";
                }
            }

            else
            {
                if (currentUserId != null)
                {
                    IList<string> roles = await userManager.GetRolesAsync(user);
                    if(roles.Any(r => r == "Auctioneer"))
                    {
                        auctionItems = this.auctionItemRepository.AllAuctionItems
                        .Where(i => i.AuctionItemTags.Any(t => t == id) == true)
                        .OrderBy(i => i.Decade);
                        currentTag = $"Listade efter kategori: {id}";
                    }
                    else
                    {
                        auctionItems = this.auctionItemRepository.AllAuctionItems
                        .Where(i => i.UserId == currentUserId)
                        .Where(i => i.AuctionItemTags.Any(t => t == id) == true)
                        .OrderBy(i => i.Decade);
                        currentTag = $"Listade efter kategori: {id}";
                    }
                }
                else
                {
                        auctionItems = this.auctionItemRepository.AllAuctionItems
                        .Where(i => i.AuctionItemTags.Any(t => t == id) == true)
                        .OrderBy(i => i.Decade);
                        currentTag = $"Listade efter kategori: {id}";
                }
            }
            
            return View(new AuctionItemListViewModel
            {
                AuctionItems = auctionItems,
                CurrentTag = currentTag
            });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
