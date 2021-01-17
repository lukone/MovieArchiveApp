using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MovieArchive
{
    #region sizesExample
    //backdrop sizes
    //  "w300",
    //  "w780",
    //  "w1280",
    //  "original"

    //logo_sizes
    //  "w45",
    //  "w92",
    //  "w154",
    //  "w185",
    //  "w300",
    //  "w500",
    //  "original"

    //poster_sizes
    //  "w92",
    //  "w154",
    //  "w185",
    //  "w342",
    //  "w500",
    //  "w780",
    //  "original"

    //profile_sizes
    //  "w45",
    //  "w185",
    //  "h632",
    //  "original"

    //still_sizes
    //  "w92",
    //  "w185",
    //  "w300",
    //  "original"  
    #endregion

    public class Movie
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int TmdbID { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }

        public DateTime? DateIns { get; set; }
        public DateTime? DateView { get; set; }
        public int Rating { get; set; }
        [Ignore]
        public string PosterW92 { get { return string.Format(PathImage, "92", Poster); } }
        [Ignore]
        public string PosterW342 { get { return string.Format(PathImage, "342",Poster); } }
        [Ignore]
        public string PosterW500 { get { return string.Format(PathImage, "500", Poster); } }
        [Ignore]
        public string PosterW780 { get { return string.Format(PathImage, "780", Poster); } }

        public const string PathImage = "https://image.tmdb.org/t/p/w{0}/{1}";

        public Movie()
        {
        }

        public Movie(Movie toCopy)
        {
            this.ID = toCopy.ID;
            this.TmdbID = toCopy.TmdbID;
            this.Title = toCopy.Title;
            this.Poster = toCopy.Poster;
            this.DateIns = toCopy.DateIns;
            this.DateView = toCopy.DateView;
            this.Rating = toCopy.Rating;
        }

        public static explicit operator Movie(Element v)
        {
            throw new NotImplementedException();
        }
    }

    public class MovieDetails : Movie
    {
        public string ImdbID { get; set; }
        public List<Person> Actors { get; set; }
        public List<Person> Directors { get; set; }
        public string Synopsis { get; set; }
        public string SynopsisShort { get { return Synopsis.Length<200 ? Synopsis : Synopsis.Substring(0,200)+" ..."; } }
        public string OriginalTitle { get; set; }
        public DateTime? ReleaseDate { get; set; }
        //public double AVGRating { get; set; }
        public string HomePage { get; set; }
        public string Trailer { get; set; }
        public string Backdrop { get; set; }
        public int? Runtime { get; set; }
        public string ProductionCountry { get; set; }
        public string Genres { get; set; }
        public List<Rating> Ratings { get; set; }

        [Ignore]
        public string BackdropW1280 { get { return string.Format(PathImage, "1280", Backdrop); } }
        [Ignore]
        public string BackdropW780 { get { return string.Format(PathImage, "780", Backdrop); } }
        [Ignore]
        public string BackdropW300 { get { return string.Format(PathImage, "300", Backdrop); } }

        public MovieDetails()
        { }

        public MovieDetails(Movie toCopy)
        : base(toCopy)
        { }
    }

    //public class MovieCarousel : Movie
    //{
    //    public double position { get; set; }

    //    public MovieCarousel()
    //    { }

    //    public MovieCarousel(Movie toCopy)
    //    : base(toCopy)
    //    { }
    //}


}