using MovieArchive.Resources;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieUnseen : ContentPage
	{
        MovieUnseenModel MoviesToSee = new MovieUnseenModel();

        public MovieUnseen()
	    {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            MoviesToSee.LoadMovieToSee();
            Title = AppResources.TitlePageMovieUnseen + " " + MoviesToSee.Movies.Count.ToString();
            MovieListUnseen.FlowItemsSource = MoviesToSee.Movies;
        }

        private async void MovieListUnseen_FlowItemTapped(object sender, ItemTappedEventArgs e)
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
            MovieListUnseen.FlowScrollTo(item, ScrollToPosition.MakeVisible, true);
        }

        private void ToolbarItem_Activated(object sender, System.EventArgs e)
        {
            string File;
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageMovieSeen + ".csv");
                        break;
                    case Device.Android:
                        File = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, AppResources.TitlePageMovieSeen + ".csv");
                        break;
                    default:
                        File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppResources.TitlePageMovieSeen + ".csv");
                        break;
                }

                DataExchange.WriteCSV(MoviesToSee.Movies, File);
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageCvsFileExported);
            }
            catch(Exception)
            {
                DependencyService.Get<IMessage>().ShortAlert(AppResources.ErrorMessageCvsFileExported);
            }
        }
    }
}