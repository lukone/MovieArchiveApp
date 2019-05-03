using MovieArchive.Resources;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMaster : ContentPage
    {
        public ListView ListView;

        public HomeMaster()
        {
            InitializeComponent();

            BindingContext = new HomeMasterViewModel();
            ListView = MenuItemsListView;

        }

        class HomeMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<HomeMenuItem> MenuItems { get; set; }
            
            public HomeMasterViewModel()
            {
                MenuItems = new ObservableCollection<HomeMenuItem>(new[]
                {
                    new HomeMenuItem { Id = 0, Title = AppResources.MenuTitlePage1, IconSource = "contacts.png", TargetType = typeof(Dashboard) },
                    new HomeMenuItem { Id = 1, Title = AppResources.MenuTitlePage2, IconSource = "contacts.png", TargetType = typeof(MovieUnseen) },
                    new HomeMenuItem { Id = 2, Title = AppResources.MenuTitlePage3, IconSource = "contacts.png", TargetType = typeof(MovieSeen) },
                    new HomeMenuItem { Id = 3, Title = AppResources.MenuTitlePage4, IconSource = "contacts.png", TargetType = typeof(InsertMovie) },
                    new HomeMenuItem { Id = 4, Title = AppResources.MenuTitlePage5, IconSource = "contacts.png", TargetType = typeof(Properties) },

                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

        //private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (e.NewTextValue.Length > 3)
        //    {
        //        App.Current.MainPage = new Home { Detail = new NavigationPage(new SearchResult(e.NewTextValue)) };
        //    }
        //}

        //private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        //{
        //    var searchBar = (SearchBar)sender;
        //    App.Current.MainPage = new Home { Detail = new NavigationPage(new SearchResult(searchBar.Text)) };

        //}
    }
}