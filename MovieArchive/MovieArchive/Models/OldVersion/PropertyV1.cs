using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieArchive
{
    public class PropertyV1
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public bool AutomaticBackup { get; set; }
        public string WebApiAddress { get; set; }
        public DateTime LastUpdate { get; set; }

    }

}
