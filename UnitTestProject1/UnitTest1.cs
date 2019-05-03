using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieArchive;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string WebApiUrl = "";

        [TestMethod]
        public void TestMethod1()
        {
            Movie M = new Movie();

            M.TmdbID = 217;
            var MC = new MovieCardModel(M);
            Assert.AreEqual(MC.MovieDet.TmdbID, 217);
        }

        [TestMethod]
        public void TestResetDB()
        {
            var DB = new DataBase();
            DB.ResetMovieDB();

        }


        [TestMethod]
        public void TestGetDataFromWebApi()
        {
            var MS = new WebApi(WebApiUrl);

            List<Movie> result = MS.GetDataMovieArchiveWS(DateTime.Now.AddDays(-5));//.Result;

            Assert.AreNotEqual(result.Count, 0);
        }

        [TestMethod]
        public void TestSetDataToWebApi()
        {
            var MS = new WebApi(WebApiUrl);

            MS.SetRatingWS(2411, 4).Wait();

        }

        [TestMethod]
        public async Task TestGetLastUpdateFromWebApi()
        {
            var DB = new DataBase();
            var PY = DB.GetPropertyAsync().Result ?? new Property();
            PY.WebApiAddress = WebApiUrl;
            PY.GetMovieLastUpdate = DateTime.Now.AddDays(-5);
            int r = DB.UpdatePropertyAsync(PY).Result;

            var DE = new DataExchange();
            DE.UpdateDataFromWebApi();

            Assert.AreNotEqual(1, 0);
        }
    }
}
