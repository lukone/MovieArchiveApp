using LaavorRatingSwap;
using MovieArchive;
using MovieArchive.Resources;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MovieCardV2 : ContentPage
	{
        private MovieCardModel MC;
        private Property PY;
        private DataBase DB;
        private const int ParallaxSpeed = 5;

        private double _lastScroll;

        public MovieCardV2(Movie movie)
        { 
		    InitializeComponent();

            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            MC = new MovieCardModel(movie);
            if(movie.ID==0)
            {
                this.ToolbarItems.Add(new ToolbarItem("AddMovie", "addmovie.png", () =>
                {
                    Movie mi = new Movie((Movie)MC.MovieDet);

                    mi.ID = DB.GetNextMovieID();
                    mi.DateIns = DateTime.Now;

                    if (DB.InsertMovieAsync(mi).Result > 0)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageTitleMovieImported, mi.Title));
                        //disabled for multiple tap
                        ((ToolbarItem)this.ToolbarItems[0]).IsEnabled = false;
                    }

                }));
            }
        }

        protected async override void OnAppearing()
        {
            try
            { 
                //await Task.Run(() =>
                //{
                MC.GetDetail();
                Title = MC.MovieDet.Title;

                GridMovie.BindingContext = MC.MovieDet;
                HorListActor.ItemsSource = MC.MovieDet.Actors;
                HorListDirector.ItemsSource = MC.MovieDet.Directors;

                //Add trailer video
                if (MC.MovieDet.Trailer != "")
                {
                    HtmlWebViewSource personHtmlSource = new HtmlWebViewSource();
                    personHtmlSource.SetBinding(HtmlWebViewSource.HtmlProperty, "HTMLDesc");
                    //style='background-color: #343e42;'
                    personHtmlSource.Html = string.Format(@"<html><body style='margin:0 0 0 0'><iframe allowtransparency='true' style.backgroundColor ='transparent' type='text/html' width=320 height=180 src='{0}' frameborder= 0 allowfullscreen/></body></html>", MC.MovieDet.Trailer);
                    Trailer.Source = personHtmlSource;      
                }
                else
                { Trailer.IsVisible = false; }
                //});                
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private void Rating_OnSelect(object sender, EventArgs e)
        {
            RatingImage ratingImage = (RatingImage)sender;

            MC.MovieDet.Rating = (int)Rating.Value;
            MC.MovieDet.DateView = DateTime.Now;
            Movie MO = new Movie(MC.MovieDet);
            int NUpdRec= DB.UpdateMovieAsync(MO).Result;

            if (PY.WebApiAddress != "" && PY.WebApiAddress != null)
            {
                var WS = new WebApi(PY.WebApiAddress);
                WS.SetRatingWS(MC.MovieDet.ID, (int)MC.MovieDet.Rating).Wait();
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);
            }
            else if(NUpdRec > 0)
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);                           
        }

        private void Synopsis_Tapped(object sender, EventArgs e)
        {
            Synopsis.Text = MC.MovieDet.Synopsis;
        }

        private async void WebRating_Tapped(object sender, EventArgs e)
        {
            try
            {
                await MC.GetWebRating();
                RatingList.ItemsSource = MC.MovieDet.Ratings;
                WebRating.IsVisible = false;
                RatingList.IsVisible = true;
            }
            catch (Exception ex)
            { }
        }
    }

}