using Microsoft.AppCenter.Crashes;
using MovieArchive.Resources;
using System;
using System.IO;
using System.Text;
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
            if(PY.AutomaticBackup)
            {
                var DBS = new DropBoxLib();
                await DBS.Authorize();
                if (DBS.IsAuthorized)
                    DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageDropBoxConnected);
                else
                    DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageDropBoxNotConnected);
            }
        }

        async void ResetTotalDB_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert(AppResources.MessageConfirmResetDB, AppResources.TitleConfirmResetDB, AppResources.ButtonConfirmResetDBReset, AppResources.ButtonConfirmResetDBCancel))
            { 
                    await DB.ResetDB();
                    DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageTotalDBReset);
            }
        }

        async void ResetMovieDB_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert(AppResources.MessageConfirmResetDB, AppResources.TitleConfirmResetDB, AppResources.ButtonConfirmResetDBReset, AppResources.ButtonConfirmResetDBCancel))
            {
                await DB.ResetMovieDB();

                PY.GetMovieLastUpdate = DateTime.MinValue;
                PY.GetRatingLastUpdate = DateTime.MinValue;
                int r = await DB.UpdatePropertyAsync(PY);
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageMovieDBReset);
            }
        }

        async void WebApiAddress_Completed(object sender, EventArgs e)
        {
            if (WebApi.IsActive(((Xamarin.Forms.Editor)sender).Text))
            {
                PY.WebApiAddress = ((Xamarin.Forms.Editor)sender).Text;
                int r = await DB.UpdatePropertyAsync(PY);
            }
            else if(((Xamarin.Forms.Editor)sender).Text!="") 
            {
                WebApiAddress.Text = "";
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageWebApiNotActive);
            }
            else
            {
                int r = await DB.UpdatePropertyAsync(PY);
            }
        }

        async void ResetTvShowDB_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert(AppResources.MessageConfirmResetDB, AppResources.TitleConfirmResetDB, AppResources.ButtonConfirmResetDBReset, AppResources.ButtonConfirmResetDBCancel))
            {
                await DB.ResetTvShowDB();

                PY.GetMovieLastUpdate = DateTime.MinValue;
                PY.GetRatingLastUpdate = DateTime.MinValue;
                int r = await DB.UpdatePropertyAsync(PY);
                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageMovieDBReset);
            }
        }

        //async void SendMailWithDB_Clicked(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //    //    ExperimentalFeatures.Enable(ExperimentalFeatures.EmailAttachments); //, ExperimentalFeatures.ShareFileRequest

        //    //    var message = new EmailMessage
        //    //    {
        //    //        Subject = "db",
        //    //        Body = "db backup",
        //    //        To = { "donluca75@gmail.com" }

        //    //    };

        //    //    message.Attachments.Add(new EmailAttachment(FileBase(DB.dbPath));

        //    //    await Email.ComposeAsync(message);
        //    ExperimentalFeatures.Enable(ExperimentalFeatures.EmailAttachments, ExperimentalFeatures.ShareFileRequest);

        //    await Share.RequestAsync(new ShareFileRequest
        //    {
        //        Title = Title,
        //        File = new ShareFile(DB.dbPath)
        //    });

        //    //}
        //    //catch (FeatureNotSupportedException fbsEx)
        //    //{
        //    //    // Email is not supported on this device
        //    //    DependencyService.Get<IMessage>().ShortAlert("Email is not supported on this device");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    // Some other exception occurred
        //    //    DependencyService.Get<IMessage>().ShortAlert(ex.Message);
        //    //}
        //}

        //private void DownloadBackup_Clicked(object sender, EventArgs e)
        //{        
        //    try
        //    {  
        //        //if automatic backup is activate copy DB in backup file
        //        if (PY.AutomaticBackup)
        //        {
        //            if(File.Exists(DB.dbPathBkp))
        //                File.Copy(DB.dbPathBkp, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Backup", "MovieArchive.db3"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private async void RestoreDB_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await DisplayAlert(AppResources.MessageConfirmRestoreDB, AppResources.TitleConfirmRestoreDB, AppResources.ButtonConfirmRestore, AppResources.ButtonConfirmCancelRestore))
                {
                    
                        var DBS = new DropBoxLib();
                        await DBS.Authorize();
                        if (DBS.IsAuthorized)
                        {
                            var db = await DBS.ReadFile("/" + Path.GetFileName(DB.dbPath));//"/Database.db3"

                            if (db != null)
                            {
                                File.WriteAllBytes(DB.dbPath, db);
                                DependencyService.Get<IMessage>().ShortAlert(AppResources.MessageDataBaseRestored);
                            }
                        }

                }
            }
            catch (Exception ex) 
            {
                Crashes.TrackError(ex); 
                throw ex; 
            };
        }

        //private async void AutoBkp_OnChanged(object sender, ToggledEventArgs e)
        //{
        //    PY.AutomaticBackup = e.Value;
        //    int r = await DB.UpdatePropertyAsync(PY);
        //}

    }
}