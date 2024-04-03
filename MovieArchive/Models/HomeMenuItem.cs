using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieArchive
{ 

    public class HomeMenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }

    public class HomeMenuItemGroup
    {
        public string Key { get; set; }
        public string IconSource { get; set; }
        public ObservableCollection<HomeMenuItem> MenuItem = new ObservableCollection<HomeMenuItem>();
    }

}