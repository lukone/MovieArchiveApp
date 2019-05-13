using System;
using System.Collections.Generic;
using System.Text;

namespace MovieArchive
{
    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }
        public string WebSite
        {
            get
            {
                string Site; switch (Source)
                {
                    case "TMDB":
                        Site = "themoviedb.org";
                        break;
                    case "Internet Movie Database":
                        Site = "imdb.com";
                        break;
                    case "Rotten Tomatoes":
                        Site = "rottentomatoes.com";
                        break;
                    case "Metacritic":
                        Site = "metacritic.com";
                        break;
                    default: Site = "None"; break;
                }
                return Site;
            }
        }
        public string ImageSrc { get { return string.Format("http://logo.clearbit.com/{0}", WebSite); } }
    }
}
