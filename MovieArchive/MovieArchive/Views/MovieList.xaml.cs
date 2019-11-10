using MovieArchive.Resources;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieList : ContentPage
	{

        MovieListModel MoviesList = new MovieListModel();
        int Seen = 0;  //Seen = 0 Not seen 1 Seen 

        public MovieList()
	    {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await MoviesList.LoadMovie(Seen);
            Title = AppResources.TitlePageMovieList + " " + (MoviesList.Movies==null ? "" : MoviesList.Movies.Count.ToString());
            MovieListFl.FlowItemsSource = MoviesList.Movies;
        }

        private async void MovieListFl_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Group is VisualElement visual))
                return;

            await visual.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
            await visual.ScaleTo(1, length: 100, easing: Easing.CubicInOut);

            var mi = (Movie)((DLToolkit.Forms.Controls.FlowListView)sender).FlowLastTappedItem;

            //App.Current.MainPage = new Home { Detail = new NavigationPage(new MovieCard(mi)) };
            await Navigation.PushAsync(new MovieCardV2(mi));
        }

        public void FlowScrollTo(object item)
        {
            MovieListFl.FlowScrollTo(item, ScrollToPosition.MakeVisible, true);
        }

        private void SaveCSV_Activated(object sender, EventArgs e)
        {
            string File;
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageMovieList + ".csv");
                        break;
                    case Device.Android:
                        //File = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, AppResources.TitlePageMovieList + ".csv");
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageMovieList + ".csv");
                        break;
                    default:
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageMovieList + ".csv");
                        break;
                }

                DataExchange.WriteCSV(MoviesList.Movies, File);
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
                NoMovieFound.IsVisible = false;
                MovieListFl.FlowItemsSource = null;
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
                
                if (await MoviesList.SearchMovie(((SearchBar)sender).Text, Seen) > 0)
                {
                    MovieListFl.FlowItemsSource = MoviesList.Movies;
                    LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                }
                else
                {
                    MovieListFl.FlowItemsSource = null;
                    NoMovieFound.IsVisible = true;
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
            TypeOfSeen.IconImageSource = Seen == 0 ? "unseen" : "seen";

            MovieListFl.FlowItemsSource = null;
            NoMovieFound.IsVisible = false;
            LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
            await MoviesList.LoadMovie(Seen);
            if (MoviesList.Movies.Count > 0)
            {
                Title = AppResources.TitlePageMovieList + " " + MoviesList.Movies.Count.ToString();
                MovieListFl.FlowItemsSource = MoviesList.Movies;
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
            }
            else
            {
                Title = AppResources.TitlePageMovieList;
                MovieListFl.FlowItemsSource = null;
                NoMovieFound.IsVisible = true;
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
            }
        }
    }
}