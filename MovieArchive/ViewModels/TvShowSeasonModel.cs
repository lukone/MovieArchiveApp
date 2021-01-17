using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieArchive
{
    public class TvShowSeasonModel
    {
            public Season SeasonDet;
            public int TmdbID;

            public TvShowSeasonModel(int tmdbid,Season season)
            {
                //Get movie base data from the class movie selected
                SeasonDet = season;
                TmdbID = tmdbid;
            }

            public async Task<int> GetSeasonEpisodes()
            {
                var DE = new DataExchange();

                SeasonDet.Episodes = await DE.GetTvShowSeasonEpisode(TmdbID, SeasonDet.N);

                return 1;
            }

    }

}
