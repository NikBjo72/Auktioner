
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using GG.Models;
using GG.ViewModels;

namespace GG.CustomValidation  
    {  
        public class CompareInOutPrice : ValidationAttribute // *** Validering för inköpspriset ***
        {  
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
            {  
                var auctionItem = (CreateAuctionItemViewModel)validationContext.ObjectInstance;
      
                if (auctionItem.PurchaseCost == 0.0m)
                {
                    return new ValidationResult("Inköpspris måste anges vid registrering.");
                }
                else if (auctionItem.PurchaseCost > auctionItem.StartingPrice)
                {
                    return new ValidationResult("Startpriset behöver vara större än inköpspriset.");
                } 
                return ValidationResult.Success;
            }  
        }  
    }  
