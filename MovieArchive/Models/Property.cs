using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieArchive
{ 
    public class Property
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int DBVersion { get; set; }
        public bool AutomaticBackup { get; set; }
        public string WebApiAddress { get; set; }
        public DateTime GetMovieLastUpdate { get; set; }
        public DateTime GetRatingLastUpdate { get; set; }
        public DateTime GetTvShowLastUpdate { get; set; }
        public DateTime GetTVShowRatingLastUpdate { get; set; }
    }

}
