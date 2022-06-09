using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieArchive
{
    public class Season
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int N { get; set; }
        public int TmdbID { get; set; }
        public int EpisodeCount { get; set; }
        public int EpisodeSeen { get; set; }
        public int PersonalRatigAVG { get; set; }

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

        [Ignore]
        public string Title { get { return "S-" + N.ToString(); } }

        [Ignore]
        public List<Episode> Episodes { get; set; }

        [Ignore]
        public bool IsChecked { get { return EpisodeCount == EpisodeSeen; } }
    }
}
