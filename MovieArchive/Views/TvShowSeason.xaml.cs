using LaavorRatingConception;
using Microsoft.AppCenter.Crashes;
using MovieArchive.Resources;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TvShowSeason : ContentPage
	{
        private TvShowSeasonModel TS;
        private Property PY;
        private DataBase DB;

        public TvShowSeason(int tmdbid,Season season)
        { 
		    InitializeComponent();

            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            TS = new TvShowSeasonModel(tmdbid,season);
            Title = AppResources.TitleSeasonDetails + season.N.ToString();
        }

        protected async override void OnAppearing()
        {
            try
            {                              
                await TS.GetSeasonEpisodes();

                EpisodeList.ItemsSource = TS.SeasonDet.Episodes;    
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex;
            }
        }

        private async void Rating_OnSelect(object sender, EventArgs e)
        {
            var ratingImage = (RatingConception)sender;

            TS.SeasonDet.Episodes[int.Parse(ratingImage.AutomationId) - 1].Rating = (int)ratingImage.Value;
            TS.SeasonDet.Episodes[int.Parse(ratingImage.AutomationId) - 1].DateView = DateTime.Now;

            int NUpdRec = await DB.InsertEpisodeAsync(TS.SeasonDet.Episodes[int.Parse(ratingImage.AutomationId) - 1]);

            if (NUpdRec > 0)
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageRatingSaved);
        }


    }

}