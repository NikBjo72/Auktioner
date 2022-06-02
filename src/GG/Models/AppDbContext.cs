using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GG.Models
{
public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }
        public DbSet<AuctionItem> AuctionItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //seed roles
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Purchaser",
                NormalizedName = "PURCHASER" 
            });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Auctioneer",
                NormalizedName = "AUCTIONEER"
            });

            //seed categories
            modelBuilder.Entity<AuctionItem>().HasData(new AuctionItem
            {
                AuctionItemId = "ABC123456",
                Definition = "Marc L. Felix - MANIEMA - 1989",
                Description = "MANIEMA - “An Essay on the Distribution of the Symbols and Myths as Depicted in the Masks of Greater Maniema”",
                PurchaseCost = 250,
                StartingPrice = 399,
                Decade = 1980,
                Tags = "bok,samlarobjekt",
                ImageUrl = "~/img/Maniema_250x313px.png"
            });
            modelBuilder.Entity<AuctionItem>().HasData(new AuctionItem
            {
                AuctionItemId = "GHJ789456",
                Definition = "Marcel Duchamp - Eau & Gaz - A Tous Les Étages - 2015",
                Description = "Super rare book ‘Eau & Gaz - A Tous Les Étages’ by Marcel Duchamp.",
                PurchaseCost = 350,
                StartingPrice = 499,
                Decade = 2000,
                Tags = "bok,samlarobjekt",
                ImageUrl = "~/img/Eau & Gaz_250x313px.png"
            });
            modelBuilder.Entity<AuctionItem>().HasData(new AuctionItem
            {
                AuctionItemId = "GPD946298",
                Definition = "Tucher und Walther - Stor brandkår 25 cm mit Federwerk und 2 Leitern - Tyskland",
                Description = "Very nice fire brigade with spring mechanism from Tucher and Walter with 2 ladders, 3 hose reels and a key for winding - in very good condition.",
                PurchaseCost = 450,
                StartingPrice = 800,
                Decade = 1860,
                Tags = "leksak, dekoration",
                ImageUrl = "~/img/Stor brandkår_250x187px.png"
            });
        }

    }    
}