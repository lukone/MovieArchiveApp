using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieArchive
{
    class MovieUnseenModel
    {
        public List<Movie> Movies = new List<Movie>();
        private DataBase DB;

        public MovieUnseenModel()
        {
            DB = new DataBase();
        }

        public void LoadMovieToSee()
        {
            try
            {
                Movies = new List<Movie>(from m in DB.GetMovieAsync().Result where m.Rating == 0 orderby m.DateIns descending select m);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
