using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieArchive
{
    class MovieSeenModel
    {
        public List<Movie> Movies = new List<Movie>();
        private DataBase DB;

        public MovieSeenModel()
        {
            DB = new DataBase();
        }

        public void LoadMovieSeen()
        {

            try
            {
                Movies = new List<Movie>(from m in DB.GetMovieAsync().Result where m.Rating != 0 orderby m.DateView descending select m);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
