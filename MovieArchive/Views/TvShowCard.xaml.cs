﻿using Microsoft.AppCenter.Crashes;
using MovieArchive.Resources;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.XForms.TreeView;
using Syncfusion.XForms.Buttons;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms.PlatformConfiguration;
using Syncfusion.XForms.TreeView.PlatformConfiguration.WindowsSpecific;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]


	public partial class TvShowCard : ContentPage
	{
        bool ActivateTreelistUpdate=false;

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
                        //disabled for multiple tap
                        ((ToolbarItem)this.ToolbarItems[0]).IsEnabled = false;

                        if(await DB.InsertSeasonsAsync(TC.TvShowDet.Seasons) > 0)
                        {
                            foreach (Season season in TC.TvShowDet.Seasons)
                            {
                                if (await DB.InsertEpisodesAsync(season.Episodes) > 0)
                                    DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageTitleTvShowImported, mi.Title));
                                else
                                    ((ToolbarItem)this.ToolbarItems[0]).IsEnabled = true;
                            }

                            if (PY.WebApiAddress != "" && PY.WebApiAddress != null)
                            {
                                //insert Tv Show on db web
                                var WS = new WebApi(PY.WebApiAddress);
                                await WS.InsertNewTvShow(mi.TmdbID, mi.Title, mi.PosterW342);
                                
                            }
                        }
                        else
                            ((ToolbarItem)this.ToolbarItems[0]).IsEnabled = true;

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

                ActivateTreelistUpdate = true;
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


        private async void CheckBox_StateChanged(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            if (ActivateTreelistUpdate)
            {
                if (e.IsChecked.HasValue && e.IsChecked.Value)
                {
                    if (!((SfCheckBox)sender).Text.StartsWith("S-"))
                    {
                        var SeenEpisode = await DB.GetEpisodeAsyncByID(((SfCheckBox)sender).TabIndex);
                        SeenEpisode.Rating = 5;

                        var NUpdRec = await DB.UpdateEpisodeAsync(SeenEpisode);

                        //Aggiorno gli episodi visti nella serie
                        if (NUpdRec > 0)
                        {
                            var seasons = await DB.GetSeasonsAsync(SeenEpisode.TmdbID);
                            if (seasons != null && seasons.Count > 0)
                            {
                                if ((bool)e.IsChecked)
                                {
                                    seasons[SeenEpisode.SeasonN - 1].EpisodeSeen += 1;
                                    if (PY.WebApiAddress != "" && PY.WebApiAddress != null)
                                    {
                                        var WS = new WebApi(PY.WebApiAddress);
                                        await WS.SetRatingTVShowWS(SeenEpisode, seasons[SeenEpisode.SeasonN - 1], TC.TvShowDet.TmdbID);
                                    }
                                    NUpdRec = await DB.UpdateSeasonAsync(seasons[SeenEpisode.SeasonN - 1]);
                                }
                                  
                            }
                            //Aggiorno ultima data episodio visto in TvShow
                            var TVShow = await DB.GetTvShowAsync(TC.TvShowDet.ID);
                            if (TVShow != null)
                            {
                                TVShow.DateLastEpSeen = DateTime.Now;
                                NUpdRec = await DB.UpdateTvShowAsync(TVShow);
                            }
                        }
                    }
                    else //verifico se è stata selezionata una stagione intera
                    {
                        int NUpdRec;

                        bool answer = await DisplayAlert("Rating", "Mettere visto su tutti gli episodi?", "Si", "No");
                        if (answer)
                        {
                            var SeenSeason = await DB.GetSeasonAsync(((SfCheckBox)sender).TabIndex);
                            var SeenEpisodes = await DB.GetEpisodeAsync(SeenSeason.TmdbID, SeenSeason.N);
                            if (SeenEpisodes != null)
                            {
                                foreach (var SeenEp in SeenEpisodes)
                                {
                                    SeenEp.Rating = 5;

                                    NUpdRec = await DB.UpdateEpisodeAsync(SeenEp);

                                    if (SeenEp.Rating > 0)
                                    {
                                        if (PY.WebApiAddress != "" && PY.WebApiAddress != null)
                                        { 
                                            var WS = new WebApi(PY.WebApiAddress);
                                            await WS.SetRatingTVShowWS(SeenEp, SeenSeason, TC.TvShowDet.TmdbID); 
                                        }
                                    }
                                }
                                //Aggiorno gli episodi visti nella serie
                                SeenSeason.EpisodeSeen = SeenSeason.EpisodeCount;
                              
                                NUpdRec = await DB.UpdateSeasonAsync(SeenSeason);

                                //Aggiorno ultima data episodio visto in TvShow
                                var TVShow = await DB.GetTvShowAsync(TC.TvShowDet.ID);
                                if (TVShow != null)
                                {
                                    TVShow.DateLastEpSeen = DateTime.Now;
                                    NUpdRec = await DB.UpdateTvShowAsync(TVShow);
                                }
                            }
                        }

                    }
                }
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

        //private async void SeasonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var ca = (CollectionView)sender;

        //    if (ca.SelectedItem != null)
        //    {
        //        var pe = (Season)(ca.SelectedItem);
        //        await Navigation.PushAsync(new TvShowSeason(TC.TvShowDet.TmdbID, pe));
        //    }
        //}

        //private async void Rating_OnSelect(object sender, EventArgs e)
        //{
        //    //Rate all season
        //    var ratingImage = (RatingConception)sender;
        //    int NUpdRec=0;
        //    if (TC.TvShowDet.Seasons[int.Parse(ratingImage.AutomationId) - 1].PersonalRatigAVG==0)
        //    {
        //        foreach (Episode EP in TC.TvShowDet.Seasons[int.Parse(ratingImage.AutomationId) - 1].Episodes)
        //        {
        //            EP.Rating = (int)ratingImage.Value;
        //            EP.DateView = DateTime.Now;

        //            NUpdRec = await DB.InsertEpisodeAsync(EP);

        //        }

        //        if (NUpdRec > 0)
        //            DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);

        //    }
        //}

    }

}