using Microsoft.AppCenter.Crashes;
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

        public MovieCardV2(Movie movie)
        { 
		    InitializeComponent();

            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            MC = new MovieCardModel(movie);
            if(movie.ID==0)
            {
                this.ToolbarItems.Add(new ToolbarItem("AddMovie", "addmedia.png", async () =>
                {
                    Movie mi = new Movie((Movie)MC.MovieDet);
                   
                    mi.ID = await DB.GetNextMovieIDAsync();
                    mi.DateIns = DateTime.Now;

                    if (await DB.InsertMovieAsync(mi) > 0)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageTitleMovieImported, mi.Title));
                        //disabled for multiple tap
                        ((ToolbarItem)this.ToolbarItems[0]).IsEnabled = false;

                        if (PY.WebApiAddress != "" && PY.WebApiAddress != null)
                        {
                            //insert movie on db web
                            var WS = new WebApi(PY.WebApiAddress);
                            await WS.InsertNewMovie(mi.TmdbID, mi.Title, mi.PosterW342, mi.PosterW780);
                            var genrelist= MC.MovieDet.Genres.Split('-');
                            foreach (var genre in genrelist)
                            {
                                await WS.InsertMovieGenre(mi.ID, genre);
                            }
                        }
                    }

                }));
            }
        }

        protected async override void OnAppearing()
        {
            try
            { 
                await MC.GetDetail();
                Title = MC.MovieDet.Title;

                GridMovie.BindingContext = MC.MovieDet;

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

                await MC.GetCrew();
                HorListActor.ItemsSource = MC.MovieDet.Actors;
                HorListDirector.ItemsSource = MC.MovieDet.Directors;

                await MC.GetWebRating();
                HorListRating.ItemsSource = MC.MovieDet.Ratings;
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex;
            }
        }

        private async void Rating_OnSelect(object sender, EventArgs e)
        {
            //RatingImage ratingImage = (RatingImage)sender;

            MC.MovieDet.Rating = (int)Rating.Value;
            MC.MovieDet.DateView = DateTime.Now;
            Movie MO = new Movie(MC.MovieDet);
            int NUpdRec= await DB.UpdateMovieAsync(MO);

            if (PY.WebApiAddress != "" && PY.WebApiAddress != null)
            {
                var WS = new WebApi(PY.WebApiAddress);
                await WS.SetRatingWS(MC.MovieDet.ID, (int)MC.MovieDet.Rating);
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);
            }
            else if(NUpdRec > 0)
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);                           
        }

        private void Synopsis_Tapped(object sender, EventArgs e)
        {
            Synopsis.Text = MC.MovieDet.Synopsis;
        }

        private async void HorListDirector_SelectedItemChanged(object sender, EventArgs e)
        {
            var ca = (HorizontalList)sender;

            if (ca.SelectedItem != null)
            {
                if (ca.ViewSelect != null)
                {
                    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
                    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
                }
                var pe = (Person)(ca.SelectedItem);
                await Navigation.PushAsync(new PersonCard(pe));
            }
        }

        private async void HorListActor_SelectedItemChanged(object sender, EventArgs e)
        {
            var ca = (HorizontalList)sender;

            if (ca.SelectedItem != null)
            {
                if (ca.ViewSelect != null)
                {
                    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
                    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
                }
                var pe = (Person)(ca.SelectedItem);
                await Navigation.PushAsync(new PersonCard(pe));
            }
        }
    }

}