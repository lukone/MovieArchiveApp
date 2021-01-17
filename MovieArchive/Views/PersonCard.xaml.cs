using Microsoft.AppCenter.Crashes;
using MovieArchive;
using MovieArchive.Resources;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MovieArchive
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PersonCard : ContentPage
	{
        private PersonCardModel PC;
        private Property PY;
        private DataBase DB;

        public PersonCard(Person person)
        { 
		    InitializeComponent();

            DB = new DataBase();
            PY = DB.GetPropertyAsync().Result;
            PC = new PersonCardModel(person);          
        }

        protected async override void OnAppearing()
        {
            try
            { 
                await PC.GetDetail();
                Title = PC.PersonDet.Name;

                GridMovie.BindingContext = PC.PersonDet;

            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                throw ex;
            }
        }


    }

}