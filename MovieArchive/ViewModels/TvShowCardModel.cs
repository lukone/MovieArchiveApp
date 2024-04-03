using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieArchive
{ 
    public class TvShowCardModel
    {
            public TvShowDetails TvShowDet;
            public DataExchange DE;

            public TvShowCardModel(TvShow tvshow)
            {
                DE = new DataExchange();
                //Get movie base data from the class movie selected
                TvShowDet = new TvShowDetails(tvshow);

            }
    
            public async Task<int> GetDetail()
            {
                
                TvShowDet = await DE.GetTvShowDetail(TvShowDet);
              
                return 1;
            }

            public async Task<int> UpdateSeasonCounter()
            {

                return await DE.CheckAndUpdateSeasonCounter(TvShowDet);

            }

            public async Task<int> GetCrew()
            {
                var Crews = await DE.GetTvShowCastAndCrew(TvShowDet.TmdbID);

                TvShowDet.Actors = Crews.Where(m => m.Type == "Actor").ToList();
                TvShowDet.Directors = Crews.Where(m => m.Type == "Director").ToList();

                return 1;
            }

            public async Task<int> GetWebRating()
            {
                    try
                    {
                        var WA = new WebApi();
                        OMDMovie OM = await WA.GetMovieOMDAPI(TvShowDet.ImdbID);
                        TvShowDet.Ratings.AddRange(OM.Ratings);
                        return 1;
                    }catch(Exception ex)
                    {
                        Crashes.TrackError(ex);
                        return 0; 
                    }
      
            }
    }

}
