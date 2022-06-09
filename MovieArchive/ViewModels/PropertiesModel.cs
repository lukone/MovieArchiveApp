using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MovieArchive.GoogleDrive;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MovieArchive.ViewModels
{

    //https://stackoverflow.com/questions/65345646/using-google-drive-api-with-xamarinforms
    //https://github.com/stevenchang0529/XamarinGoogleDriveRest
    public class PropertiesModel : INotifyPropertyChanged
    {
        private string scope = "https://www.googleapis.com/auth/drive.file";
        private string clientId = "668725292936-ioshrk805lcu08t6um7dh67svtvp0td6.apps.googleusercontent.com";
        private string redirectUrl = "com.lk.MovieArchive1:/oauth2redirect";

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OnGoogleDrive { get; set; }

        public PropertiesModel()
        {
            //var token = SecureStorage.GetAsync("GDriveToken");

            var auth = new OAuth2Authenticator(
             this.clientId,
             string.Empty,
             scope,
             new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
             new Uri(redirectUrl),
             new Uri("https://www.googleapis.com/oauth2/v4/token"),
             isUsingNativeUI: true);
            AuthenticatorHelper.OAuth2Authenticator = auth;
            auth.Completed += async (sender, e) =>
            {
                if (e.IsAuthenticated)
                {

                    var initializer = new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets()
                        {
                            ClientId = clientId,
                        }
                    };
                    initializer.Scopes = new[] { scope };
                    initializer.DataStore = new FileDataStore("Google.Apis.Auth");
                    var flow = new GoogleAuthorizationCodeFlow(initializer);
                    var user = "DriveBkp";
                    var token = new TokenResponse()
                    {
                        AccessToken = e.Account.Properties["access_token"],
                        ExpiresInSeconds = Convert.ToInt64(e.Account.Properties["expires_in"]),
                        RefreshToken = e.Account.Properties["refresh_token"],
                        Scope = e.Account.Properties["scope"],
                        TokenType = e.Account.Properties["token_type"]
                    };

                    //await SecureStorage.SetAsync("GDriveToken", token.AccessToken);

                    UserCredential userCredential = new UserCredential(flow, user, token);
                    var driveService = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = userCredential,
                        ApplicationName = "MovieArchive",
                    });

                    //test google drive
                    DriveServiceHelper helper = new DriveServiceHelper(driveService);
                    var id = await helper.CreateFile();
                    await helper.SaveFile(id, "test.txt", "test save content");
                    var content = await helper.ReadFile(id);

                }
            };

            auth.Error += (sender, e) =>
            {

            };

            this.OnGoogleDrive = new Command(() =>
            {
                var presenter = new OAuthLoginPresenter();
                presenter.Login(auth);
            });
        }

    }

    public static class AuthenticatorHelper
    {
        public static OAuth2Authenticator OAuth2Authenticator { get; set; }
    }
}