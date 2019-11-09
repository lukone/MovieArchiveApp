using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MovieArchive
{
    public class TvShow
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int TmdbID { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }
        public DateTime? DateIns { get; set; }
        public int SeasonCount { get; set; }
        public int SeasonSeen { get; set; }

        [Ignore]
        public string PosterW92 { get { return string.Format(PathImage, "92", Poster); } }
        [Ignore]
        public string PosterW342 { get { return string.Format(PathImage, "342", Poster); } }
        [Ignore]
        public string PosterW500 { get { return string.Format(PathImage, "500", Poster); } }
        [Ignore]
        public string PosterW780 { get { return string.Format(PathImage, "780", Poster); } }

        public const string PathImage = "https://image.tmdb.org/t/p/w{0}/{1}";

        public TvShow()
        {
        }

        public TvShow(TvShow toCopy)
        {
            this.ID = toCopy.ID;
            this.TmdbID = toCopy.TmdbID;
            this.Title = toCopy.Title;
            this.Poster = toCopy.Poster;
            this.DateIns = toCopy.DateIns;
            this.SeasonCount = toCopy.SeasonCount;
            this.SeasonSeen = toCopy.SeasonSeen;
        }

        public static explicit operator TvShow(Element v)
        {
            throw new NotImplementedException();
        }
    }

    public class TvShowDetails : TvShow
    {
        public string ImdbID { get; set; }
        public string Synopsis { get; set; }
        public string SynopsisShort { get { return Synopsis.Length < 200 ? Synopsis : Synopsis.Substring(0, 200) + " ..."; } }
        public string OriginalTitle { get; set; }
        public string HomePage { get; set; }
        public string Trailer { get; set; }
        public string Backdrop { get; set; }
        public string ProductionCountry { get; set; }
        public string Genres { get; set; }

        public List<Rating> Ratings { get; set; }
        public List<Season> Seasons { get; set; }
        public List<Person> Actors { get; set; }
        public List<Person> Directors { get; set; }

        [Ignore]
        public string BackdropW1280 { get { return string.Format(PathImage, "1280", Backdrop); } }
        [Ignore]
        public string BackdropW780 { get { return string.Format(PathImage, "780", Backdrop); } }
        [Ignore]
        public string BackdropW300 { get { return string.Format(PathImage, "300", Backdrop); } }

        public TvShowDetails()
        { }

        public TvShowDetails(TvShow toCopy)
        : base(toCopy)
        { }
    }

}
//"poster_path": "/jIhL6mlT7AblhbHJgEoiBIOUVl1.jpg",
//      "popularity": 29.780826,
//      "id": 1399,
//      "backdrop_path": "/mUkuc2wyV9dHLG0D0Loaw5pO2s8.jpg",
//      "vote_average": 7.91,
//      "overview": "Seven noble families fight for control of the mythical land of Westeros. Friction between the houses leads to full-scale war. All while a very ancient evil awakens in the farthest north. Amidst the war, a neglected military order of misfits, the Night's Watch, is all that stands between the realms of men and icy horrors beyond.",
//      "first_air_date": "2011-04-17",
//      "origin_country": [
//        "US"
//      ],
//      "genre_ids": [
//        10765,
//        10759,
//        18
//      ],
//      "original_language": "en",
//      "vote_count": 1172,
//      "name": "Game of Thrones",
//      "original_name": "Game of Thrones"


