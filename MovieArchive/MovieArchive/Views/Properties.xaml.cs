using MovieArchive.Resources;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Properties : ContentPage
	{
        Property PY;
        DataBase DB;

        public Properties ()
        {
            InitializeComponent ();

            DB = new DataBase();

            PY = DB.GetPropertyAsync().Result ?? new Property();
            AutoBkp.BindingContext = PY;
            WebApiAddress.BindingContext = PY;
        }

        async void AutoBkp_Toggled(object sender, ToggledEventArgs e)
        {
            PY.AutomaticBackup = e.Value;
            int r = await DB.UpdatePropertyAsync(PY);
        }

        private void ConnectToGoogle_Clicked(object sender, EventArgs e)
        {
            //if (GoogleDriveAPIV3.GoogleDriveConnection("MovieArchive"))
            //{
            //    GConnected.Text = "Connected";
            //}
            //else
            //{
            //    GConnected.Text = "Error";
            //}
        }

        private void ResetTotalDB_Clicked(object sender, EventArgs e)
        {
            DB.ResetDB();
            DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageTotalDBReset);
        }

        async void ResetMovieDB_Clicked(object sender, EventArgs e)
        {
            DB.ResetMovieDB();

            PY.GetMovieLastUpdate = DateTime.MinValue;
            PY.GetRatingLastUpdate = DateTime.MinValue;
            int r = await DB.UpdatePropertyAsync(PY);
            DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageMovieDBReset);
        }

        async void WebApiAddress_Completed(object sender, EventArgs e)
        {
            if (WebApi.IsActive(((Xamarin.Forms.Editor)sender).Text))
            {
                PY.WebApiAddress = ((Xamarin.Forms.Editor)sender).Text;
                int r = await DB.UpdatePropertyAsync(PY);
            }
            else
            {
                WebApiAddress.Text = "";
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageWebApiNotActive);
            }
        }
    }
}