using MovieArchive.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.TvShows;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using Xamarin.Forms;
using Microsoft.AppCenter.Crashes;

namespace MovieArchive
{
    public class DataExchange
    {
        public List<Movie> MoviesFoundMulti;
        public List<Movie> MoviesFound;
        public List<TvShow> TvShowsFound;

        private List<string> FileNameExclusion = new List<string> { "xvid", "dvix", "BDRip", "BRRip", "dvdrip", "CD1", "CD2", "480p", "720p", "1080p", " ENG ", " ITA", "ITALIAN", "2007", "2008", "2009", "2010", "2011", "AC3", "ITALIA", "2004", "2005", "avi", "mkv", "mp4", "2006", "2003", "2002", "(2010)", "(2011)", "2012", "(2012)", "2013", "(2013)", "2014", "(2014)", "2015", "(2015)", "576p", "1080p", "576p", "1080p", "640p", "720p", "640p", "720p", "2016", "(2016)", "2017", "(2017)", "2018", "(2018)", "2019", "(2019)", "2020", "(2020)", "2021", "(2021)", "2022", "(2022)" };
        private List<string> FileNameSubstitution = new List<string> { ".", ",", "_", "-" };

        private DataBase DB;

        public DataExchange()
        {
            DB = new DataBase();
        }

        #region Movie
        // import data from csv file
        public async Task ImportMovieDataFromFile(Stream DataFile)
        {
            Movie MovieImport, MovieTMDB;
            string[] values;
            int Result;

            try
            {
                MoviesFound = new List<Movie>();
                using (var reader = new StreamReader(DataFile))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (!line.Contains("ID,Title,"))
                        {
                            values = line.Split(',');

                            MovieImport = new Movie();

                            MovieImport.ID = int.Parse(values[0]);
                            MovieImport.Title = values[1];
                            MovieImport.TmdbID = int.Parse(values[2]);
                            if (values[3] != "")
                                MovieImport.DateIns = DateTime.Parse(values[3], CultureInfo.CreateSpecificCulture("it-ITA"));
                            else
                                MovieImport.DateIns = DateTime.Now;

                            //cerco il poster aggiornato 
                            MovieTMDB = SearchMovieInTMDBByID(MovieImport.TmdbID);
                            MovieImport.Poster = MovieTMDB.Poster;

                            MoviesFound.Add(MovieImport);
                        }
                    }

                    Result = await DB.InsertMoviesAsync(MoviesFound);

                }
            }
            catch (Exception ex)
            { Crashes.TrackError(ex); }

        }

        // import data from web api 
        public async Task<int> ImportDataFromWebApi()
        {            
            Property PR = DB.GetPropertyAsync().Result;
            if (PR != null && PR.WebApiAddress != "" && PR.WebApiAddress != null)
            {
                try
                {
                    var MS = new WebApi(PR.WebApiAddress);
                    MoviesFound = await MS.GetDataMovieArchiveWS(PR.GetMovieLastUpdate);

                    if (MoviesFound.Count > 0)
                        await DB.InsertMoviesAsync(MoviesFound);

                    //Update date
                    PR.GetMovieLastUpdate = PR.GetRatingLastUpdate = DateTime.Now;
                    int r = await DB.UpdatePropertyAsync(PR);
                    return r;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    return 0; 
                }
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
                    MoviesFound = await MS.GetDataMovieArchiveWS(PR.GetMovieLastUpdate);

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
                    MoviesFound = await MS.GetLastMovieRatingWS(PR.GetRatingLastUpdate);

                    if (MoviesFound != null && MoviesFound.Count > 0)
                    {
                        await DB.UpdateMoviesAsync(MoviesFound);
                        //Update date
                        PR.GetRatingLastUpdate = DateTime.Now;
                        await DB.UpdatePropertyAsync(PR);
                    }

                }
                catch (Exception ex)
                { 
                    Crashes.TrackError(ex); 
                    throw ex; 
                }
            }
        }

        //import movie from file path
        public void ImportMovieDataFromFolder(string MoviePath)
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
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex; 
            }
        }

        public async Task<List<Person>> GetCastAndCrew(int TMDbID)
        {
            var Crews = new List<Person>();
            var Crew = new Person();
            //TMDbLib.Objects.People.ProfileImages CastImage;

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = await client.GetMovieAsync(TMDbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName,null, MovieMethods.Credits);

            foreach (TMDbLib.Objects.Movies.Cast actor in result.Credits.Cast)
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
            var result = client.GetMovieAsync(TMDbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName,null, MovieMethods.Videos).Result;

            if(result.Video)
                Trailer = result.Videos.Results[0].Key;

            return Trailer;
        }

        public async Task<MovieDetails> GetMovieDetail(MovieDetails Movie)
        {

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = await client.GetMovieAsync(Movie.TmdbID, CultureInfo.CurrentCulture.TwoLetterISOLanguageName,null, MovieMethods.Videos); //| MovieMethods.Credits

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
                //Movie.Actors = (from d in result.Credits.Cast select new Person() { Name = d.Name, tmdbid = d.Id, Photo = (d.ProfilePath ?? "").Replace("/", ""), Type = "Director" }).ToList();
                //Movie.Directors = (from d in result.Credits.Crew where d.Job == "Director" select new Person() { Name = d.Name, tmdbid = d.Id, Photo = (d.ProfilePath ?? "").Replace("/", ""), Type = "Director" }).ToList();

                Movie.Genres = String.Join(" - ", result.Genres.Select(e => e.Name).ToList());
                Movie.Ratings = new List<Rating>();
                Movie.Ratings.Add(new Rating { Source = "TMDB", Value = result.VoteAverage.ToString() });

                //Get providers list
                Movie.StreamingProviders = new List<StreamingProvider>();
                try
                {
                    var resultprov = await client.GetMovieWatchProvidersAsync(Movie.TmdbID);
                    if (resultprov.Results.Count()>0 && resultprov.Results[CultureInfo.CurrentCulture.Name.Substring(CultureInfo.CurrentCulture.Name.Length - 2)].FlatRate != null)
                    {
                        foreach (var provider in resultprov.Results[CultureInfo.CurrentCulture.Name.Substring(CultureInfo.CurrentCulture.Name.Length - 2)].FlatRate)
                        {
                            var Prov = new StreamingProvider();
                            Prov.Type = "Streaming";
                            Prov.MovieID = Movie.ID;
                            Prov.logo_path = provider.LogoPath;
                            Prov.provider_name = provider.ProviderName;
                            Prov.provider_id = provider.ProviderId;
                            Prov.display_priority = provider.DisplayPriority;
                            Movie.StreamingProviders.Add(Prov);
                        }
                    }
                    else
                    {
                        var Prov = new StreamingProvider();
                        Prov.Type = "Local";
                        Prov.MovieID = Movie.ID;
                        Prov.logo_path = "plex.tv";
                        Prov.provider_name = "Plex";
                        Prov.provider_id = 0;
                        Prov.display_priority = 1;
                        Movie.StreamingProviders.Add(Prov);
                    }
                }
                catch(Exception ex)
                {
                    Crashes.TrackError(ex);
                    throw ex;
                }

            }

            return Movie;
        }
        #endregion

        #region Tv Show
        public void SearchTvShowInTMDB(string SearchText)
        {
            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            SearchContainer<SearchTv> results = client.SearchTvShowAsync(SearchText).Result;

            TvShowsFound = new List<TvShow>();
            TvShow TvShowFound;

            foreach (SearchTv result in results.Results)
            {
                TvShowFound = new TvShow();
                TvShowFound.TmdbID = result.Id;
                TvShowFound.Title = result.Name;
                TvShowFound.Poster = (result.PosterPath ?? "").Replace("/", "");

                TvShowsFound.Add(TvShowFound);
            }
        }

        public TvShow SearchTvShowInTMDBByID(int TMDbID)
        {
            try
            {
                TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
                //TMDbConfig conf = await client.GetConfigAsync();
                var result = client.GetTvShowAsync(TMDbID,language: CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Result;

                TvShow TvShowFound = new TvShow();

                if (result.Id != 0)
                {
                    TvShowFound = new TvShow();
                    TvShowFound.TmdbID = result.Id;
                    TvShowFound.Title = result.Name;
                    TvShowFound.Poster = (result.PosterPath ?? "").Replace("/", "");
                    TvShowFound.SeasonCount = result.Seasons.Count();
                }

                return TvShowFound;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex; 
            }
        }

        //import tv show from file path 
        public void ImportTvShowDataFromFolder(string TvShowPath)
        {
            string TvShowToSearch;
            MoviesFoundMulti = new List<Movie>();

            if (Directory.Exists(TvShowPath))
            {
                var Dir = Directory.GetFiles(TvShowPath);

                foreach (string file in Dir)
                {
                    TvShowToSearch = Path.GetFileNameWithoutExtension(file.Normalize());

                    foreach (string FE in FileNameExclusion)
                        TvShowToSearch = TvShowToSearch.Substring(0, TvShowToSearch.IndexOf(FE) < 0 ? TvShowToSearch.Length : TvShowToSearch.IndexOf(FE));

                    foreach (string FS in FileNameSubstitution)
                        TvShowToSearch = TvShowToSearch.Replace(FS, " ");

                    SearchTvShowInTMDB(TvShowToSearch);
                }
            }
        }

        // import data from csv file
        public async Task ImportTvShowDataFromFile(Stream DataFile)
        {
            TvShow TvShowImport, TvShowTMDB;
            string[] values;
            int Result;
            try
            {
                TvShowsFound = new List<TvShow>();
                using (var reader = new StreamReader(DataFile))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (!line.Contains("ID,Title,"))
                        {
                            values = line.Split(',');

                            //cerco il poster aggiornato 
                            TvShowTMDB = SearchTvShowInTMDBByID(int.Parse(values[2]));

                            TvShowImport = new TvShow(TvShowTMDB);

                            TvShowImport.ID = int.Parse(values[0]);
                            //TvShowImport.Title = values[1];
                            //TvShowImport.TmdbID = int.Parse(values[2]);
                            if (values[3] != "")
                                TvShowImport.DateIns = DateTime.Parse(values[3], CultureInfo.CreateSpecificCulture("it-ITA"));
                            else
                                TvShowImport.DateIns = DateTime.Now;

                            //TvShowImport.Poster = TvShowTMDB.Poster;
                            //TvShowImport.SeasonCount = TvShowTMDB.SeasonCount;                       

                            TvShowsFound.Add(TvShowImport);
                        }
                    }

                    Result = await DB.InsertTvShowsAsync(TvShowsFound);
                }
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            { Crashes.TrackError(ex); }
        }

        public async Task<TvShowDetails> GetTvShowDetail(TvShowDetails tvshow)
        {

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            TMDbLib.Objects.TvShows.TvShow result = await client.GetTvShowAsync(tvshow.TmdbID,extraMethods:TvShowMethods.Videos| TvShowMethods.ExternalIds, language : CultureInfo.CurrentCulture.TwoLetterISOLanguageName); //| MovieMethods.Credits
            
            if (result.Id != 0)
            {
                tvshow.ImdbID = result.ExternalIds.ImdbId;
                tvshow.Synopsis = result.Overview;
                tvshow.OriginalTitle = result.OriginalName;
                tvshow.HomePage = result.Homepage;
                tvshow.Backdrop = result.BackdropPath;

                if (result.OriginCountry.Count > 0)
                    tvshow.ProductionCountry = result.OriginCountry.FirstOrDefault();
                else
                    tvshow.ProductionCountry = "";
                if (result.Videos.Results.Count > 0)
                    tvshow.Trailer = result.Videos.Results.Where(m => m.Type == "Trailer").Select(t => t.Site == "YouTube" ? "https://www.youtube.com/embed/" + t.Key : t.Key).FirstOrDefault();
                else
                    tvshow.Trailer = "";

                if (result.Seasons.Count > 0)
                {
                    List<Episode> episodes = await DB.GetAllEpisodesAsync(tvshow.TmdbID);
                    tvshow.Seasons = result.Seasons.Select(s => new Season { ID = s.Id, N = s.SeasonNumber, TmdbID = tvshow.TmdbID, PersonalRatigAVG= (int)episodes.Where(e => e.SeasonN == s.SeasonNumber).Select(a => a.Rating).Average(), Poster = (s.PosterPath ?? "").Replace("/", ""), EpisodeCount = s.EpisodeCount, EpisodeSeen = episodes.Where(e => e.SeasonN == s.SeasonNumber).Count() }).ToList();
                    tvshow.SeasonCount = result.Seasons.Count;

                }
                else
                    tvshow.SeasonCount = 0;

                tvshow.SeasonSeen = 0;
                tvshow.Genres = String.Join(" - ", result.Genres.Select(e => e.Name).ToList());
                tvshow.Ratings = new List<Rating>();
                tvshow.Ratings.Add(new Rating { Source = "TMDB", Value = result.VoteAverage.ToString() });

            }

            return tvshow;
        }

        public async Task<int> CheckAndUpdateSeasonCounter(TvShowDetails tvshow)
        {
            int n=0;
            //if there is a new season update season number and so make tvshow unseen
            if (tvshow.SeasonCount < tvshow.Seasons.Count)
            {
                tvshow.SeasonCount = tvshow.Seasons.Count;
                n=await DB.UpdateTvShowAsync(new TvShow((TvShow)tvshow));
            }

            //if all episode of season are seen update number in tvshow
            int SeasonSeen = tvshow.Seasons.Where(s => s.EpisodeCount == s.EpisodeSeen).Count();
            if (SeasonSeen > tvshow.SeasonSeen)
            {
                tvshow.SeasonSeen = SeasonSeen;
                n = await DB.UpdateTvShowAsync(new TvShow((TvShow)tvshow));
            }

            return n;
        }

        public async Task<List<Person>> GetTvShowCastAndCrew(int TMDbID)
        {
            var Crews = new List<Person>();
            var Crew = new Person();
            //TMDbLib.Objects.People.ProfileImages CastImage;

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = await client.GetTvShowCreditsAsync(TMDbID,language: CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            foreach (TMDbLib.Objects.TvShows.Cast actor in result.Cast)
            {
                Crew = new Person();
                Crew.Name = actor.Name;
                Crew.ProfilePath = "";
                Crew.tmdbid = actor.Id;
                Crew.Photo = (actor.ProfilePath ?? "").Replace("/", "");
                Crew.Type = "Actor";

                Crews.Add(Crew);
            }

            foreach (Crew Director in result.Crew)
            {
                if (Director.Job == "Director")
                {
                    Crew = new Person();
                    Crew.Name = Director.Name;
                    Crew.ProfilePath = "";
                    Crew.tmdbid = Director.Id;
                    Crew.Photo = (Director.ProfilePath ?? "").Replace("/", "");
                    Crew.Type = "Director";

                    Crews.Add(Crew);
                }
            }

            return Crews;
        }

        public async Task<List<Episode>> GetTvShowSeasonEpisode(int TMDbID,int NSeason)
        {
            Episode EP;
            List<Episode> Episodes = new List<Episode>();
            List<Episode> EpisodeDB = new List<Episode>();
            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            var result = await client.GetTvSeasonAsync(TMDbID, NSeason, language: CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            if (result.Episodes.Count > 0)
            {
                EpisodeDB = await DB.GetEpisodeAsync(TMDbID, NSeason);

                //Episodes= result.Episodes.Select(s => new Episode { ID = s.Id, SeasonN = s.SeasonNumber,N = s.EpisodeNumber, Title = s.Name, Synopsis = s.Overview }).ToList();

                foreach (TvSeasonEpisode TVEpisode in result.Episodes)
                {
                    EP = new Episode();
                    EP.ID = TVEpisode.Id;
                    EP.TmdbID = TMDbID;
                    EP.SeasonN = TVEpisode.SeasonNumber;
                    EP.N = TVEpisode.EpisodeNumber;
                    EP.Title = TVEpisode.Name;
                    EP.Synopsis = TVEpisode.Overview;
                    EP.Poster = TVEpisode.StillPath;

                    if (EpisodeDB.Count >0 && EpisodeDB.Exists(x => x.N == EP.N))
                    {
                        EP.Rating = EpisodeDB.Find(x => x.N == EP.N).Rating;
                        EP.DateView = EpisodeDB.Find(x => x.N == EP.N).DateView ?? null;
                    }

                    Episodes.Add(EP);
                }
            }
            return Episodes;
        }

        #endregion

        #region Person
        public async Task<PersonDetails> GetPersonDetail(PersonDetails Person)
        {

            TMDbClient client = new TMDbClient(ApiKey.tmdbkeyV3, true);
            TMDbLib.Objects.People.Person result = await client.GetPersonAsync(Person.tmdbid);

            if (result.Id != 0)
            {

                Person.Name = result.Name;
                Person.Photo = result.ProfilePath;
                Person.Biography = result.Biography;
                Person.Birthday = result.Birthday;
                Person.Deathday = result.Deathday;
                Person.Gender = result.Gender.ToString();
                Person.PlaceOfBirth = result.PlaceOfBirth;
                Person.HomePage = result.Homepage;
                //Person.MovieCredits = result.MovieCredits;
                //Person.TvCredits = result.TvCredits;
            }

            return Person;
        }
        #endregion

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
