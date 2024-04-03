using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MovieArchive
{
    public class InsertMediaModel
    {
        public List<Movie> Movies = new List<Movie>();
        public List<TvShow> TvShows = new List<TvShow>();
        //private DataBase DB;

        public InsertMediaModel()
        {
            //DB = new DataBase();
        }

        #region movie
        public async Task ImportMovieFromFile(byte[] DataFile)
        {
            var DE = new DataExchange();

            Stream stream = new MemoryStream(DataFile);

            await DE.ImportMovieDataFromFile(stream);
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
            DE.ImportMovieDataFromFolder(SearchPath);

            Movies = DE.MoviesFoundMulti;

        }

        #endregion

        #region TvShow
        public async Task<int> SearchTvShow(string SearchText)
        {
            await Task.Run(() =>
            {
                var DE = new DataExchange();

                DE.SearchTvShowInTMDB(SearchText);
                TvShows = DE.TvShowsFound;
            });
            return TvShows.Count;

        }

        public void SearchTvShowFromFolder(string SearchPath)
        {
            var DE = new DataExchange();
            DE.ImportTvShowDataFromFolder(SearchPath);

            TvShows = DE.TvShowsFound;

        }
     
        public async Task ImportTvShowFromFile(byte[] DataFile)
        {
            var DE = new DataExchange();

            Stream stream = new MemoryStream(DataFile);

            await DE.ImportTvShowDataFromFile(stream);
            TvShows = DE.TvShowsFound;
        }

        public async Task ImportTvShowFromWebService()
        {
            var DE = new DataExchange();

            await DE.ImportTVShowDataFromWebApi();
            TvShows = DE.TvShowsFound;
        }

        #endregion
    }
} 
