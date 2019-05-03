using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace MovieArchive
{
    public class DataBase
    {
        SQLiteAsyncConnection cnnDBAsync;
        SQLiteConnection cnnDB;
        private readonly int DBVersion = 2;

        private string dbPath= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MovieArchive.db3");
        
        public DataBase()
        {
            try
            {
                cnnDBAsync = new SQLiteAsyncConnection(dbPath);
                cnnDB = new SQLiteConnection(dbPath);
                //Create table if not exists 
                if (!TableExists("Property"))
                {
                    InitializeDB();
                }
                else if(!DBUptodate()) //if relese of local db is different upgrade the db
                {
                    UpgradeDB();
                }
            }
            catch(Exception e)
            {       
            }
        }

        #region MovieAsync
        public Task<List<Movie>> GetMovieAsync()
        {
            try {
                return cnnDBAsync.Table<Movie>().ToListAsync();
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Task<Movie> GetMovieAsync(int id)
        {
            try { 
                return cnnDBAsync.Table<Movie>().Where(i => i.ID == id).FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Task<int> InsertMovieAsync(Movie item)
        {
            try { 
                return cnnDBAsync.InsertAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<int> InsertMoviesAsync(List<Movie> item)
        {
            try { 
                return cnnDBAsync.InsertAllAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<int> UpdateMovieAsync(Movie item)
        {
            try
            {
                return cnnDBAsync.UpdateAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<int> UpdateMoviesAsync(List<Movie> item)
        {
            try { 
                return cnnDBAsync.UpdateAllAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<int> DeleteMovieAsync(Movie item)
        {
            try{
                return cnnDBAsync.DeleteAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<List<Movie>> GetMovieByQueryAsync(string Query)
        {
            try { 
                return cnnDBAsync.QueryAsync<Movie>(Query);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region MovieSync

        public List<Movie> GetMovies()
        {
            try { 
                return cnnDB.Table<Movie>().ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int GetNextMovieID()
        {
            try {
                var ID=cnnDB.ExecuteScalar<int>("SELECT COALESCE(MAX(id)+1, 0) FROM Movie");
                if (ID == 0) { ID++; };
                return ID;
            }
            catch (Exception e)
            {
                return 1;
            }
            //return cnnDB.Table<Movie>().Max(x => x.ID);
        }

        #endregion

        #region PropertyAsync

        public Task<Property> GetPropertyAsync()
        {
            try { 
                return cnnDBAsync.Table<Property>().FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<int> InsertPropertyAsync(Property item)
        {
            try { 
                return cnnDBAsync.InsertAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<int> UpdatePropertyAsync(Property item)
        {
            try { 
                if (item.ID==0)
                    return cnnDBAsync.InsertAsync(item);
                else
                    return cnnDBAsync.UpdateAsync(item);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region DB Generic

        private void InitializeDB()
        {
            cnnDBAsync.CreateTableAsync<Property>().Wait();
            var PR = new Property();
            InsertPropertyAsync(PR);

            if (!TableExists("Movie"))
                cnnDBAsync.CreateTableAsync<Movie>().Wait();
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
            catch(Exception e)
            {
                return false;
            }
        }
        private void UpgradeDB()
        {
            if (DBVersion == 2) //first upgrade with property without dbversion field
            {
                try
                {
                    var PAV1 = cnnDBAsync.QueryAsync<PropertyV1>("Select * from property").Result;
                    cnnDB.Execute("ALTER TABLE Property RENAME TO PropertyOld");
                    cnnDB.CreateTable<Property>();
                    var PR = new Property();
                    PR.ID = PAV1[0].ID;
                    PR.AutomaticBackup = PAV1[0].AutomaticBackup;
                    PR.WebApiAddress = PAV1[0].WebApiAddress;
                    PR.GetMovieLastUpdate = PAV1[0].LastUpdate;
                    PR.GetRatingLastUpdate = PAV1[0].LastUpdate;
                    PR.DBVersion = DBVersion;
                    cnnDB.Insert(PR);
                    cnnDB.Execute("DROP TABLE PropertyOld");
                }
                catch(Exception e)
                {  }
            }
        }

        public virtual bool TableExists(string tableName)
        {
            bool TableExist=false;
            try { 
                int count = cnnDB.ExecuteScalar<int>("SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name=?", tableName);

                if (count > 0)
                    TableExist = true;
            
                return TableExist;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void ResetDB()
        {
            try { 
                if (!TableExists("Movie"))
                    cnnDBAsync.CreateTableAsync<Movie>().Wait();
                else
                {
                    cnnDBAsync.DropTableAsync<Movie>().Wait();
                    cnnDBAsync.CreateTableAsync<Movie>().Wait();
                }

                if (!TableExists("Property"))
                    cnnDBAsync.CreateTableAsync<Property>().Wait();
                else
                {
                    cnnDBAsync.DropTableAsync<Property>().Wait();
                    cnnDBAsync.CreateTableAsync<Property>().Wait();
                }
                var PR = new Property();
                InsertPropertyAsync(PR);
            }
            catch (Exception e)
            {
                
            }
        }

        public void ResetMovieDB()
        {
            try { 
                if (!TableExists("Movie"))
                    cnnDBAsync.CreateTableAsync<Movie>().Wait();
                else
                {
                    cnnDBAsync.DropTableAsync<Movie>().Wait();
                    cnnDBAsync.CreateTableAsync<Movie>().Wait();
                }
            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}
