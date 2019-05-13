using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieArchive
{
    public class Person
    {
        public int tmdbid { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string ProfilePath { get; set; }
        public string Type { get; set; }

        [Ignore]
        public string PhotoW45 { get { return string.Format(PathImage, "45", Photo); } }
        [Ignore]
        public string PhotoW185 { get { return string.Format(PathImage, "185", Photo); } }

        public const string PathImage = "https://image.tmdb.org/t/p/w{0}/{1}";
    }
}
