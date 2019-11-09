using MovieArchive.Resources;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TvShowList : ContentPage
	{
        TvShowListModel TvShowListM = new TvShowListModel();
        int Seen = 0;  //Seen = 0 Not seen 1 Seen 

        public TvShowList()
	    {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await TvShowListM.LoadTvShow(Seen);
            Title = AppResources.TitlePageTvShowList + " " + (TvShowListM.TvShows==null ? "" : TvShowListM.TvShows.Count.ToString());
            TvShowListFl.FlowItemsSource = TvShowListM.TvShows;
        }

        private async void TvShowListFl_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Group is VisualElement visual))
                return;

            await visual.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
            await visual.ScaleTo(1, length: 100, easing: Easing.CubicInOut);

            var mi = (TvShow)((DLToolkit.Forms.Controls.FlowListView)sender).FlowLastTappedItem;

            //App.Current.MainPage = new Home { Detail = new NavigationPage(new MovieCard(mi)) };
            await Navigation.PushAsync(new TvShowCard(mi));
        }

        public void FlowScrollTo(object item)
        {
            TvShowListFl.FlowScrollTo(item, ScrollToPosition.MakeVisible, true);
        }

        private void SaveCSV_Activated(object sender, EventArgs e)
        {
            string File;
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageTvShowList + ".csv");
                        break;
                    case Device.Android:
                        File = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, AppResources.TitlePageTvShowList + ".csv");
                        break;
                    default:
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageTvShowList + ".csv");
                        break;
                }

                DataExchange.WriteCSV(TvShowListM.TvShows, File);
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageCvsFileExported);
            }
            catch (Exception)
            {
                DependencyService.Get<IMessage>().ShortAlert(AppResources.ErrorMessageCvsFileExported);
            }
        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (((SearchBar)sender).Text != "")
            {
                NoTvShowFound.IsVisible = false;
                TvShowListFl.FlowItemsSource = null;
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
                
                if (await TvShowListM.SearchTvShow(((SearchBar)sender).Text, Seen) > 0)
                {
                    TvShowListFl.FlowItemsSource = TvShowListM.TvShows;
                    LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                }
                else
                {
                    TvShowListFl.FlowItemsSource = null;
                    NoTvShowFound.IsVisible = true;
                    LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                }
            }
        }

        private void ShowSearch_Activated(object sender, EventArgs e)
        {
            SearchBar.IsVisible = !SearchBar.IsVisible;
        }

        private async void TypeOfSeen_Activated(object sender, EventArgs e)
        {
            Seen = Seen == 1 ? 0 : 1;
#pragma warning disable CS0618 // 'MenuItem.Icon' è obsoleto: 'Icon is obsolete as of 4.0.0. Please use IconImageSource instead.'
            TypeOfSeen.Icon = Seen == 0 ? "unseen" : "seen";
#pragma warning restore CS0618 // 'MenuItem.Icon' è obsoleto: 'Icon is obsolete as of 4.0.0. Please use IconImageSource instead.'

            TvShowListFl.FlowItemsSource = null;
            NoTvShowFound.IsVisible = false;
            LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
            await TvShowListM.LoadTvShow(Seen);
            if (TvShowListM.TvShows.Count > 0)
            {
                Title = AppResources.TitlePageTvShowList + " " + TvShowListM.TvShows.Count.ToString();
                TvShowListFl.FlowItemsSource = TvShowListM.TvShows;
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
            }
            else
            {
                Title = AppResources.TitlePageTvShowList;
                TvShowListFl.FlowItemsSource = null;
                NoTvShowFound.IsVisible = true;
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
            }
        }
    }
}