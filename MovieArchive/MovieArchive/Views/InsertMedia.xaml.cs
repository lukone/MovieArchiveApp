using Microsoft.AppCenter.Crashes;
using MovieArchive.Resources;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]   
    public partial class InsertMedia : ContentPage
	{
        public InsertMediaModel MediaToIns;
        private DataBase DB;
        private Property PY;
        int TypeOfMedia = 1;           // 1 = Movie  2 = Tv Show

        public InsertMedia()
	    {
            InitializeComponent();
            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            MediaToIns = new InsertMediaModel();

        }

#pragma warning disable CS1998 // In questo metodo asincrono non sono presenti operatori 'await', pertanto verrà eseguito in modo sincrono. Provare a usare l'operatore 'await' per attendere chiamate ad API non di blocco oppure 'await Task.Run(...)' per effettuare elaborazioni basate sulla CPU in un thread in background.
        protected override async void OnAppearing()
#pragma warning restore CS1998 // In questo metodo asincrono non sono presenti operatori 'await', pertanto verrà eseguito in modo sincrono. Provare a usare l'operatore 'await' per attendere chiamate ad API non di blocco oppure 'await Task.Run(...)' per effettuare elaborazioni basate sulla CPU in un thread in background.
        {
            Folder.OnClickCommand = new Command(async () =>
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData != null)
                {
                    switch (TypeOfMedia)
                    {
                        case 1:
                            MediaToIns.SearchMovieFromFile(Path.GetDirectoryName(fileData.FilePath));
                            MovieList.ItemsSource = MediaToIns.Movies;
                            break;
                        case 2:
                            MediaToIns.SearchTvShowFromFolder(Path.GetDirectoryName(fileData.FilePath));
                            MovieList.ItemsSource = MediaToIns.TvShows;
                            break;
                        default:
                            break;
                    }

                }
            });
            Csv.OnClickCommand = new Command(async () =>
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData != null)
                {
                    switch (TypeOfMedia)
                    {
                        case 1:
                            await MediaToIns.ImportMovieFromFile(fileData.DataArray);
                            break;
                        case 2:
                            await MediaToIns.ImportTvShowFromFile(fileData.DataArray);
                            break;
                        default:
                            break;
                    }
                }
            });
            WebApi.OnClickCommand = new Command(async () =>
            {
                try
                {
                    if (TypeOfMedia == 1)
                    {
                        await MediaToIns.ImportMovieFromWebService();
                        DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageNMovieImported, MediaToIns.Movies.Count.ToString()));
                    }
                }
                catch (Exception ex) { Crashes.TrackError(ex); }
            });

            if (PY.WebApiAddress!=null && PY.WebApiAddress != "" && TypeOfMedia == 1)
                WebApi.IsEnabled = true;
            else
                WebApi.IsEnabled = false;

        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (((SearchBar)sender).Text != "")
            {
                NoMediaFound.IsVisible = false;
                LoadingIndicator.IsVisible=LoadingIndicator.IsRunning = true;
                if (TypeOfMedia == 1)
                {
                    await MediaToIns.SearchMovie(((SearchBar)sender).Text);
                    if (MediaToIns.Movies.Count > 0)
                    {
                        MovieList.FlowItemsSource = MediaToIns.Movies;
                        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                    }
                    else
                    {
                        MovieList.FlowItemsSource = null;
                        NoMediaFound.IsVisible = true;
                        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                    }
                }
                else if(TypeOfMedia == 2)
                {
                    await MediaToIns.SearchTvShow(((SearchBar)sender).Text);
                    if (MediaToIns.TvShows.Count > 0)
                    {
                        MovieList.FlowItemsSource = MediaToIns.TvShows;
                        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                    }
                    else
                    {
                        MovieList.FlowItemsSource = null;
                        NoMediaFound.IsVisible = true;
                        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                    }
                }
            }
        }

        private async void MovieList_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Group is VisualElement visual))
                return;

            await visual.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
            await visual.ScaleTo(1, length: 100, easing: Easing.CubicInOut);

            //mi.ID = DB.GetNextMovieID();
            //mi.DateIns = DateTime.Now;

            //await DB.InsertMovieAsync(mi);
            //DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageTitleMovieImported, mi.Title));
            if (TypeOfMedia == 1)
            {
                var mi = (Movie)((DLToolkit.Forms.Controls.FlowListView)sender).FlowLastTappedItem;
                await Navigation.PushAsync(new MovieCardV2(mi));
            }
            else
            {
                var mi = (TvShow)((DLToolkit.Forms.Controls.FlowListView)sender).FlowLastTappedItem;
                await Navigation.PushAsync(new TvShowCard(mi));
            }
                            
        }

        private void ShowSearch_Activated(object sender, EventArgs e)
        {
            SearchBar.IsVisible = !SearchBar.IsVisible;
        }

        private void SelTypeOfMedia_Activated(object sender, EventArgs e)
        {
            TypeOfMedia = TypeOfMedia == 1 ? 2 : 1;
            SelTypeOfMedia.IconImageSource = TypeOfMedia == 1 ? "movie" : "tv";
            WebApi.IsEnabled = TypeOfMedia == 1 ? WebApi.IsEnabled : false;

        }
    }
}