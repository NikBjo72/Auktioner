using System;
using Xunit;
using GG.Models;
using GG.CustomValidation;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace GG.Test
{
    public class GGUnitTests
    {
        [Fact]
        public void GenerateNewAuctionItemId()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);

            bool auctionIdOk = false;
            bool expectedAuctionIdOk = true;

            //Act
            string newAuctionItemId = auctionItemRepository.GenerateAuctionItemId();
            bool idExisting = auctionItemRepository.AllAuctionItems.Any(obj => obj.AuctionItemId == newAuctionItemId);
            int amountDigits = newAuctionItemId.Count(Char.IsDigit);
            int amountLetters = newAuctionItemId.Count(Char.IsLetter);
            if(!idExisting && amountDigits == 6 && amountLetters == 3){
                auctionIdOk = true;
            }
            
            //Assert
            Assert.Equal(auctionIdOk, expectedAuctionIdOk);
        }

        [Fact]
        public void CheckIfAuctionItemIdExists()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);

            bool expectedABC123456 = true;
            bool notExpectedBBB987654 = false;

            //Act
            bool ABC123456 = auctionItemRepository.CheckIfAuctionItemIdExists("ABC123456");
            bool BBB987654 = auctionItemRepository.CheckIfAuctionItemIdExists("BBB987654");

            //Assert
            Assert.Equal(expectedABC123456, ABC123456);
            Assert.Equal(notExpectedBBB987654, BBB987654);
        }

        [Fact]
        public void AddNewAuctionItem()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);

            int expectedAmountOfObjects = auctionItemRepository.AllAuctionItems.Count() + 1;
            bool expetedObjectAdded = true;

            //Act
            bool objectAdded = auctionItemRepository.AddAuctionItem(SetTestAuctionItem());
            appDbContext.SaveChanges();

            int amountOfObjects = auctionItemRepository.AllAuctionItems.Count();

            //Assert
            Assert.Equal(expectedAmountOfObjects, amountOfObjects);
            Assert.Equal(expetedObjectAdded, objectAdded);
        }

        [Fact]
        public void StartingPriceNotLowerThanPurchaseCost()
        {
            // arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext); 
            bool expectedPurchaseCostHigher = true;

            // act
            bool purchaseCostHigher = auctionItemRepository.CompareInAndOutPrice(SetTestAuctionItem());

            // assert              
            Assert.Equal(expectedPurchaseCostHigher, purchaseCostHigher);
        }

        [Fact]
        public void SoldAutomaticlyTrueWhenSalePriceSet()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);
            bool expectedSold = true;
            //Act
            AuctionItem retrievedItem = auctionItemRepository.GetAuctionItemFromId("ABC123456");
            retrievedItem.SalePrice = 599;
            bool sold = auctionItemRepository.IfSalePriceSetSoldToTrue(retrievedItem);

            //Assert
            Assert.Equal(expectedSold, sold);
        }

        [Fact]
        public void SetAndGetTagOnAuctionItem()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);
            string expectedHashtagString = "nyhet";

            bool objectAdded = auctionItemRepository.AddAuctionItem(SetTestAuctionItem());
            appDbContext.SaveChanges();
            
            //Act
            auctionItemRepository.SetTagWithItemId("FPB397635", "nyhet");
            List <string> hashTagList = auctionItemRepository.WithIdGetAllTagsOnAuctionItemToList("FPB397635");
            int index = hashTagList.Count() - 1;

            //Assert
            Assert.Equal(expectedHashtagString, hashTagList[index]);

        }

        [Fact]
        public void GetAllAuctionItemsWithSpecifiedTag()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);

            bool objectAdded = auctionItemRepository.AddAuctionItem(SetTestAuctionItem());
            appDbContext.SaveChanges();

            string expectedAuctionItemId = "FPB397635";
            
            //Act
            IEnumerable<AuctionItem> retrievedAuctionItemList= auctionItemRepository.GetListOfAuctionItemsWithTag("test");

            //Assert
            Assert.Equal(expectedAuctionItemId, retrievedAuctionItemList.ElementAt(0).AuctionItemId);
        }

        [Fact]
        public void RemoveTagFromAuctionItem()
        {
            //Arrange
            var appDbContext = CreateTestServer();
            AuctionItemRepository auctionItemRepository = new (appDbContext);

            //string newID = auctionItemRepository.GenerateAuctionItemId();

            bool objectAdded = auctionItemRepository.AddAuctionItem(SetTestAuctionItem());
            appDbContext.SaveChanges();

            bool expectedRemoveAnswer = true;
            
            //Act
            bool removeAnswer = auctionItemRepository.RemoveHashTag("test", "FPB397635");

            //Assert
            Assert.Equal(expectedRemoveAnswer, removeAnswer);  
        }
        
        // *** Metoder för DRY i tester ***
        public AppDbContext CreateTestServer()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("test").Options;
            var appDbContext = new AppDbContext(options);
            return appDbContext;
        }
        public AuctionItem SetTestAuctionItem()
        {
           AuctionItem newItem = new AuctionItem
            {
                AuctionItemId = "FPB397635",
                Definition = "TagTestItem",
                Description = "Beskrivning av TagTestItem",
                PurchaseCost = 299,
                StartingPrice = 350,
                Tags = "test,hej,hoppsan,jul"
            };
            return newItem; 
        }
    }
}
