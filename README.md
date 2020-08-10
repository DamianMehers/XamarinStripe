# A Xamarin Stripe example

Watch this video to see this example in action:

[<img src="https://img.youtube.com/vi/Gd-GjT0_A4M/maxresdefault.jpg" width="50%">](https://youtu.be/Gd-GjT0_A4M)

The video shows using Stripe's API from a Xamarin app that runs on iOS, Android and Windows, with all the code, including the UI code being shared across all three platforms.

## Running the example
Clone this repository, open it in Visual Studio and update the [Config.cs](XamarinStripe.Forms/Services/Config.cs) file to enter your own values.  To get these values I recommend following the instructions Stripe provide for their [iOS](https://stripe.com/docs/mobile/ios/basic#setup-ios) and [Android](https://stripe.com/docs/mobile/android/basic) example apps ([register with Stripe](https://dashboard.stripe.com/register) and one-click deploy their [example server to Heroku](https://github.com/stripe/example-mobile-backend)).

## The top level structure
If you are familiar with Xamarin then the code structure will be familiar.
At the top level there are projects for each of the three supported platforms ([iOS](XamarinStripe.iOS), [Android](XamarinStripe.Android) and [Windows](XamarinStripe.UWP), as well as a project for the code that is shared between them, [XamarinStripe.Forms](XamarinStripe.Forms)

I didn't write any code in the platform-specific projects for this example.

## Inside the [shared code](XamarinStripe.Forms)
Here you will find folders for:
* [Models](XamarinStripe.Forms/Models): Not much here since I've reused the models from the Stripe SDK where possibe
* [ViewModels](XamarinStripe.Forms/ViewModels): These wrap the models and expose functionality that is consumed by the views, making use of the Services to do so.  They have no "awareness" of the views.
* [Views](XamarinStripe.Forms/Views):  These bind to the ViewModels and are defined almost entirely in Xamarin Forms XAML, with some code to do animations.  The XAML gets rendered as native UI elements, so an iOS button will look like an iOS button and an Android text entry field will look like an Android field.
* [Services](XamarinStripe.Forms/Services): These encapsulate discrete functionality such as credit card verification, Product storage (check out the [ProductsDataStoreService](XamarinStripe.Forms/Services/ProductsDataStoreService.cs) for the Emojis that are for sale), and [communication](XamarinStripe.Forms/Services/EphemeralService.cs) with the Heroku based service that we use to not store secrets in the app.

## Pull requests welcome!
I implemented some, but by no means all of the functionality from Stripe's iOS and Android examples.  For example I didn't do 3D Secure, Apple Pay, not did I set up the Settings page.  None of these are hard, but I spent a weekend doing this first example and I've run out of time.  Please don't hesitate to contribute.

Also, I am not a Stripe expert.  To be honest I had never used the API before putting this example together, so feel free to let me know if I've made mistakes.

Don't hesitate to use this example code, but **you are responsible** for verifying that it is correct.

## Calling the Stripe API from .NET

If all you are about is how to call the Stripe API from C#, then you'll want to check out:
* [EphemeralService.cs](XamarinStripe.Forms/Services/EphemeralService.cs) which encapsulates talking to the intermediary Heroku service to do things get a Stripe [ephemeral key](https://stripe.dev/stripe-android/com/stripe/android/EphemeralKey.html) and create a new payment intent;
* [PaymentOptionsViewModel](XamarinStripe.Forms/ViewModels/PaymentOptionsViewModel.cs) which lists the payment methods available for a customer
* [AddCardViewModel](XamarinStripe.Forms/ViewModels/AddCardViewModel.cs) which creates a payment method and attaches it to a customer
* [ShippingAddressViewModel](XamarinStripe.Forms/ViewModels/ShippingAddressViewModel.cs) which adds a shipping address to a customer
* [CheckoutViewModel](XamarinStripe.Forms/ViewModels/CheckoutViewModel.cs) which creates a payment intent and confirms it.


## Initial author

This example was originally created by [Damian Mehers](https://damian.fyi/)
