using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieArchive
{
    class MovieListModel
    {
        public List<Movie> Movies = new List<Movie>();
        private DataBase DB;

        public MovieListModel()
        {
            DB = new DataBase();
        }

        //Seen = 0 Not seen 1 Seen
        public async Task<int> LoadMovie(int Seen)
        {
            try
            {
                Movies = await DB.GetMovieAsync();
                if (Seen == 0)
                    Movies = Movies.Where(n => n.Rating == 0).OrderByDescending(n => n.DateIns).ToList();
                    // Movies = new List<Movie>(from m in DB.GetMovieAsync().Result where m.Rating == 0 orderby m.DateIns descending select m);
                else                    
                    Movies = Movies.Where(n => n.Rating != 0).OrderByDescending(n => n.DateIns).ToList();
                return 1;

            }
#pragma warning disable CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return 0;
            }

        }

        //Seen = 0 Not seen 1 Seen
        public async Task<int> SearchMovie(string SearchText,int Seen)
        {
            try
            {
                Movies = await DB.GetMovieAsync();
                if(Seen==0)
                    Movies = Movies.Where(n => n.Title.Contains(SearchText)).Where(n => n.Rating==0).OrderByDescending(n => n.DateIns).ToList();
                else
                    Movies = Movies.Where(n => n.Title.Contains(SearchText)).Where(n => n.Rating!=0).OrderByDescending(n => n.DateIns).ToList();
                return Movies.Count;
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
