using System.Collections.Generic;
using XamarinStripe.Forms.Models;

namespace XamarinStripe.Forms.Services {
  public class ProductsDataStoreService {
    private ProductsDataStoreService() { }

    private static List<Product> ProductsAndPrices { get; } = new List<Product> {
      new Product {Emoji = "👕", Price = 2000, Name = "T-Shirt"},
      new Product {Emoji = "👖", Price = 4000, Name = "Jeans"},
      new Product {Emoji = "👗", Price = 3000, Name = "Dress"},
      new Product {Emoji = "👞", Price = 700, Name = "Mans Shoe"},
      new Product {Emoji = "👟", Price = 600, Name = "Athletic Shoe"},
      new Product {Emoji = "👠", Price = 1000, Name = "High-Heeled Shoe"},
      new Product {Emoji = "👡", Price = 2000, Name = "Womans Sandal"},
      new Product {Emoji = "👢", Price = 2500, Name = "Womans Boots"},
      new Product {Emoji = "👒", Price = 800, Name = "Womans Hat"},
      new Product {Emoji = "👙", Price = 3000, Name = "Bikini"},
      new Product {Emoji = "💄", Price = 2000, Name = "Lipstick"},
      new Product {Emoji = "🎩", Price = 5000, Name = "Top Hat"},
      new Product {Emoji = "👛", Price = 5500, Name = "Purse"},
      new Product {Emoji = "👜", Price = 6000, Name = "Handbag"},
      new Product {Emoji = "🕶", Price = 2000, Name = "Dark Sunglasses"},
      new Product {Emoji = "👚", Price = 2500, Name = "Womans Clothes"}
    };

    public IEnumerable<Product> Items => ProductsAndPrices.ToArray();


    public static ProductsDataStoreService Instance { get; } = new ProductsDataStoreService();
  }
}