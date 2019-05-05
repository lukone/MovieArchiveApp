using CarouselView.FormsPlugin.Abstractions;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Dashboard : ContentPage
	{
        DashBoardModel DA;
        const int NItems = 3;
        List<MovieCarousel> Movies= new List<MovieCarousel>();

        public Dashboard()
		{            
            InitializeComponent();
        }
       
        protected override void OnAppearing()
        {
            try
            {
                DA = new DashBoardModel();
                
                DA.LastMovieUploaded(NItems);
                Movies=DA.Movies.Select(n => new MovieCarousel(n) { position=Movies.Count}).ToList();
                carouselLast.ItemsSource = Movies;

                if (Movies.Count == 0)
                {
                    WelcomeMessage1.IsVisible = true;
                    WelcomeMessage2.IsVisible = true;
                    TitleLastAdd.IsVisible = false;
                    TitleLastSaw.IsVisible = false;
                    TitleBestRating.IsVisible = false;
                }

                DA.LastMovieSeen(NItems);
                Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                carouselSeen.ItemsSource = Movies;

                DA.BestRatedMovie(NItems);
                Movies = DA.Movies.Select(n => new MovieCarousel(n) { position = Movies.Count }).ToList();
                carouselBest.ItemsSource = Movies;

            }
            catch(Exception e)
            { throw e; }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var ca = (CarouselViewControl)((CachedImage)sender).Parent;

            await ca.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
            await ca.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
            var mi = (Movie)(ca.ItemsSource.GetItem(ca.Position));
            await Navigation.PushAsync( new MovieCardV2(mi));
        }

    }
}