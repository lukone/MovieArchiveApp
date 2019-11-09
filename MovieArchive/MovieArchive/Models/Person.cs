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
        public string PhotoW45 { get { return string.Format(PathImage, "w45", Photo); } }
        [Ignore]
        public string PhotoW185 { get { return string.Format(PathImage, "w185", Photo); } }
        [Ignore]
        public string PhotoW632 { get { return string.Format(PathImage, "h632", Photo); } }

        public const string PathImage = "https://image.tmdb.org/t/p/{0}/{1}";

        public Person()
        {
        }

        public Person(Person toCopy)
        {
            this.tmdbid = toCopy.tmdbid;
            this.Name = toCopy.Name;
            this.Photo = toCopy.Photo;
            this.ProfilePath = toCopy.ProfilePath;
            this.Type = toCopy.Type;
        }

    }

    public class PersonDetails : Person
    {
        public string Biography { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? Deathday { get; set; }
        public string Gender { get; set; }
        public string PlaceOfBirth { get; set; }
        public string HomePage { get; set; }
        public List<string> MovieCredits { get; set; }
        public List<string> TvCredits { get; set; }

        public PersonDetails()
        { }

        public PersonDetails(Person toCopy)
        : base(toCopy)
        { }
    }
}
