using MovieArchive.Resources;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{    
    [XamlCompilation(XamlCompilationOptions.Compile)]   
    public partial class InsertMovie : ContentPage
	{
        public InsertMovieModel MoviesToIns;
        private DataBase DB;
        private Property PY;

        public InsertMovie ()
	   {
            InitializeComponent();
            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            MoviesToIns = new InsertMovieModel();

            Folder.OnClickCommand = new Command(async () =>
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();

                if (fileData != null)
                {
                    MoviesToIns.SearchMovieFromFile(Path.GetDirectoryName(fileData.FilePath));

                    MovieList.ItemsSource = MoviesToIns.Movies;
                }
            });
            Csv.OnClickCommand = new Command(async () =>
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();

                if (fileData != null)
                {
                    await MoviesToIns.ImportMovieFromFile(fileData.FilePath);
                }
            });
            WebApi.OnClickCommand = new Command(async () =>
            {
                try
                {
                    MoviesToIns.ImportMovieFromWebService();
                    DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageNMovieImported, MoviesToIns.Movies.Count.ToString()));
                }
                catch (Exception ex) { Console.Write(ex); }
            });

        }
        protected override void OnAppearing()
        {
            if (PY.WebApiAddress!=null && PY.WebApiAddress != "")
                WebApi.IsEnabled = true;
            else
                WebApi.IsEnabled = false;

        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (((SearchBar)sender).Text != "")
            {
                NoMovieFound.IsVisible = false;
                LoadingIndicator.IsRunning = true;
                await MoviesToIns.SearchMovie(((SearchBar)sender).Text);
                if (MoviesToIns.Movies.Count > 0)
                {
                    MovieList.FlowItemsSource = MoviesToIns.Movies;
                    LoadingIndicator.IsRunning = false;
                }
                else
                {
                    MovieList.FlowItemsSource = null;
                    NoMovieFound.IsVisible = true;
                    LoadingIndicator.IsRunning = false;
                }
            }
        }

        private async void MovieList_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Group is VisualElement visual))
                return;

            await visual.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
            await visual.ScaleTo(1, length: 100, easing: Easing.CubicInOut);

            var mi = (Movie)((DLToolkit.Forms.Controls.FlowListView)sender).FlowLastTappedItem;

            //mi.ID = DB.GetNextMovieID();
            //mi.DateIns = DateTime.Now;

            //await DB.InsertMovieAsync(mi);
            //DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageTitleMovieImported, mi.Title));
            await Navigation.PushAsync(new MovieCard(mi));

        }

    }
}