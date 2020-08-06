using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace XamarinStripe.Forms.Services {
  internal class CardDefinitionService {
    private readonly CardDefinition[] _cardDefinitions;

    private readonly CardDefinition _error = new CardDefinition {
      Length = 16,
      Image = ImageService.Instance.CardError
    };

    private readonly CardDefinition _unknown = new CardDefinition {
      Length = 16,
      Image = ImageService.Instance.CardUnknown
    };

    private CardDefinitionService() {
      _cardDefinitions = new[] {
        new CardDefinition {
          Image = ImageService.Instance.Amex,
          Length = 15,
          Prefixes = {34, 37}
        },

        new CardDefinition {
          Image = ImageService.Instance.Diners,
          Length = 16,
          Prefixes = {30},
          Ranges = {(38, 39)}
        },
        new CardDefinition {
          Image = ImageService.Instance.Diners,
          Length = 14,
          Prefixes = {36}
        },


        new CardDefinition {
          Image = ImageService.Instance.Discover,
          Length = 16,
          Prefixes = {60},
          Ranges = {(64, 65)}
        },

        new CardDefinition {
          Image = ImageService.Instance.Jcb,
          Length = 16,
          Ranges = {(35, 36)}
        },

        new CardDefinition {
          Image = ImageService.Instance.Mastercard,
          Length = 16,
          Ranges = {(50, 59), (22, 27)},
          Prefixes = {67}
        },

        new CardDefinition {
          Image = ImageService.Instance.Unionpay,
          Length = 16,
          Prefixes = {62}
        },

        new CardDefinition {
          Image = ImageService.Instance.Visa,
          Length = 16,
          Ranges = {(40, 49)}
        },
        new CardDefinition {
          Image = ImageService.Instance.Visa,
          Length = 13,
          Ranges = {
            (413600, 413600),
            (444509, 444509),
            (444509, 444509),
            (444550, 444550),
            (450603, 450603),
            (450617, 450617),
            (450628, 450629),
            (450636, 450636),
            (450640, 450641),
            (450662, 450662),
            (463100, 463100),
            (476142, 476142),
            (476143, 476143),
            (492901, 492902),
            (492920, 492920),
            (492923, 492923),
            (492928, 492930),
            (492937, 492937),
            (492939, 492939),
            (492960, 492960)
          }
        }
      };
    }

    public static CardDefinitionService Instance { get; } = new CardDefinitionService();

    public (int length, ImageSource image) DetailsFor(string cardNumber) {
      var definition = DefinitionFor(cardNumber);

      return (definition.Length, definition.Image);
    }


    private CardDefinition DefinitionFor(string cardNumber) {
      if (cardNumber.Length == 0) return _unknown;

      var twoChars = cardNumber.Length >= 2 ? cardNumber.Substring(0, 2) : null;
      var fourChars = cardNumber.Length >= 4 ? cardNumber.Substring(0, 4) : null;


      var two = 0;
      var four = 0;


      if (twoChars != null) int.TryParse(twoChars, out two);


      if (fourChars != null) int.TryParse(fourChars, out four);

      var prefixMatches = new HashSet<CardDefinition>();

      foreach (var cardDefinition in _cardDefinitions) {
        if (cardDefinition.Prefixes.Contains(two) || cardDefinition.Prefixes.Contains(four)) return cardDefinition;

        foreach (var (lower, upper) in cardDefinition.Ranges) {
          if (two >= lower && two <= upper || four >= lower && four <= upper) return cardDefinition;

          if (lower.ToString().StartsWith(cardNumber)) prefixMatches.Add(cardDefinition);
        }

        if (cardDefinition.Prefixes.Any(p => p.ToString().StartsWith(cardNumber))) prefixMatches.Add(cardDefinition);
      }

      var groupedPrefixes = prefixMatches.GroupBy(p => p.Image).ToList();

      switch (groupedPrefixes.Count) {
        case 0:
          return _error;
        case 1:
          return groupedPrefixes[0].First();
        default:
          return _unknown;
      }
    }

    public ImageSource ImageForBrand(string cardBrand) {
      switch (cardBrand) {
        case "amex":
          return ImageService.Instance.Amex;
        case "diners":
          return ImageService.Instance.Diners;
        case "discover":
          return ImageService.Instance.Discover;
        case "jcb":
          return ImageService.Instance.Jcb;
        case "mastercard":
          return ImageService.Instance.Mastercard;
        case "unionpay":
          return ImageService.Instance.Unionpay;
        case "visa":
          return ImageService.Instance.Visa;
        default:
          return ImageService.Instance.CardUnknown;
      }
    }

    public string NameForBrand(string cardBrand) {
      if (cardBrand == "unionpay") return "UnionPay";

      return cardBrand[0].ToString().ToUpperInvariant() + cardBrand.Substring(1);
    }

    private class CardDefinition {
      public ImageSource Image { get; set; }
      public int Length { get; set; }

      public List<int> Prefixes { get; } = new List<int>();


      public List<(int lower, int upper)> Ranges { get; } = new List<(int lower, int upper)>();
    }
  }
}