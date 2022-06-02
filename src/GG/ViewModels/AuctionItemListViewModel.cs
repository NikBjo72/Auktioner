using System.Collections.Generic;
using System.Linq;
using GG.Models;

namespace GG.ViewModels
{
    public class AuctionItemListViewModel
    {
        public IEnumerable<AuctionItem> AuctionItems { get; set; }
        public string CurrentTag { get; set; }

    }

    
}