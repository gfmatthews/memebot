using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImgFlipAPI.APISource.Models;
using ImgFlipAPI.APISource;
using System.Collections.Generic;

namespace ImgFlipAPITests
{
    [TestClass]
    public class GeneralEndpointTests
    {
        private static String username = "GeorgeMatthews";
        private static String password = "aoX8rZfIk27869v";

        [TestInitialize]
        public void StartupTests()
        {

        }

        [TestMethod]
        public void GetMemesReturnsList()
        {
            GetMemeRoot x = ImgFlipAPISource.Instance.GetMemesAsync().Result;
            List<Meme> xList = x.data.memes;
            Assert.IsTrue(xList.Count > 99);
        }

        [TestMethod]
        public void CaptionMeme()
        {
            string template_id = "0";
            string toptext = "hello";
            string bottomtext = "it's me";

            GetMemeRoot x = ImgFlipAPISource.Instance.GetMemesAsync().Result;
            List<Meme> xList = x.data.memes;
            template_id = xList[0].id;

            CaptionMemeRoot z = ImgFlipAPISource.Instance.CaptionMemeAsync(template_id, username, password, toptext, bottomtext).Result;
            Assert.IsNotNull(z.data.url);
        }

        [TestMethod]
        public void MatchingMemeTest()
        {
            string knownDogeUri = "http://i.imgflip.com/4t0m5.jpg";

            Uri dogeURI = ImgFlipAPISource.Instance.GetMemeBaseImage(PopularMemeTypes.Doge).Result;
            Assert.IsTrue(dogeURI.ToString() == knownDogeUri);
        }

        [TestMethod]
        public void FindMemesTest()
        {
            // this should return grumpy cat
            Meme testMeme = ImgFlipAPISource.Instance.FindMeme("cat").Result[0];

            int id;
            Int32.TryParse(testMeme.id, out id);

            // should match up to grumpy cats id
            Assert.IsTrue(id == (int)PopularMemeTypes.GrumpyCat);
        }

        [TestMethod]
        public void FindMemesTestPlural()
        {
            // this should return a list
            List<Meme> testMeme = ImgFlipAPISource.Instance.FindMeme("dogs").Result;

            // should be a result list
            Assert.IsTrue(testMeme.Count > 0);
        }
    }
}