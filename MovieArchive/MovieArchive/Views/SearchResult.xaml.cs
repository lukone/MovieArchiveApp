using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchResult : ContentPage
	{
        public SearchResultModel ResultList;

        public SearchResult(string SearchText)
		{
			InitializeComponent();
            ResultList = new SearchResultModel();

            ResultList.SearchMovie(SearchText);
            SearchMovieList.ItemsSource = ResultList.Movies;

        }

        private void SearchMovieList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var mi = e.SelectedItem as Movie;
            App.Current.MainPage = new Home { Detail = new NavigationPage(new MovieCard(mi)) };
        }
    }
}