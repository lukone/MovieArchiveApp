using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MovieArchive
{
    public class DropBoxLib
    {
        #region Constants

        private const string AppKeyDropboxtoken = "";

        private const string ClientId = ApiKey.DropBoxClientID;

        private const string RedirectUri = "http://127.0.0.1:52475/authorize";
        #endregion

        #region Fields

        private string oauth2State;

        #endregion

        #region Properties
        private string AccessToken { get; set; }
        public bool IsAuthorized { get; set; }
        #endregion

        #region Contructors
        public DropBoxLib()
        {           
        }

        #endregion

        #region Public Authentication

        /// <summary>
        ///     <para>Runs the Dropbox OAuth authorization process if not yet authenticated.</para>  
        /// </summary>
        /// <returns></returns>
        public async Task Authorize()
        {
            if (string.IsNullOrWhiteSpace(AccessToken) == false)
            {
                IsAuthorized = true;
                // Already authorized
                return;
            }
            if (GetAccessTokenFromSettings())
            {
                IsAuthorized = true;
                // Found token and set AccessToken 
                return;
            }
            // Run Dropbox authentication
            oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ClientId, new Uri(RedirectUri), oauth2State);
            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            await Application.Current.MainPage.Navigation.PushModalAsync(contentPage);
        }
        #endregion

        #region Dropbox function
        public async Task<IList<Metadata>> ListFiles()
        {
            try
            {
                using (var client = GetClient())
                {
                    var list = await client.Files.ListFolderAsync(string.Empty);
                    return list?.Entries;
                }
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<byte[]> ReadFile(string file)
        {
            try
            {
                using (var client = GetClient())
                {
                    var response = await client.Files.DownloadAsync(file);
                    var bytes = response?.GetContentAsByteArrayAsync();
                    return bytes?.Result;
                }
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<FileMetadata> WriteFile(byte[] fileContent, string filename)
        {
            try
            {
                var commitInfo = new CommitInfo(filename, WriteMode.Overwrite.Instance, false, DateTime.Now);

                using (var client = GetClient())
                {
                    var metadata = await client.Files.UploadAsync(commitInfo, new MemoryStream(fileContent));
                    return metadata;
                }
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<FileMetadata> WriteFile(MemoryStream mem, string filename)
        {
            try
            {
                using (var client = GetClient())
                {
                    var metadata = await client.Files.UploadAsync(filename, WriteMode.Overwrite.Instance, body: mem );
                    return metadata;
                }
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Saves the Dropbox token to app settings
        /// </summary>
        /// <param name="token">Token received from Dropbox authentication</param>
        private async Task SaveDropboxToken(string token)
        {
            if (token == null)
            {
                IsAuthorized = false;
                return;
            }

            try
            {
                Application.Current.Properties.Add(AppKeyDropboxtoken, token);
                await Application.Current.SavePropertiesAsync();
                IsAuthorized = true;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                IsAuthorized = false;
            }
        }

        private DropboxClient GetClient()
        {
            return new DropboxClient(AccessToken, new DropboxClientConfig() { HttpClient = new HttpClient(new HttpClientHandler()) });
        }

        /// <summary>
        ///     Tries to find the Dropbox token in application settings
        /// </summary>
        /// <returns>Token as string or <c>null</c></returns>
        private bool GetAccessTokenFromSettings()
        {
            try
            {
                if (!Application.Current.Properties.ContainsKey(AppKeyDropboxtoken))
                    return false;

                AccessToken = Application.Current.Properties[AppKeyDropboxtoken]?.ToString();
                if (AccessToken != null)
                    return true;

                return false;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        private async void WebViewOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!e.Url.StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                IsAuthorized = false;
                return;
            }

            try
            {
                var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

                if (result.State != oauth2State)
                {
                    IsAuthorized = false;
                    return;
                }

                AccessToken = result.AccessToken;

                await SaveDropboxToken(AccessToken);
            }
            catch (ArgumentException ex)
            {
                // There was an error in the URI passed to ParseTokenFragment
                Crashes.TrackError(ex);
            }
            finally
            {
                e.Cancel = true;
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        #endregion

    }
}