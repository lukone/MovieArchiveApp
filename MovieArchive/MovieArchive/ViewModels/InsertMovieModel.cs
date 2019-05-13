using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieArchive
{
    public class InsertMovieModel
    {
        public List<Movie> Movies = new List<Movie>();
        private DataBase DB;

        public InsertMovieModel()
        {
            DB = new DataBase();
        }

        public async Task ImportMovieFromFile(string SearchPath)
        {
            var DE = new DataExchange();

            await DE.ImportDataFromFile(SearchPath);
            Movies = DE.MoviesFoundMulti;
        }

        public async Task ImportMovieFromWebService()
        {
            var DE = new DataExchange();

            await DE.ImportDataFromWebApi();
            Movies = DE.MoviesFound;
        }

        public async Task<int> SearchMovie(string SearchText)
        {
            await Task.Run(() =>
            {
                var DE = new DataExchange();

                    DE.SearchMovieInTMDB(SearchText);
                    Movies = DE.MoviesFound;
            });
            return Movies.Count;

        }

        public void SearchMovieFromFile(string SearchPath)
        {
            var DE = new DataExchange();
            DE.ImportDataFromFolder(SearchPath);

            Movies = DE.MoviesFoundMulti;

        }

    }
}
