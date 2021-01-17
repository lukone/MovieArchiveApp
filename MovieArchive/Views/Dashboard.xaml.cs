using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Dashboard : ContentPage
	{
        DashBoardModel DA;
        const int NItems = 3;
        //List<MovieCarousel> Movies= new List<MovieCarousel>();
        List<Movie> Movies = new List<Movie>();
        List<TvShow> TvShows = new List<TvShow>();

        public Dashboard()
		{            
            InitializeComponent();
        }
       
        protected async override void OnAppearing()
        {
            try
            {
                DA = new DashBoardModel();

                if (await DA.LastMovieUploaded(NItems) > 0)
                {
                    //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                    //carouselLast.ItemsSource = Movies;
                    Movies = DA.Movies;
                    HorListLastAdded.ItemsSource = Movies;
                }
                else { 
                    WelcomeMessage1.IsVisible = true;
                    WelcomeMessage2.IsVisible = true;
                    TitleLastAdd.IsVisible = false;
                    TitleLastSaw.IsVisible = false;
                    TitleBestRating.IsVisible = false;
                    TitleTvLastSeen.IsVisible = false;
                }

                if (await DA.LastMovieSeen(NItems) > 0)
                {
                    //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                    //carouselSeen.ItemsSource = Movies;
                    Movies = DA.Movies;
                    HorListLastSeen.ItemsSource = Movies;
                }

                if (await DA.BestRatedMovie(NItems) > 0)
                {
                    //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                    //carouselBest.ItemsSource = Movies;
                    Movies = DA.Movies;
                    HorListBestRating.ItemsSource = Movies;
                }

                if (await DA.LastTvShowSeen(NItems) > 0)
                {
                    //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                    //carouselSeen.ItemsSource = Movies;
                    TvShows = DA.TvShows;
                    HorListTvLastSeen.ItemsSource = TvShows;
                }
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex; 
            }
        }

        //private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    var ca = (CarouselViewControl)((CachedImage)sender).Parent;

        //    await ca.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
        //    await ca.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
        //    var mi = (Movie)(ca.ItemsSource.GetItem(ca.Position));
        //    await Navigation.PushAsync( new MovieCardV2(mi));
        //}

        private async void HorListLastAdded_SelectedItemChanged(object sender, EventArgs e)
        {
            var ca = (HorizontalList)sender;

            if (ca.SelectedItem != null)
            {
                if (ca.ViewSelect != null)
                {
                    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
                    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
                }
                var mi = (Movie)(ca.SelectedItem);
                await Navigation.PushAsync(new MovieCardV2(mi));
            }
        }
        private async void HorListLastSeen_SelectedItemChanged(object sender, EventArgs e)
        {
            var ca = (HorizontalList)sender;
            if (ca.SelectedItem != null)
            {
                if (ca.ViewSelect != null)
                {
                    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
                    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
                }
                var mi = (Movie)(ca.SelectedItem);
                await Navigation.PushAsync(new MovieCardV2(mi));
            }
        }
        private async void HorListBestRating_SelectedItemChanged(object sender, EventArgs e)
        {
            var ca = (HorizontalList)sender;
            if (ca.SelectedItem != null)
            {
                if (ca.ViewSelect != null)
                {
                    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
                    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
                }
                var mi = (Movie)(ca.SelectedItem);
                await Navigation.PushAsync(new MovieCardV2(mi));
            }
        }

        private async void HorListTvLastSeen_SelectedItemChanged(object sender, EventArgs e)
        {
            var ca = (HorizontalList)sender;
            if (ca.SelectedItem != null)
            {
                if (ca.ViewSelect != null)
                {
                    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
                    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
                }
                var mi = (TvShow)(ca.SelectedItem);
                await Navigation.PushAsync(new TvShowCard(mi));
            }
        }
    }
}