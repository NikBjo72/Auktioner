using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GG.CustomValidation;
using GG.Models;
using Microsoft.AspNetCore.Http;

namespace GG.ViewModels
{
    public class EditAuctionItemViewModel
    {
        public string AuctionItemId { get; set; }
        public string Definition { get; set; }
        public int? Decade { get; set; }
        public decimal? PurchaseCost { get; set; }
        public decimal? StartingPrice { get; set; }


        [Display(Name = "Försäljningspris")]
        [Range(0, double.MaxValue, ErrorMessage = "Skriv bara beloppet med siffror")]
        public decimal? SalePrice { get; set; }


        [Display(Name = "Auktionsobjektet sålt")]
        public bool Sold { get; set; }


        [Display(Name = "Auktionsobjektet levererat")]
        public bool Delivered { get; set; }


        [Required(ErrorMessage = "Skriv in en beskrivning av auktionsobjektet.")]
        [Display(Name = "Beskrivning (max 250 tecken)")]
        [StringLength(250)]
        public string Description { get; set; }


        [Required(ErrorMessage = "En kategori för användningsområde måste skrivas in")]
        [Display(Name = "Taggar för kategorisering")]
        public string Tags { get; set; }


        [DisplayName("Bild")]
        public IFormFile Image { get; set; }

        public string ImageUrl { get; set; }
    }

    
}