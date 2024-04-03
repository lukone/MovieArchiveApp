using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;

namespace MovieArchive
{
    public class WebApi
    {
        private HttpClient client;
        private string WebCall;

        public WebApi(string address)
        {
            if (address != "")
            {
                WebCall = address;
                client = new HttpClient();
            }
            else
                throw new NotSupportedException("ERROR: WebApi address not valid");
        }
        
        public WebApi()
        {
            client = new HttpClient();
        }
        
        #region Movie

        private readonly string GetData = "GetDataFromDB.php?{0}={1}";
        private readonly string SetRating = "SetRating.php?{0}={1}&{2}={3}";
        private readonly string InsNewMovie = "InsMovie.php?{0}={1}&{2}={3}&{4}={5}&{6}={7}";
        private readonly string InsMovieGenre = "InsMovieGenre.php?{0}={1}&{2}={3}";
        private readonly string UpdateBackdrop = "UpdateBackDrop.php";
        private readonly string GetRating = "GetLastRating.php?{0}={1}";
        static readonly string Active = "IsActive.php";

        public static bool IsActive(string Address)
        {
            try
            {
                var uri = new Uri(Address + WebApi.Active);
                var client = new HttpClient();
                var response = client.GetAsync(uri, HttpCompletionOption.ResponseContentRead).Result;

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public async Task<List<Movie>> GetDataMovieArchiveWS(DateTime LastUpdate)
        {
            Uri uri;
            try
            {

                uri = new Uri(String.Format(WebCall+GetData, "LastUpdateDate", LastUpdate.ToString("yyyy.MM.dd HH:mm:ss")));

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<List<Movie>>(content, new CustomDateTimeConverter());

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }

        }

        public async Task<List<Movie>> GetLastMovieRatingWS(DateTime LastUpdate)
        {
            Uri uri;
            try
            {

                uri = new Uri(String.Format(WebCall+GetRating, "LastUpdateDate", LastUpdate.ToString("yyyy.MM.dd HH:mm:ss")));

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<List<Movie>>(content, new CustomDateTimeConverter());

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }

        }

        public async Task SetRatingWS(int ID, int Rating)
        {
            var uri = new Uri(String.Format(WebCall + SetRating, "id", ID.ToString(), "rate", Rating.ToString()));
            try
            {
                var response = client.GetAsync(uri).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new NotSupportedException(string.Format("ERROR: WebApi Post ", uri.ToString()));
            }
            catch (Exception ex) 
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task UpdateBackdropWS(int ID, string Backdrop)
        {

            var uri = new Uri(WebCall + UpdateBackdrop);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("id", ID.ToString()),
                new KeyValuePair<string, string>("backdrop",  Backdrop),
            });

            var response = await client.PostAsync(uri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new NotSupportedException(string.Format("ERROR: WebApi Post ", uri.ToString()));
        }

        public async Task InsertNewMovie(int IDTMDB, string Title, string Picture,string Backdrop)
        {
            var uri = new Uri(String.Format(WebCall + InsNewMovie, "IDTMDB", IDTMDB.ToString(), "Title", Title, "Picture", Picture, "Backdrop", Backdrop));
            try
            {
                var response = client.GetAsync(uri).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new NotSupportedException(string.Format("ERROR: WebApi Post ", uri.ToString()));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task InsertMovieGenre(int IDmovie, string Genre)
        {
            var uri = new Uri(String.Format(WebCall + InsMovieGenre, "IDmovie", IDmovie.ToString(), "Genre", Genre));
            try
            {
                var response = client.GetAsync(uri).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new NotSupportedException(string.Format("ERROR: WebApi Post ", uri.ToString()));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion

        #region TVShow
        private readonly string InsNewTvShow = "InsTvShow.php?{0}={1}&{2}={3}&{4}={5}";
        private readonly string SetRatingEpisode = "SetRatingTvShow.php?{0}={1}&{2}={3}&{4}={5}&{6}={7}";
        private readonly string GetTvShowData = "GetTvShowDataFromDB.php?{0}={1}";
        private readonly string GetSeasonData = "GetSeasonDataFromDB.php?{0}={1}";
        private readonly string GetEpisodeData = "GetEpisodeDataFromDB.php?{0}={1}&{2}={3}";

        public async Task<List<TvShow>> GetDataTvShowWS(DateTime LastUpdate)
        {
            Uri uri;
            try
            {

                uri = new Uri(String.Format(WebCall + GetTvShowData, "LastUpdateDate", LastUpdate.ToString("yyyy.MM.dd HH:mm:ss")));

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<List<TvShow>>(content, new CustomDateTimeConverter());

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }

        }

        public async Task<List<Season>> GetDataSeasonWS(int ID)
        {
            Uri uri;
            try
            {

                uri = new Uri(String.Format(WebCall + GetSeasonData, "ID", ID));

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<List<Season>>(content, new CustomDateTimeConverter());

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }

        }
        
        public async Task<List<Episode>> GetDataEpisodeWS(int ID,int N)
        {
            Uri uri;
            try
            {

                uri = new Uri(String.Format(WebCall + GetEpisodeData, "ID", ID,"N",N));

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<List<Episode>>(content, new CustomDateTimeConverter());

                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }

        }

        public async Task InsertNewTvShow(int IDTMDB, string Title, string Picture)
        {
            var uri = new Uri(String.Format(WebCall + InsNewTvShow, "IDTMDB", IDTMDB.ToString(), "Title", Title, "Picture", Picture));
            try
            {
                var response = client.GetAsync(uri).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new NotSupportedException(string.Format("ERROR: WebApi Post ", uri.ToString()));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task SetRatingTVShowWS(Episode EpisodeSeen,Season SeasonSel,BigInteger TMDBID)
        {
            var uri = new Uri(String.Format(WebCall + SetRatingEpisode, "tmdbid", TMDBID.ToString(), "seasonN", SeasonSel.N.ToString(), "episodeN", EpisodeSeen.N.ToString(), "rate", EpisodeSeen.Rating.ToString()));
            try
            {
                var response = client.GetAsync(uri).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new NotSupportedException(string.Format("ERROR: WebApi Post ", uri.ToString()));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        #endregion

        #region omdbapi.com
        //Multiple information from imdb, rotten tomatoes ecc
        //    http://www.omdbapi.com/?apikey=[yourkey]&

        private readonly string GetMovieData = "https://www.omdbapi.com/?apikey={0}&i={1}";

        public async Task<OMDMovie> GetMovieOMDAPI(string IMDBID)
        {
            Uri uri;
            try
            {
                uri = new Uri(String.Format(GetMovieData, ApiKey.OMDApikey, IMDBID));

                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<OMDMovie>(content, new CustomDateTimeConverter());

                }
                else
                    return null;
            }
            catch (Exception ex )
            {
                Crashes.TrackError(ex);
                return null;
            }

        }

        #endregion
    }

    public class CustomDateTimeConverter : DateTimeConverterBase
    { /*"dd.M.yy HH:mm:ss"*/
        public static string[] DefaultInputFormats = new[] {                                   
        "yyyyMMdd", "yyyy/MM/dd", "dd/MM/yyyy", "dd-MM-yyyy","dd.M.yy HH:mm:ss","dd.MM.yyyy HH:mm:ss","d/M/yyyy HH:mm:ss a","dd/M/yyyy HH:mm:ss a","dd.MM.yy","dd.MM.yy HH:mm:ss",
        "yyyyMMddHHmmss", "yyyy/MM/dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss","yyyy-MM-ddTHH:mm:ssK","yyyy-MM-ddTHH:mm:ssZ"};
        public static string DefaultOutputFormat = CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern;
        public static bool DefaultEvaluateEmptyStringAsNull = true;

        private string[] InputFormats = DefaultInputFormats;
        private string OutputFormat = DefaultOutputFormat;
        private bool EvaluateEmptyStringAsNull = DefaultEvaluateEmptyStringAsNull;

        public CustomDateTimeConverter()
        {
        }

        public CustomDateTimeConverter(string[] inputFormats, string outputFormat, bool evaluateEmptyStringAsNull = true)
        {
            if (inputFormats != null) InputFormats = inputFormats;
            if (outputFormat != null) OutputFormat = outputFormat;
            EvaluateEmptyStringAsNull = evaluateEmptyStringAsNull;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string v = (reader.Value != null) ? reader.Value.ToString() : null;
            try
            {
                // The following line grants Nullable DateTime support. We will return (DateTime?)null if the Json property is null.
                if ((String.IsNullOrEmpty(v) && Nullable.GetUnderlyingType(objectType) != null) || v== DateTime.MinValue.ToString("s") || v == "0000-00-00T00:00:00Z" || v == "00.00.00")
                {
                    // If EvaluateEmptyStringAsNull is true an empty string will be treated as null, 
                    // otherwise we'll let DateTime.ParseExactwill throw an exception in a couple lines.
                    if (v == null || EvaluateEmptyStringAsNull)
                        return null;
                    else
                        return DateTime.Now;

                }
                return DateTime.ParseExact(v, InputFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw new NotSupportedException(String.Format("ERROR: Input value '{0}' is not parseable using the following supported formats: {1}", v, string.Join(",", InputFormats)));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(OutputFormat));
        }
    }

}
