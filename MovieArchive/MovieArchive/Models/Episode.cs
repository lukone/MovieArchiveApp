using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieArchive
{
    public class Episode
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int N { get; set; }
        public int TmdbID { get; set; }
        public int SeasonN { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        //public string SynopsisShort { get { return (Synopsis=="" ? 0 : Synopsis.Length) < 200 ? Synopsis : Synopsis.Substring(0, 200) + " ..."; } }
        [Ignore]
        public string Poster { get; set; }
        [Ignore]
        public string PosterW92 { get { return string.Format(PathImage, "92", Poster); } }
        [Ignore]
        public string PosterW342 { get { return string.Format(PathImage, "342", Poster); } }
        [Ignore]
        public string PosterW500 { get { return string.Format(PathImage, "500", Poster); } }
        [Ignore]
        public string PosterW780 { get { return string.Format(PathImage, "780", Poster); } }

        public const string PathImage = "https://image.tmdb.org/t/p/w{0}/{1}";

        public int Rating { get; set; }
        public DateTime? DateView { get; set; }

        public Episode()
        { }
    }

}
