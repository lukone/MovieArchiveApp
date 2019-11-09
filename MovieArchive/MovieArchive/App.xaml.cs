using DLToolkit.Forms.Controls;
using Dropbox.Api.Files;
using MovieArchive.Resources;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MovieArchive
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            AppResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
            FlowListView.Init();
            MainPage = new Home();
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android=1170586d-e16c-46e4-bcb5-6e782fa9c092;" +
                   "uwp={Your UWP App secret here};" +
                   "ios=e5be38b3-c691-4051-a4dd-8bae698a577e;",
                   typeof(Analytics), typeof(Crashes));

            try
            {
                var DE = new DataExchange();
                await DE.UpdateDataFromWebApi();
            }
            catch (Exception e) { throw e; };
        }

        protected async override void OnSleep()
        {
            // Se c'è connessione e il backup automatico è attivo eseguo backup
            try
            {                
                var DB = new DataBase();

                var PR = await DB.GetPropertyAsync();
               
                if(PR != null && PR.AutomaticBackup)
                {
                    var DBS = new DropBoxLib();
                    await DBS.Authorize();

                    if (DBS.IsAuthorized)
                    {    
                        // Write the database to DropBox folder
                        //await DBS.WriteFile(File.ReadAllBytes(DB.dbPath), "/"+Path.GetFileName(DB.dbPath));
                        MemoryStream s = DependencyService.Get<IFile>().GetFileStream(DB.dbPath);
                        await DBS.WriteFile(s, "/" + Path.GetFileName(DB.dbPath));
                    }
                }
            }
            catch (Exception e) {
                throw e; };
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
