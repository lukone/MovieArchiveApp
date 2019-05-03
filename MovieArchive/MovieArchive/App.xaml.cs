using DLToolkit.Forms.Controls;
using MovieArchive.Resources;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MovieArchive
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            AppResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
            FlowListView.Init();
            MainPage = new Home();
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            try
            {
                var DE = new DataExchange();
                await DE.UpdateDataFromWebApi();
            }
            catch (Exception e) { throw e; };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
