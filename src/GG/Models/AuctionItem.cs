using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GG.CustomValidation;
using Microsoft.AspNetCore.Http;

namespace GG.Models
{
    public class AuctionItem
    {   
        [Display(Name = "Id för auktionsobjekt (lämna tomt för automatiskt Id)")]
        [StringLength(9)]
        public string AuctionItemId { get; set; }


        [Required(ErrorMessage = "Skriv in ett namn på auktionsobjektet.")]
        [Display(Name = "Objektbenämning (max 80 tecken)")]
        [StringLength(80)]
        public string Definition { get; set; }


        [Required(ErrorMessage = "Skriv in en beskrivning av auktionsobjektet.")]
        [Display(Name = "Beskrivning (max 250 tecken)")]
        [StringLength(250)]
        public string Description { get; set; }


        [Required(ErrorMessage = "Skriv in inköpskostnaden för auktionsobjektet.")]
        [Range(0, double.MaxValue, ErrorMessage = "Skriv bara beloppet med siffror")]
        [Display(Name = "Inköpskostnad")]
        public decimal? PurchaseCost { get; set; }


        [Display(Name = "Utropspris")]
        [Range(0, double.MaxValue, ErrorMessage = "Skriv bara beloppet med siffror")]
        [CompareInOutPrice]
        public decimal? StartingPrice { get; set; }

        
        [Display(Name = "Försäljningspris")]
        [Range(0, double.MaxValue, ErrorMessage = "Skriv bara beloppet med siffror")]
        public decimal? SalePrice { get; set; }


        [Display(Name = "Auktionsobjektet sålt")]
        public bool Sold { get; set; }


        [Display(Name = "Auktionsobjektet levererat")]
        public bool Delivered { get; set; }


        [Required(ErrorMessage = "Skriv in ett årtionde då auktionsobektet är tillverkat")]
        [Range(100, 9999, ErrorMessage = "Minst tre och max fyra siffor")]
        [Display(Name = "Årtionde för tillverkning")]
        public int? Decade { get; set; }


        [Required(ErrorMessage = "En kategori för användningsområde måste skrivas in")]
        [Display(Name = "Taggar för kategorisering")]
        public string Tags { get; set; }
        

        [DisplayName("Uppladdning av bildfil")]
        public string ImageUrl { get; set; }
        

        [NotMapped]
        public List <string> AuctionItemTags { get; set; }

        public string UserId { get; set; }

        public AuctionItem()
        {
            this.Tags = "";
            this.Sold = false;
            this.Delivered = false;
        }
    }
}
