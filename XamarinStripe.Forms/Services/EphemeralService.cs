using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stripe;
using XamarinStripe.Forms.Models;
using Product = XamarinStripe.Forms.Models.Product;

namespace XamarinStripe.Forms.Services {
  internal class EphemeralService {

    private readonly HttpClient _httpClient = new HttpClient();
    private readonly object _lock = new object();
    private EphemeralKeyAssociatedObject _customerAssociatedObject;
    private LocalEphemeralKey _ephemeralKey;
    private TaskCompletionSource<bool> _taskCompletionSource;

    private EphemeralService() { }

    public static EphemeralService Instance { get; } = new EphemeralService();

    public async Task<StripeClient> GetClient(bool usePublishedKey = false) {
      if (usePublishedKey) return new StripeClient(Config.PublishedKey);
      await Load();
      return new StripeClient(_ephemeralKey.Secret);
    }

    public async Task<string> GetCustomerId() {
      await Load();
      return _customerAssociatedObject.Id;
    }

    public async Task<(string secret, string intent)> CreatePaymentIntent(List<Product> products,
      ShippingAddress address,
      PaymentMethod method) {
      var url = $"{Config.BaseUrl}/{Paths.CreatePaymentIntent}";

      var parameters = new Dictionary<string, object> {
        {
          // example-mobile-backend allows passing metadata through to Stripe
          "metadata", new Dictionary<string, string> {{"payment_request_id", "B3E611D1-5FA1-4410-9CEC-00958A5126CB"}}
        },
        {"products", products.Select(p => p.Emoji).ToList()},
        {"shipping", method.Id},
        {"country", address.Country}
      };

      var json = JsonConvert.SerializeObject(parameters);

      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var response = await _httpClient.PostAsync(url, content);
      var responseText = await response.Content.ReadAsStringAsync();

      if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"{response.ReasonPhrase} ({responseText})");

      var createPaymentIntentResponse = JsonConvert.DeserializeObject<CreatePaymentIntentResponse>(responseText);
      return (createPaymentIntentResponse.Secret, createPaymentIntentResponse.Intent);
    }

    private async Task Load() {

      if (Config.PublishedKey.StartsWith("TODO") || Config.BaseUrl.StartsWith("TODO")) {
        throw new Exception($"Please enter your public key and server url in  {typeof(Config).Namespace + "." + nameof(Config)}");
      }

      if (_ephemeralKey != null) return;
      TaskCompletionSource<bool> tcs;
      var wait = true;
      lock (_lock) {
        if (_ephemeralKey != null) return;

        if (_taskCompletionSource != null) {
          tcs = _taskCompletionSource;
        }
        else {
          tcs = _taskCompletionSource = new TaskCompletionSource<bool>();
          wait = false;
        }
      }

      if (wait) { // Already being fetched
        await tcs.Task;
        return;
      }

      try {
        var url = $"{Config.BaseUrl}/{Paths.EphemeralKeys}?api_version={StripeConfiguration.ApiVersion}";
        var response = await _httpClient.PostAsync(url, null);
        var content = await response.Content.ReadAsStringAsync();

        _ephemeralKey = JsonConvert.DeserializeObject<LocalEphemeralKey>(content);

        _customerAssociatedObject = _ephemeralKey.AssociatedObjects.Single(ao => ao.Type == "customer");


        tcs.SetResult(true);
      }
      catch (Exception ex) {
        tcs.SetException(ex);
        throw;
      }
    }

    private class Paths {
      //internal const string CreateSetupIntent = "create_setup_intent";
      internal const string CreatePaymentIntent = "create_payment_intent";
      //internal const string ConfirmPaymentIntent = "confirm_payment_intent";
      internal const string EphemeralKeys = "ephemeral_keys";
    }

    private class CreatePaymentIntentResponse {
      [JsonProperty("secret")] public string Secret { get; set; }

      [JsonProperty("status")] public string Status { get; set; }

      [JsonProperty("intent")] public string Intent { get; set; }
    }

    public class LocalEphemeralKey : EphemeralKey {
      [JsonProperty("secret")] public string Secret { get; set; }
    }
  }
}