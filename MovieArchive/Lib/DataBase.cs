using Microsoft.AppCenter.Crashes;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//--------------------------------------------
// https://www.litedb.org/  no sql per xamarin
//--------------------------------------------
namespace MovieArchive
{
    public class DataBase
    {
        private SQLiteAsyncConnection cnnDBAsync;
        private SQLiteConnection cnnDB;

        private readonly int DBVersion = 4;

        public string dbPath= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MovieArchive.db3");
        //public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MovieArchive.db3");

        public DataBase()
        {
            try
            {
                cnnDBAsync = new SQLiteAsyncConnection(dbPath);
                
                if (!TableExists("Property"))
                {
#pragma warning disable CS4014 // Non è possibile attendere la chiamata, pertanto l'esecuzione del metodo corrente continuerà prima del completamento della chiamata. Provare ad applicare l'operatore 'await' al risultato della chiamata.
                    InitializeDB();
#pragma warning restore CS4014 // Non è possibile attendere la chiamata, pertanto l'esecuzione del metodo corrente continuerà prima del completamento della chiamata. Provare ad applicare l'operatore 'await' al risultato della chiamata.
                }
                else if(!DBUptodate()) //if relese of local db is different upgrade the db
                {
#pragma warning disable CS4014 // Non è possibile attendere la chiamata, pertanto l'esecuzione del metodo corrente continuerà prima del completamento della chiamata. Provare ad applicare l'operatore 'await' al risultato della chiamata.
                    UpgradeDB();
#pragma warning restore CS4014 // Non è possibile attendere la chiamata, pertanto l'esecuzione del metodo corrente continuerà prima del completamento della chiamata. Provare ad applicare l'operatore 'await' al risultato della chiamata.
                }

            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #region MovieAsync
        public async Task<List<Movie>> GetMovieAsync()
        {
            try {
                return await cnnDBAsync.Table<Movie>().ToListAsync();
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            try { 
                return await cnnDBAsync.Table<Movie>().Where(i => i.ID == id).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> InsertMovieAsync(Movie item)
        {
            try { 
                return await cnnDBAsync.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> InsertMoviesAsync(List<Movie> item)
        {
            try { 
                return await cnnDBAsync.InsertAllAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> UpdateMovieAsync(Movie item)
        {
            try
            {
                return await cnnDBAsync.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> UpdateMoviesAsync(List<Movie> item)
        {
            try { 
                return await cnnDBAsync.UpdateAllAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> DeleteMovieAsync(Movie item)
        {
            try{
                return await cnnDBAsync.DeleteAsync(item);
            }
            catch (Exception ex )
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<List<Movie>> GetMovieByQueryAsync(string Query)
        {
            try { 
                return await cnnDBAsync.QueryAsync<Movie>(Query);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> GetNextMovieIDAsync()
        {
            try
            {
                var ID = await cnnDBAsync.ExecuteScalarAsync<int>("SELECT COALESCE(MAX(id)+1, 0) FROM Movie");
                if (ID == 0) { ID++; };
                return ID;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 1;
            }
        }

        #endregion

        #region TvShow
        public async Task<List<TvShow>> GetTvShowAsync()
        {
            try
            {
                return await cnnDBAsync.Table<TvShow>().ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<TvShow> GetTvShowAsync(int id)
        {
            try
            {
                return await cnnDBAsync.Table<TvShow>().Where(i => i.ID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> InsertTvShowAsync(TvShow item)
        {
            try
            {
                return await cnnDBAsync.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> InsertTvShowsAsync(List<TvShow> item)
        {
            try
            {
                return await cnnDBAsync.InsertAllAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> UpdateTvShowAsync(TvShow item)
        {
            try
            {
                return await cnnDBAsync.UpdateAsync(item);
            }
#pragma warning disable CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> GetNextTvShowIDAsync()
        {
            try
            {
                var ID = await cnnDBAsync.ExecuteScalarAsync<int>("SELECT COALESCE(MAX(id)+1, 0) FROM TvShow");
                if (ID == 0) { ID++; };
                return ID;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 1;
            }
        }

        public async Task<List<TvShow>> GetTvShowByQueryAsync(string Query)
        {
            try
            {
                return await cnnDBAsync.QueryAsync<TvShow>(Query);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        #endregion

        #region Season
        public async Task<List<Season>> GetSeasonsAsync(int tmdbid)
        {
            try
            {
                return await cnnDBAsync.Table<Season>().Where(i => i.TmdbID == tmdbid).ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<Season> GetSeasonAsync(int id)
        {
            try
            {
                return await cnnDBAsync.Table<Season>().Where(i => i.ID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> InsertSeasonAsync(Season item)
        {
            try
            {
                return await cnnDBAsync.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> InsertSeasonsAsync(List<Season> item)
        {
            try
            {
                return await cnnDBAsync.InsertAllAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> UpdateSeasonAsync(Season item)
        {
            try
            {
                return await cnnDBAsync.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }
        #endregion

        #region Espisode
        public async Task<List<Episode>> GetEpisodeAsync(int tmdbid,int SeasonN)
        {
            try
            {
                //List<Episode> c = new List<Episode>();
                //c=await cnnDBAsync.Table<Episode>().ToListAsync();
                //return c.Where(i => i.TmdbID == tmdbid && i.SeasonN == SeasonN).ToList();

                return await cnnDBAsync.Table<Episode>().Where(i => i.TmdbID == tmdbid).Where(x=> x.SeasonN == SeasonN).OrderBy(o=> o.N).ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }
        public async Task<Episode> GetEpisodeAsyncByID(int id)
        {
            try
            {
                return await cnnDBAsync.Table<Episode>().Where(i => i.ID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<List<Episode>> GetAllEpisodesAsync(int tmdbid)
        {
            try
            {
                List<Episode> c = new List<Episode>();
                c = await cnnDBAsync.Table<Episode>().ToListAsync();
                return c.Where(i => i.TmdbID == tmdbid).ToList();
            }
#pragma warning disable CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'e' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> InsertEpisodeAsync(Episode item)
        {
            try
            {
                return await cnnDBAsync.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }
        public async Task<int> InsertEpisodesAsync(List<Episode> item)
        {
            try
            {
                return await cnnDBAsync.InsertAllAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> UpdateEpisodeAsync(Episode item)
        {
            try
            {
                return await cnnDBAsync.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<List<Episode>> GetEpisodeByQueryAsync(string Query)
        {
            try
            {
                return await cnnDBAsync.QueryAsync<Episode>(Query);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> GetNextEpisodeIDAsync()
        {
            try
            {
                var ID = await cnnDBAsync.ExecuteScalarAsync<int>("SELECT COALESCE(MAX(id)+1, 0) FROM Episode");
                if (ID == 0) { ID++; };
                return ID;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 1;
            }
        }

        #endregion

        #region PropertyAsync

        public Task<Property> GetPropertyAsync()
        {
            try {
                return cnnDBAsync.Table<Property>().FirstOrDefaultAsync();
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<int> InsertPropertyAsync(Property item)
        {
            try { 
                return await cnnDBAsync.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public async Task<int> UpdatePropertyAsync(Property item)
        {
            try { 
                if (item.ID==0)
                    return await cnnDBAsync.InsertAsync(item);
                else
                    return await cnnDBAsync.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        #endregion

        #region DB Generic

        private void InitializeDB()
        {
            try
            {
                cnnDB = new SQLiteConnection(dbPath);
                var res = cnnDB.CreateTable<Property>();
                var PR = new Property();
                PR.DBVersion = DBVersion;
                PR.AutomaticBackup = false;
                var ris = cnnDB.Insert(PR);          
                res = cnnDB.CreateTable<Movie>();
                res = cnnDB.CreateTable<TvShow>();
                res = cnnDB.CreateTable<Season>();
                res = cnnDB.CreateTable<Episode>();
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch (Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                throw;
            }
        }
        
        private bool DBUptodate()
        {
            var PR = new Property();
            try
            {
                PR = GetPropertyAsync().Result;
                if (PR.DBVersion == DBVersion)
                    return true;
                else
                    return false;
            }
#pragma warning disable CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            catch(Exception ex)
#pragma warning restore CS0168 // La variabile 'ex' è dichiarata, ma non viene mai usata
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        private async Task UpgradeDB()
        {
            if (DBVersion == 2) //first upgrade with property without dbversion field
            {
                try
                {
                    var PAV1 = cnnDBAsync.QueryAsync<PropertyV1>("Select * from property").Result;
                    await cnnDBAsync.ExecuteAsync("ALTER TABLE Property RENAME TO PropertyOld");
                    await cnnDBAsync.CreateTableAsync<Property>();
                    var PR = new Property();
                    PR.ID = PAV1[0].ID;
                    PR.AutomaticBackup = PAV1[0].AutomaticBackup;
                    PR.WebApiAddress = PAV1[0].WebApiAddress;
                    PR.GetMovieLastUpdate = PAV1[0].LastUpdate;
                    PR.GetRatingLastUpdate = PAV1[0].LastUpdate;
                    PR.DBVersion = DBVersion;
                    await cnnDBAsync.InsertAsync(PR);
                    await cnnDBAsync.ExecuteAsync("DROP TABLE PropertyOld");
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            else if (DBVersion == 3) //New table TvShow and Season
            {
                await cnnDBAsync.CreateTableAsync<TvShow>();
                var PR = GetPropertyAsync().Result;
                PR.DBVersion = DBVersion;
                await UpdatePropertyAsync(PR);
            }
            else if (DBVersion == 4) //New table TvShow and Season
            {
                if(!TableExists("TvShow"))
                    await cnnDBAsync.CreateTableAsync<TvShow>();
                if (!TableExists("Season"))
                    await cnnDBAsync.CreateTableAsync<Season>();
                if (!TableExists("Episode"))
                    await cnnDBAsync.CreateTableAsync<Episode>();
                var PR = GetPropertyAsync().Result;
                PR.DBVersion = DBVersion;
                await UpdatePropertyAsync(PR);
            }
        }

        public bool TableExists(string tableName)
        {
            bool TableExist=false;
            try { 
                int count = cnnDBAsync.ExecuteScalarAsync<int>("SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name=?", tableName).Result;

                if (count > 0)
                    TableExist = true;
            
                return TableExist;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public async Task ResetDB()
        {
            try {
                await ResetMovieDB();

                await ResetTvShowDB();

                if (!TableExists("Property"))
                    await cnnDBAsync.CreateTableAsync<Property>();
                else
                {
                    await cnnDBAsync.DropTableAsync<Property>();
                    await cnnDBAsync.CreateTableAsync<Property>();
                }
                var PR = new Property();
                await InsertPropertyAsync(PR);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw;
            }
        }

        public async Task ResetMovieDB()
        {
            try { 
                if (!TableExists("Movie"))
                    await cnnDBAsync.CreateTableAsync<Movie>();
                else
                {
                    await cnnDBAsync.DropTableAsync<Movie>();
                    await cnnDBAsync.CreateTableAsync<Movie>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw;
            }
        }

        public async Task ResetTvShowDB()
        {
            try
            {
                if (!TableExists("TvShow"))
                    await cnnDBAsync.CreateTableAsync<TvShow>();                    
                else
                {
                    await cnnDBAsync.DropTableAsync<TvShow>();
                    await cnnDBAsync.CreateTableAsync<TvShow>();
                }

                if (!TableExists("Season"))
                    await cnnDBAsync.CreateTableAsync<Season>();
                else
                {
                    await cnnDBAsync.DropTableAsync<Season>();
                    await cnnDBAsync.CreateTableAsync<Season>();
                }

                if (!TableExists("Episode"))
                    await cnnDBAsync.CreateTableAsync<Episode>();
                else
                {                 
                    await cnnDBAsync.DropTableAsync<Episode>();
                    await cnnDBAsync.CreateTableAsync<Episode>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw;
            }
        }

        public async Task DisconnectDB()
        {
            await cnnDBAsync.CloseAsync();
        }
        #endregion

    }
}
