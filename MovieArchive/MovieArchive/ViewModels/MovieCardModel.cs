using System;
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
    
            public void GetDetail()
            {
                var DE = new DataExchange();

                MovieDet=DE.GetMovieDetail(MovieDet);

                //var Crews=DE.GetCastAndCrew(MovieDet.TmdbID);

                //MovieDet.Actors = Crews.Where(m => m.Type == "Actor").ToList();
                //MovieDet.Directors = Crews.Where(m => m.Type == "Director").ToList();
            }

            public async Task<int> GetWebRating()
            {
                try
                {
                    var WA = new WebApi();
                    OMDMovie OM = WA.GetMovieOMDAPI(MovieDet.ImdbID).Result;
                    MovieDet.Ratings.AddRange(OM.Ratings);
                    return 1;
                }catch(Exception e)
                { return 0; }
      
            }
    }

}
