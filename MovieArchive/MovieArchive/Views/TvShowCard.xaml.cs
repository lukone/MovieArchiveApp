using LaavorRatingSwap;
using Microsoft.AppCenter.Crashes;
using MovieArchive;
using MovieArchive.Resources;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TvShowCard : ContentPage
	{
        private TvShowCardModel TC;
        private Property PY;
        private DataBase DB;

        public TvShowCard(TvShow tvshow)
        { 
		    InitializeComponent();

            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            TC = new TvShowCardModel(tvshow);
            if (tvshow.ID == 0)
            {
                this.ToolbarItems.Add(new ToolbarItem("AddTvShow", "addmedia.png", async () =>
                {
                    TvShow mi = new TvShow((TvShow)TC.TvShowDet);
                
                    mi.ID = await DB.GetNextTvShowIDAsync();
                    mi.DateIns = DateTime.Now;

                    if (await DB.InsertTvShowAsync(mi) > 0)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageTitleTvShowImported, mi.Title));
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
                await TC.GetDetail();
                Title = TC.TvShowDet.Title;

                GridMovie.BindingContext = TC.TvShowDet;

                //Add trailer video
                if (TC.TvShowDet.Trailer != "")
                {
                    HtmlWebViewSource personHtmlSource = new HtmlWebViewSource();
                    personHtmlSource.SetBinding(HtmlWebViewSource.HtmlProperty, "HTMLDesc");
                    personHtmlSource.Html = string.Format(@"<html><body style='margin:0 0 0 0'><iframe allowtransparency='true' style.backgroundColor ='transparent' type='text/html' width=320 height=180 src='{0}' frameborder= 0 allowfullscreen/></body></html>", TC.TvShowDet.Trailer);
                    Trailer.Source = personHtmlSource;      
                }
                else
                { Trailer.IsVisible = false; }

                await TC.GetCrew();
                HorListActor.ItemsSource = TC.TvShowDet.Actors;
                HorListDirector.ItemsSource = TC.TvShowDet.Directors;

                await TC.GetWebRating();
                HorListRating.ItemsSource = TC.TvShowDet.Ratings;
                SeasonList.ItemsSource = TC.TvShowDet.Seasons;

                await TC.UpdateSeasonCounter();
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex;
            }
        }

        private void Synopsis_Tapped(object sender, EventArgs e)
        {
            Synopsis.Text = TC.TvShowDet.Synopsis;
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

        //private async void SeasonList_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    var ca = (ListView)sender;

        //    if (ca.SelectedItem != null)
        //    {
        //        //if (ca.ViewSelect != null)
        //        //{
        //        //    await ca.ViewSelect.ScaleTo(1.1, length: 150, easing: Easing.CubicInOut);
        //        //    await ca.ViewSelect.ScaleTo(1, length: 100, easing: Easing.CubicInOut);
        //        //}
        //        var pe = (Season)(ca.SelectedItem);
        //        await Navigation.PushAsync(new TvShowSeason(TC.TvShowDet.TmdbID,pe));
        //    }
        //}

        private async void SeasonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ca = (CollectionView)sender;

            if (ca.SelectedItem != null)
            {
                var pe = (Season)(ca.SelectedItem);
                await Navigation.PushAsync(new TvShowSeason(TC.TvShowDet.TmdbID, pe));
            }
        }

        private async void Rating_OnSelect(object sender, EventArgs e)
        {
            //Rate all season
            RatingImage ratingImage = (RatingImage)sender;
            int NUpdRec=0;
            if (TC.TvShowDet.Seasons[int.Parse(ratingImage.AutomationId) - 1].PersonalRatigAVG==0)
            {
                foreach (Episode EP in TC.TvShowDet.Seasons[int.Parse(ratingImage.AutomationId) - 1].Episodes)
                {
                    EP.Rating = (int)ratingImage.Value;
                    EP.DateView = DateTime.Now;

                    NUpdRec = await DB.InsertEpisodeAsync(EP);

                }

                if (NUpdRec > 0)
                    DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);
            }
        }

    }

}