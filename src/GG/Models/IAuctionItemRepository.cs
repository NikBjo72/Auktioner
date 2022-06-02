using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace GG.Models
{
    public interface IAuctionItemRepository
    {
        IEnumerable<AuctionItem> AllAuctionItems { get; }
        string GenerateAuctionItemId(int exists = 0);
        void CreateNewAuctionItem(AuctionItem auctionItem);
        bool CheckIfAuctionItemIdExists(string auctionItemId);
        bool CompareInAndOutPrice(AuctionItem item);
        AuctionItem GetAuctionItemFromId(string auctionItemId);
        void EditAuctionItem(AuctionItem auctionItem);
        bool CeckIfAuctionItemIdIsValid(string auctionItemId);
    }
}
