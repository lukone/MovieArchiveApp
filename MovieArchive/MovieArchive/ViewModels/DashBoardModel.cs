using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
//using SkiaSharp;
//using Microcharts;
//using Entry = Microcharts.Entry;

namespace MovieArchive
{
    class DashBoardModel
    {
        public List<Movie> Movies = new List<Movie>();
        public List<TvShow> TvShows = new List<TvShow>();
        private DataBase DB;

        private const string qryLastMovieUploaded = "Select * From Movie Order by dateins Desc LIMIT {0}";
        private const string qryLastMovieSeen = "Select * From Movie Where rating>0 Order by DateView Desc LIMIT {0}";
        private const string qryBestRatingMovie = "Select * From Movie Where rating>0 Order by rating Desc LIMIT {0}";
        private const string qryLastTvShowSeen = "Select T.* From Episode E Join TvShow T On E.TmdbID=T.TmdbID Where rating>0 Order by DateView Desc LIMIT {0}";

        //List<Entry> entries;

        public DashBoardModel()
        {
            DB = new DataBase();
        }

        #region Movie
        public async Task<int> LastMovieUploaded(int N)
        {
            try
            {
                Movies = await DB.GetMovieByQueryAsync(string.Format(qryLastMovieUploaded, N.ToString()));
                return Movies.Count;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch(Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                return 0;
            }
        }

        public async Task<int> LastMovieSeen(int N)
        {
            try
            {
                Movies = await DB.GetMovieByQueryAsync(string.Format(qryLastMovieSeen, N.ToString()));
                return Movies.Count;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch(Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                return 0;
            }
        }

        public async Task<int> BestRatedMovie(int N)
        {
            try
            {
                Movies = await DB.GetMovieByQueryAsync(string.Format(qryBestRatingMovie, N.ToString()));
                return Movies.Count;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch(Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                return 0;
            }
        }
        #endregion

        #region Tv Show

        public async Task<int> LastTvShowSeen(int N)
        {
            try
            {
                TvShows = await DB.GetTvShowByQueryAsync(string.Format(qryLastTvShowSeen, N.ToString()));
                return Movies.Count;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                return 0;
            }
        }

        #endregion

        public List<string> GetRssNews(string url)
        {
            //  @"https://www.comingsoon.it/rss/rss.asp?feed=boxofficeita"
            //  @"https://www.comingsoon.it/rss/rss.asp?feed=boxofficeusa"
            // rss / rss.asp ? feed = filmweekend
            // rss / rss.asp ? feed = filmprossim">

            var rssFeed = XDocument.Load(url);

            List<string> posts = (from item in rssFeed.Descendants("item")
                                  select item.Element("description").Value).ToList();

            return posts;

        }

        public void RatingForYear()
        {
            //Movies = DB.GetMovies();

            //Entry EntVal = new Entry(155);
            //EntVal.Label = "";
            //EntVal.ValueLabel = "155";
            //EntVal.FillColor = SKColor.Parse("#266489");

            //var entries = Movies
            //    .GroupBy(n => DateTime.Parse(n.DateView.ToString()).Year)
            //    .Select(n => new Entry(n.Count())
            //            {
            //                Label = DateTime.Parse(n.DateView.ToString()).Year,
            //                ValueLabel = n.Count(),
            //                FillColor = SKColor.Parse("#266489")
            //            }
            //    );

        }

    }


}
