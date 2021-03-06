using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GG.Models
{
    public class AuctionItemRepository : IAuctionItemRepository
    {
        private readonly AppDbContext appDbContext;

        public AuctionItemRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IEnumerable<AuctionItem> AllAuctionItems
        {
            get
            {
                return SetTagstringToTagList(appDbContext.AuctionItems);
            }
        }

        public IEnumerable<AuctionItem> SetTagstringToTagList(IEnumerable<AuctionItem> auctionItemList)
        {
            foreach (var item in auctionItemList)
            {
                item.AuctionItemTags = TagStringToList(item.Tags);
            }
            return auctionItemList;
        }

        public void CreateNewAuctionItem(AuctionItem auctionItem)
        {
            IfSalePriceSetSoldToTrue(auctionItem);
            auctionItem.AuctionItemTags = TagStringToList(auctionItem.Tags);
            appDbContext.AuctionItems.Add(auctionItem);
            appDbContext.SaveChanges();
        }
        public void EditAuctionItem(AuctionItem auctionItem)
        {
            IfSalePriceSetSoldToTrue(auctionItem);
            auctionItem.AuctionItemTags = TagStringToList(auctionItem.Tags);
            appDbContext.AuctionItems.Update(auctionItem);
            appDbContext.SaveChanges();
        }
        public bool CeckIfAuctionItemIdIsValid(string auctionItemId)
        {
            bool letter = false;
            bool number = false;

            char[] auctionItemIdArray = auctionItemId.ToCharArray();
            var letterArray = auctionItemIdArray.Take(3);
            var numnberArray = auctionItemIdArray.Skip(3);

            if (letterArray.Count() == 3 && letterArray.All(l => Char.IsLetter(l) == true))
            {
                letter = true;
            }
            if (numnberArray.Count() == 6 && numnberArray.All(l => Char.IsDigit(l) == true))
            {
                number = true;
            }

            if (letter && number)
            {
                return true;
            }
            throw new Exception("Inte till??tet id!"); // *** Anv??nder throw new Exeption f??r validering ***
        }

        public string GenerateAuctionItemId(int exists = 0) //*** Genererar AuctionItemId ***
        {
            string auctionItemId ="";
            int letterInt = 0;

            int count = AllAuctionItems.Count() + exists;
            Random rndLetter = new Random(count);
            for(int j = 0; j < 3; j++)
            {
                var letter = (char)rndLetter.Next('a','z');
                auctionItemId += letter.ToString();
                letterInt += letter;
            }
            Random rndNumber = new Random(letterInt + AllAuctionItems.Count());
            var Number = rndNumber.Next(0,999999);
            auctionItemId += Number.ToString("000000");
            auctionItemId = auctionItemId.ToUpper();

            return auctionItemId;
        }

        public bool CheckIfAuctionItemIdExists(string auctionItemId) //*** Kollar s?? att AuctionItemId inte redan finns ***
        {
            return this.appDbContext.AuctionItems.Any(obj => obj.AuctionItemId == auctionItemId);
        }

        public bool AddAuctionItem(AuctionItem item) //*** L??gger till AuctionItem om Id:t inte redan finns ***
        {
            if(!CheckIfAuctionItemIdExists(item.AuctionItemId))
            {
            this.appDbContext.Add<AuctionItem> (item);
            return true;
            }
            else return false;
        }

        public bool CompareInAndOutPrice(AuctionItem item) //*** Kollar s?? att Inpriset p?? ett AuctionTtem inte ??r h??gre ??n StartingPrice ***
        {
            if (item.PurchaseCost <= item.StartingPrice)
            {
                return true;
            } else return false;
        }

        public AuctionItem GetAuctionItemFromId(string auctionItemId) //*** H??mtar auktionsobjekt med id ***
        {
            return this.appDbContext.AuctionItems.FirstOrDefault(i => i.AuctionItemId == auctionItemId);
        }

        public bool IfSalePriceSetSoldToTrue(AuctionItem auctionItem) //*** S??tter auktionsobjektet till s??lt om SalePrise s??tts ***
        {
            if (auctionItem.SalePrice > 0)
            {
                auctionItem.Sold = true;
                return true;
            }
            else return false;
        }

        public void SetTagWithItemId(string auctionItemId, string tag) //*** L??gger till tag p?? auktionsobject ***
        {
            AuctionItem retrievedItem = GetAuctionItemFromId(auctionItemId);

            tag = String.Concat(tag.Where(c => !Char.IsWhiteSpace(c)));
            if (retrievedItem.Tags != "")
            {
                retrievedItem.Tags += retrievedItem.Tags + "," + tag;
                retrievedItem.AuctionItemTags = WithIdGetAllTagsOnAuctionItemToList(auctionItemId);
            }
            else
                retrievedItem.Tags = tag;
        }

        public List <string> WithIdGetAllTagsOnAuctionItemToList(string auctionItemId) //*** G??r tagstr??ng till en lista med alla tags f??r AuctionItem med medskickat Id ***
        {
            AuctionItem retrievedItem = GetAuctionItemFromId(auctionItemId);
            string[] tags = retrievedItem.Tags.Split(',');
            List <string> tagList = tags.ToList();
            return tagList;
        }

        public IEnumerable<AuctionItem> GetListOfAuctionItemsWithTag(string hashTag) //*** G??r en lista av alla AuctionItem som inneh??ller specifik medskickad tag ***
        {
            IEnumerable<AuctionItem> retrievedAuctionItems = AllAuctionItems.Where(a => a.Tags.Split(',').ToList().Any(tag => tag == hashTag));
            return retrievedAuctionItems;
        }

        public bool RemoveHashTag(string tag, string auctionItemId) //*** Tar bort en tag utifr??m AuctionItemId och specificerad tag ***
        {
            List <string> hashTagList = WithIdGetAllTagsOnAuctionItemToList(auctionItemId);
            if (hashTagList.IndexOf(tag) != -1)
            {
                hashTagList.RemoveAt(hashTagList.IndexOf(tag));
                var newHashTagString = String.Join(",", hashTagList.ToArray());
                GetAuctionItemFromId(auctionItemId).Tags = newHashTagString;
                return true;
            }
            return false;
        }

        public List <string> TagStringToList(string tagString) //*** G??r hashtagstr??ng till en lista med alla tags f??r AuctionItem med medskickat Id ***
        {
            string[] tags = tagString.Split(',');
            List <string> tagList = tags.ToList();
            return tagList;
        }
    }
}
