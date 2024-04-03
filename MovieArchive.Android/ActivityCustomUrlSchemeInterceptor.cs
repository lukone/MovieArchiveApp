using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MovieArchive.ViewModels;
using System;
using Xamarin.Auth;

namespace MovieArchive.Droid
{
    [Activity(Label = "ActivityCustomUrlSchemeInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataSchemes = new[] { "com.lk.MovieArchive1" }, DataPath = "/oauth2redirect")]
    public class ActivityCustomUrlSchemeInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            global::Android.Net.Uri uri_android = Intent.Data;
            CustomTabsConfiguration.CustomTabsClosingMessage = null;
            var uri = new Uri(Intent.Data.ToString());
            AuthenticatorHelper.OAuth2Authenticator.OnPageLoading(uri);
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
            this.Finish();
            return;
        }
    }

}