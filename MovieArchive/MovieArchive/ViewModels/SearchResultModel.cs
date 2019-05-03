using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MovieArchive
{
    public class SearchResultModel
    {
        public List<Movie> Movies = new List<Movie>();
        private DataBase DB;

        public SearchResultModel()
        {
            DB = new DataBase();
        }

        public void SearchMovie(string SearchText)
        {

            Movies = DB.GetMovieAsync().Result.Where(n => n.Title.Contains(SearchText)).ToList();
            
        }


    }
}