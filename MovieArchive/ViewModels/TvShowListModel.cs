using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieArchive
{
    class TvShowListModel
    {
        public List<TvShow> TvShows = new List<TvShow>();
        private DataBase DB;

        public TvShowListModel()
        {
            DB = new DataBase();
        }

        //Seen = 0 Not seen 1 Seen
        public async Task<int> LoadTvShow(int Seen)
        {
            try
            {
                TvShows = await DB.GetTvShowAsync();
                if (Seen == 0) 
                    //Unseen
                    TvShows = TvShows.Where(n => n.SeasonCount > n.SeasonSeen).OrderByDescending(n => n.DateIns).ToList();
                else
                    TvShows = TvShows.Where(n => n.SeasonCount == n.SeasonSeen).OrderByDescending(s => s.DateLastEpSeen).OrderByDescending(n => n.DateIns).ToList();
                return 1;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }

        }

        //Seen = 0 Not seen 1 Seen
        public async Task<int> SearchTvShow(string SearchText,int Seen)
        {
            try
            {
                TvShows = await DB.GetTvShowAsync();
                if (Seen == 0)
                    TvShows = TvShows.Where(n => n.SeasonCount > n.SeasonSeen).OrderByDescending(n => n.DateIns).ToList();
                else
                    TvShows = TvShows.Where(n => n.SeasonCount == n.SeasonSeen).OrderByDescending(n => n.DateIns).ToList();
                return TvShows.Count;
            }
#pragma warning disable CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

    }
}
