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

        public Dashboard()
		{            
            InitializeComponent();
        }
       
        protected async override void OnAppearing()
        {
            try
            {
                DA = new DashBoardModel();

                await DA.LastMovieUploaded(NItems);
                //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                //carouselLast.ItemsSource = Movies;
                Movies = DA.Movies;
                HorListLastAdded.ItemsSource = Movies;

                if (Movies.Count == 0)
                {
                    WelcomeMessage1.IsVisible = true;
                    WelcomeMessage2.IsVisible = true;
                    TitleLastAdd.IsVisible = false;
                    TitleLastSaw.IsVisible = false;
                    TitleBestRating.IsVisible = false;
                }

                await DA.LastMovieSeen(NItems);
                //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                //carouselSeen.ItemsSource = Movies;
                Movies = DA.Movies;
                HorListLastSeen.ItemsSource = Movies;

                await DA.BestRatedMovie(NItems);
                //Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                //carouselBest.ItemsSource = Movies;
                Movies = DA.Movies;
                HorListBestRating.ItemsSource = Movies;
            }
            catch(Exception e)
            { throw e; }
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
    }
}