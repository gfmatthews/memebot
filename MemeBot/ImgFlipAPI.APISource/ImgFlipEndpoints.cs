using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgFlipAPI.APISource
{
    static public class ImgFlipEndpoints
    {
        #region API Base
        /// <summary>
        /// Returns the endpoint base used to create Imgur API calls - free or paid
        /// </summary>
        static internal String APIBase
        {
            get
            {
                return _freeEndpoint;
            }
        }

        static private String _freeEndpoint = "https://api.imgflip.com/{0}";

        #endregion

        #region Meme Routes
        static private String _getMemes = String.Format(APIBase, "get_memes");
        static private String _captionImage = String.Format(APIBase, "caption_image");

        static public String GetMemes
        {
            get
            {
                return String.Format(_getMemes);
            }
        }

        static public String CaptionImage
        {
            get
            {
                return String.Format(_captionImage);
            }
        }



        #endregion

    }

    public enum PopularMemeTypes
    {
        OneDoesNotSimply = 61579,
        DosEquisGuy = 61532,
        GrumpyCat = 405658,
        AliensGuy = 101470,
        XEverywhere = 347390,
        FuturamaFry = 61520,
        Y_U_NO = 61527,
        NedStarkBrace = 61546,
        PizzaFrenchFry = 100951,
        YoDawg = 101716,
        AmITheOnlyOneAroundHere = 259680,
        WhatIfIToldYou = 100947,
        Doge = 8072285,
        Batman=438680,
        FirstWorldProblems= 61539,
        ThatWouldBeGreat=563423,
        PicardFacepalm= 1509839,
        Oprah= 28251713,
        AintNobodyGotTime=442575,
        SuccessKid= 61544,
        GrandmaInternet= 61556,
        ItsGone= 766986,
        ConfessionBear= 100955,
        SociallyAwkwardAwesomePenguin = 61584,
        Philosoraptor= 61516,
        ClarityClarence= 100948,
        AllTheThings= 61533,
        NoPatrick = 22751625,
        SpartaLeonidas = 195389,
        SkepticalBaby = 101288,
        DontYouSquidward = 101511,
        RyanGosling = 389834,
        SpidermanComputer = 1366993,
        EmbarassedBunny = 33105543,
        DarthVader = 6742540,
        Clippy = 60759575,
        IAmDisappoint = 42752910,
        Trump = 40181531,
        Hillary = 5153844,
        Ermergerd = 7590469
    };


}
