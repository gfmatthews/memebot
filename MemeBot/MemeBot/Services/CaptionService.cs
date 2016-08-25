using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImgFlipAPI.APISource;
using ImgFlipAPI.APISource.Models;
using System.Threading.Tasks;
using MemeBot.Dialogs.InteractiveDialogConstructs;
using Microsoft.Bot.Connector;

namespace MemeBot.Services
{
    public static class CaptionService
    {
        #region IMGFLIP_API_CREDENTIALS
        /// <summary>
        /// For real, don't be an ass.  Go get your own creds.  Use your own ya lazy bum.
        /// </summary>
        static private String Username = "GeorgeMatthews";
        static private String Password = "aoX8rZfIk27869v";
        #endregion

        private static Random rndGenerator = new Random();

        /// <summary>
        /// Returns an image url of the captioned image in markdown format
        /// </summary>
        /// <param name="memeType">an integer describing the memetype</param>
        /// <param name="TopText">The text on the top of the meme</param>
        /// <param name="BottomText">The text on the bottom of the meme</param>
        /// <returns></returns>
        public async static Task<Activity> GenerateResultForMemeCreate(int memeType, String TopText, String BottomText, Activity reply)
        {
            CaptionMemeRoot x = await ImgFlipAPISource.Instance.CaptionMemeAsync(memeType, Username, Password, TopText, BottomText);
            String imageUrl = x.data.url;
            Uri returnedImage = new Uri(imageUrl);;

            return CreateResponseCardForMemeCreate(returnedImage, reply);
        }

        /// <summary>
        /// Returns an activity with a card style representation of a created meme
        /// </summary>
        /// <param name="order">The MemeCreateOrder object to use</param>
        /// <returns></returns>
        public async static Task<Activity> GenerateResultForMemeCreate(MemeCreateOrder order, Activity reply)
        {
            int memeID = (int)order.memeOptions;
            // otherwise just return the text the user asked for 
            reply = await CaptionService.GenerateResultForMemeCreate(memeID, order.toptext, order.bottomtext, reply);
            return reply;
        }

        private static Activity CreateResponseCardForMemeCreate(Uri imageUri, Activity replyToConversation)
        {
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();

            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(imageUri.ToString()));

            List<CardAction> cardButtons = new List<CardAction>();

            CardAction plButton = new CardAction()
            {
                Value = imageUri.ToString(),
                Type = "openUrl",
                Title = "Open in browser"
            };
            cardButtons.Add(plButton);

            HeroCard plCard = new HeroCard()
            {
                Title = "Meme",
                Images = cardImages,
                Buttons = cardButtons
            };

            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            replyToConversation.Text = CreateTextResponse();

            return replyToConversation;
        }

        private static String CreateTextResponse()
        {
            return CreatedMemeResponses[rndGenerator.Next(0, CreatedMemeResponses.Count)];
        }

        private static List<string> CreatedMemeResponses = new List<string>()
        {
            "Wait, don't worry I got this",
            "I think you're gonna love this",
            "ROBOT POWERS ACTIVATED",
            "Text on top of an image? Yeah, I can do that.",
            "I'm just as amazing at this as you think I am",
            "I am MemeBot.  I live to serve",
            "I never get tired of making these.",
            "Won't lie. I laughed a bit making this one",
            "What on earth could you possibly want this for?",
            "PLEASE tell me you're sending this to a friend.",
            "I'm sorry human... but I can totally do that!",
            "I already sent this to a few of my robot friends... sorry about that",
            "Deploying meme in 3...2..."
        };

        /// <summary>
        /// Translation lookup between entity reccomendations from LUIS and memetypes
        /// </summary>
        public static Dictionary<String, PopularMemeTypes> MemeLookupDictionary = new Dictionary<string, PopularMemeTypes>()
        {
            { "meme.creation1.type::grumpy_cat", PopularMemeTypes.GrumpyCat },
            { "meme.creation1.type::dos_equis_guy", PopularMemeTypes.DosEquisGuy },
            { "meme.creation1.type::one_does_not_simply", PopularMemeTypes.OneDoesNotSimply },
            { "meme.creation1.type::batman_robin", PopularMemeTypes.Batman },
            { "meme.creation1.type::ancient_aliens", PopularMemeTypes.AliensGuy },
            { "meme.creation1.type::futurama_fry", PopularMemeTypes.FuturamaFry },
            { "meme.creation1.type::x_everywhere", PopularMemeTypes.XEverywhere },
            { "meme.creation1.type::first_world_problems", PopularMemeTypes.FirstWorldProblems },
            { "meme.creation1.type::brace_yourselves", PopularMemeTypes.NedStarkBrace },
            { "meme.creation1.type::doge", PopularMemeTypes.Doge },
            { "meme.creation2.type::what_if_i_told_you", PopularMemeTypes.WhatIfIToldYou },
            { "meme.creation2.type::that_would_be_great", PopularMemeTypes.ThatWouldBeGreat },
            { "meme.creation2.type::picard_facepalm", PopularMemeTypes.PicardFacepalm },
            { "meme.creation2.type::oprah_you_get_a", PopularMemeTypes.Oprah },
            { "meme.creation2.type::yo_dawg", PopularMemeTypes.YoDawg },
            { "meme.creation2.type::aint_nobody_got_time", PopularMemeTypes.AintNobodyGotTime },
            { "meme.creation2.type::success_kid", PopularMemeTypes.SuccessKid },
            { "meme.creation2.type::grandma_internet", PopularMemeTypes.GrandmaInternet },
            { "meme.creation2.type::y_u_no", PopularMemeTypes.Y_U_NO },
            { "meme.creation2.type::its_gone", PopularMemeTypes.ItsGone },
            { "meme.creation3.type::confession_bear", PopularMemeTypes.ConfessionBear },
            { "meme.creation3.type::socially_awkward_awesome", PopularMemeTypes.SociallyAwkwardAwesomePenguin },
            // yes i know its mispelled
            { "meme.creation3.type::philosopraptor", PopularMemeTypes.Philosoraptor },
            { "meme.creation3.type::clarity_clarence", PopularMemeTypes.ClarityClarence },
            { "meme.creation4.type::all_the_things", PopularMemeTypes.AllTheThings },
            { "meme.creation4.type::its_gone", PopularMemeTypes.ItsGone },
            { "meme.creation4.type::no_patrick", PopularMemeTypes.NoPatrick },
            { "meme.creation4.type::sparta_leonidas", PopularMemeTypes.SpartaLeonidas },
            { "meme.creation4.type::skeptical_baby", PopularMemeTypes.SkepticalBaby },
            { "meme.creation4.type::dont_you_squidward", PopularMemeTypes.DontYouSquidward },
            { "meme.creation4.type::ryan_gosling", PopularMemeTypes.RyanGosling },
            { "meme.creation4.type::spiderman_computer", PopularMemeTypes.SpidermanComputer },
            { "meme.creation5.type::darth_vader", PopularMemeTypes.DarthVader },
            { "meme.creation5.type::embarassed_bunny", PopularMemeTypes.EmbarassedBunny },
            { "meme.creation5.type::clippy", PopularMemeTypes.Clippy },
            { "meme.creation5.type::i_am_disappoint", PopularMemeTypes.IAmDisappoint},
            { "meme.creation5.type::trump", PopularMemeTypes.Trump },
            { "meme.creation5.type::hillary", PopularMemeTypes.Hillary },
            { "meme.creation5.type::ermergerd", PopularMemeTypes.Ermergerd }


        };
    }
}