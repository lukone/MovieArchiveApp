using MovieArchive.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using Xamarin.Forms;

namespace MovieArchive
{
    public class DataExchange
    {
        public List<Movie> MoviesFoundMulti;
        public List<Movie> MoviesFound;
       
        private List<string> FileNameExclusion = new List<string> { "xvid", "dvix", "BDRip", "BRRip", "dvdrip", "CD1", "CD2", "480p", "720p", "1080p", " ENG ", " ITA", "ITALIAN", "2007", "2008", "2009", "2010", "2011", "AC3", "ITALIA", "2004", "2005", "avi", "mkv", "mp4", "2006", "2003", "2002", "(2010)", "(2011)", "2012", "(2012)", "2013", "(2013)", "2014", "(2014)", "2015", "(2015)", "576p", "1080p", "576p", "1080p", "640p", "720p", "640p", "720p", "2016", "(2016)", "2017", "(2017)", "2018", "(2018)", "2019", "(2019)", "2020", "(2020)", "2021", "(2021)", "2022", "(2022)" };
        private List<string> FileNameSubstitution = new List<string> { ".", ",", "_", "-" };

        private DataBase DB;

        public DataExchange()
        {
            DB = new DataBase();
        }

        // import data from csv file
        public async Task ImportDataFromFile(string FilePath)
        {
            FileInfo FileRead = new FileInfo(FilePath);
            string Row;
            string[] Field;

            if (File.Exists(FilePath))
            {
                StreamReader rReader = FileRead.OpenText();

                Row = rReader.ReadLine();

                while ((Row = rReader.ReadLine()) != null)
                {
                    if (Row != null)
                    {
                        Field = Row.Split(';');
                        var MovieImport = new Movie();

                        MovieImport.ID = int.Parse(Field[0]);
                        MovieImport.Title = Field[3];
                        //MovieImport.Poster = Field[4];
                        MovieImport.TmdbID = int.Parse(Field[2]);
                        if (Field[1] != "")
                            MovieImport.DateIns = DateTime.Parse(Field[1].Substring(0, Field[1].IndexOf(" ")), CultureInfo.CreateSpecificCulture("it-ITA"));
                        else
                            MovieImport.DateIns = DateTime.Now;
                        if (Field[5] != "")
                            MovieImport.DateView = DateTime.Parse(Field[5], CultureInfo.CreateSpecificCulture("it-ITA"));
                        if (Field[6] != "")
                            MovieImport.Rating = int.Parse(Field[6]);
                        else
                            MovieImport.Rating = 0;

                        //cerco il poster aggiornato 
                        var movie = SearchMovieInTMDBByID(MovieImport.TmdbID);
                        //MovieImport.Poster = Dim_SmallPoster+movie.Poster;
                        MovieImport.Poster = movie.Poster;

                        var Result = await DB.InsertMovieAsync(MovieImport);

                        MoviesFoundMulti.Add(MovieImport);
                    }
                }
            }
        }

        // import data from web api 
        public void ImportDataFromWebApi()
        {            
            Property PR = DB.GetPropertyAsync().Result;
            if (PR != null && PR.WebApiAddress != "" && PR.WebApiAddress != null)
            {
                try
                {
                    var MS = new WebApi(PR.WebApiAddress);
                    MoviesFound = MS.GetDataMovieArchiveWS(PR.GetMovieLastUpdate);
                               
                    if(MoviesFound.Count>0)
                        DB.InsertMoviesAsync(MoviesFound).Wait();

                    //Update date
                    PR.GetMovieLastUpdate=PR.GetRatingLastUpdate = DateTime.Now;
                    int r = DB.UpdatePropertyAsync(PR).Result;
                }
                catch (Exception e)
                { throw e; }
            }
            else
                throw new NotSupportedException("ERROR: WebApi address not valid");
        }

        // import last update data from web api 
        public async Task UpdateDataFromWebApi()
        {
            Property PR = DB.GetPropertyAsync().Result;
            if (PR != null && PR.WebApiAddress != "" && PR.WebApiAddress != null)
            {
                try
                {
                    var MS = new WebApi(PR.WebApiAddress);

                    //New movie not seen
                    MoviesFound = MS.GetDataMovieArchiveWS(PR.GetMovieLastUpdate);

                    if (MoviesFound != null && MoviesFound.Count > 0)
                    {
                        await DB.InsertMoviesAsync(MoviesFound);

                        DependencyService.Get<IMessage>().ShortAlert(String.Format(AppResources.MessageNMovieImported, MoviesFound.Count.ToString()));

                        //Update date
                        PR.GetMovieLastUpdate = DateTime.Now;
                        await DB.UpdatePropertyAsync(PR);
                    }
                    else
                        DependencyService.Get<IMessage>().ShortAlert(AppResources.Message0MovieImported);

                    //Get rating of last movie seen
                    MoviesFound = MS.GetLastMovieRatingWS(PR.GetRatingLastUpdate);

                    if (MoviesFound != null && MoviesFound.Count > 0)
                    {
                        await DB.UpdateMoviesAsync(MoviesFound);
                        //Update date
                        PR.GetRatingLastUpdate = DateTime.Now;
                        await DB.UpdatePropertyAsync(PR);
                    }

                }
                catch (Exception e)
                { throw e; }
            }
        }

        //import movie from file path
        public void ImportDataFromFolder(string MoviePath)
        {
            string MovieToSearch;
            MoviesFoundMulti = new List<Movie>();

            if (Directory.Exists(MoviePath))
            {
                var Dir = Directory.GetFiles(MoviePath);

                foreach(string file in Dir)
                {
                    MovieToSearch=Path.GetFileNameWithoutExtension(file.Normalize());

                    foreach (string FE in FileNameExclusion)
                        MovieToSearch = MovieToSearch.Substring(0, MovieToSearch.IndexOf(FE)<0 ? MovieToSearch.Length : MovieToSearch.IndexOf(FE));

                    foreach (string FS in FileNameSubstitution)
                        MovieToSearch = MovieToSearch.Replace(FS," ");

                    SearchMovieInTMDB(MovieToSearch);

                    MoviesFoundMulti.AddRange(MoviesFound);
                }
            }
        }

        public void SearchMovieInTMDB(string SearchText)
        {
            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3,true);
            //TMDbConfig conf = await client.GetConfigAsync();
            SearchContainer<SearchMovie> results = client.SearchMovieAsync(SearchText, CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Result;

            MoviesFound = new List<Movie>();
            Movie MovieFound;

            foreach (SearchMovie result in results.Results)
            {
                MovieFound = new Movie();        
                MovieFound.TmdbID = result.Id;
                MovieFound.Title = result.Title;
                MovieFound.Poster = (result.PosterPath ?? "").Replace("/","");

                MoviesFound.Add(MovieFound);
            }
        }

        public Movie SearchMovieInTMDBByID(int TMDbID)
        {
            try
            {
                TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
                //TMDbConfig conf = await client.GetConfigAsync();
                var result = client.GetMovieAsync(TMDbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Result;

                Movie MovieFound = new Movie();

                if (result.Id != 0)
                {
                    MovieFound = new Movie();
                    MovieFound.TmdbID = result.Id;
                    MovieFound.Title = result.Title;
                    MovieFound.Poster = (result.PosterPath ??"").Replace("/", "");
                }

                return MovieFound;
            }
            catch(Exception e)
            { throw e; }
        }

        public List<Person> GetCastAndCrew(int TMDbID)
        {
            var Crews = new List<Person>();
            var Crew = new Person();
            //TMDbLib.Objects.People.ProfileImages CastImage;

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = client.GetMovieAsync(TMDbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, MovieMethods.Credits).Result;

            foreach (Cast actor in result.Credits.Cast)
            {
                Crew = new Person();
                Crew.Name = actor.Name;
                Crew.ProfilePath = "";
                Crew.tmdbid = actor.Id;
                Crew.Photo = (actor.ProfilePath ?? "").Replace("/", "");
                //CastImage = client.GetPersonImagesAsync(actor.Id).Result;
                //if (CastImage.Id != 0 && CastImage.Profiles.Count>0)
                //{
                //    Crew.Photo = Dim_CrewPhoto+CastImage.Profiles[0].FilePath;
                //}
                Crew.Type = "Actor";

                Crews.Add(Crew);
            }
            
            foreach (Crew Director in result.Credits.Crew)
            {
                if (Director.Job == "Director")
                {
                    Crew = new Person();
                    Crew.Name = Director.Name;
                    Crew.ProfilePath = "";
                    Crew.tmdbid = Director.Id;
                    Crew.Photo = (Director.ProfilePath ?? "").Replace("/", "");
                    //CastImage = client.GetPersonImagesAsync(Director.Id).Result;
                    //if (CastImage.Id != 0 && CastImage.Profiles.Count > 0)
                    //{
                    //    Crew.Photo = Dim_CrewPhoto+CastImage.Profiles[0].FilePath;
                    //}
                    Crew.Type = "Director";

                    Crews.Add(Crew);
                }
            }

            return Crews;
        }

        public string GetTrailer(int TMDbID)
        {
            string Trailer="";
            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = client.GetMovieAsync(TMDbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, MovieMethods.Videos).Result;

            if(result.Video)
                Trailer = result.Videos.Results[0].Key;

            return Trailer;
        }

        public MovieDetails GetMovieDetail(MovieDetails Movie)
        {

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = client.GetMovieAsync(Movie.TmdbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, MovieMethods.Videos | MovieMethods.Credits).Result;

            if (result.Id != 0)
            {
                Movie.ImdbID = result.ImdbId;
                Movie.Synopsis = result.Overview;
                Movie.OriginalTitle = result.OriginalTitle;
                Movie.ReleaseDate = result.ReleaseDate;
                //Movie.AVGRating = result.VoteAverage;
                Movie.HomePage = result.Homepage;
                Movie.Backdrop = result.BackdropPath;
                Movie.Runtime = result.Runtime;
                if (result.ProductionCountries.Count > 0)
                    Movie.ProductionCountry = result.ProductionCountries.First().Name;
                else
                    Movie.ProductionCountry = "";
                if (result.Videos.Results.Count > 0)
                    Movie.Trailer = result.Videos.Results.Where(m => m.Type == "Trailer").Select(t => t.Site == "YouTube" ? "https://www.youtube.com/embed/" + t.Key : t.Key).FirstOrDefault();
                else
                    Movie.Trailer = "";
                Movie.Actors = (from d in result.Credits.Cast select new Person() { Name = d.Name, tmdbid = d.Id, Photo = (d.ProfilePath ?? "").Replace("/", ""), Type = "Director" }).ToList();
                Movie.Directors = (from d in result.Credits.Crew where d.Job == "Director" select new Person() { Name = d.Name, tmdbid = d.Id, Photo = (d.ProfilePath ?? "").Replace("/", ""), Type = "Director" }).ToList();

                Movie.Genres = String.Join(" - ", result.Genres.Select(e => e.Name).ToList());
                Movie.Ratings = new List<Rating>();
                Movie.Ratings.Add(new Rating { Source = "TMDB", Value = result.VoteAverage.ToString() });
            }

            return Movie;
        }

        public static void WriteCSV<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }
    }
}
