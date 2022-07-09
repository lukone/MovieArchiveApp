using Microsoft.AppCenter.Crashes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieArchive
{
    public class MovieCardModel
    {
            public MovieDetails MovieDet;

            public MovieCardModel(Movie movie)
            {
                //Get movie base data from the class movie selected
                MovieDet = new MovieDetails(movie);

            }
    
            public async Task<int> GetDetail()
            {
                try
                {
                    var DE = new DataExchange();
                    MovieDet=await DE.GetMovieDetail(MovieDet);
                    //var Crews=DE.GetCastAndCrew(MovieDet.TmdbID);
                    //MovieDet.Actors = Crews.Where(m => m.Type == "Actor").ToList();
                    //MovieDet.Directors = Crews.Where(m => m.Type == "Director").ToList();
                    return 1;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return 0;
                }
            }

            public async Task<int> GetCrew()
            {
                try
                {
                    var DE = new DataExchange();
                    var Crews = await DE.GetCastAndCrew(MovieDet.TmdbID);
                    MovieDet.Actors = Crews.Where(m => m.Type == "Actor").ToList();
                    MovieDet.Directors = Crews.Where(m => m.Type == "Director").ToList();
                    return 1;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return 0;
                }
            }

            public async Task<int> GetWebRating()
            {
                try
                {
                    var WA = new WebApi();
                    OMDMovie OM = await WA.GetMovieOMDAPI(MovieDet.ImdbID);
                    MovieDet.Ratings.AddRange(OM.Ratings);
                    return 1;
                }catch(Exception ex)
                {
                    Crashes.TrackError(ex);
                    return 0; 
                }
      
            }

    }

}
