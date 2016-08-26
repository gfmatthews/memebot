using ImgFlipAPI.APISource.Models;
using ImgFlipAPI.APISource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;

namespace MemeBot.Services
{
    public static class SearchService
    {
        private static Random rndGenerator = new Random();

        public async static Task<List<Meme>> GetMemeForSearchTerm(string searchTerm)
        {
            return await ImgFlipAPISource.Instance.FindMeme(searchTerm);
        }

        public async static Task<Activity> GenerateResultForSearch(string searchTerm, Activity replyToConversation)
        {
            List<Meme> x = await GetMemeForSearchTerm(searchTerm);
            replyToConversation = CreateResponseCardForSearch(x, replyToConversation);
            return replyToConversation;
        }

        private static Activity CreateResponseCardForSearch(List<Meme> memelist, Activity start)
        {
            Activity replyToConversation = start;
            replyToConversation.Attachments = new List<Attachment>();

            foreach (Meme x in memelist)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(x.url));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = x.url,
                    Type = "openUrl",
                    Title = x.name
                };
                cardButtons.Add(plButton);

                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = x.name,
                    Subtitle = x.url,
                    Images = cardImages,
                    Buttons = cardButtons
                };
                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }
            replyToConversation.AttachmentLayout = "carousel";

            if (memelist.Count == 0)
            {
                replyToConversation.Text = CreateTextResponse(true);
            }
            else
            {
                replyToConversation.Text = CreateTextResponse(false);
            }

            return replyToConversation;
        }

        private static String CreateTextResponse(bool useNegative)
        {
            if (useNegative)
            {
                return NegativeSearchResponses[rndGenerator.Next(0, NegativeSearchResponses.Count)];
            }
            else
            {
                return PositiveSearchResponses[rndGenerator.Next(0, PositiveSearchResponses.Count)];
            }
        }

        private static List<string> PositiveSearchResponses = new List<string>()
        {
            "I FOUND ALL THE THINGS!",
            "my sources tell me this is what you want",
            "that'll do human, that'll do",
            "Boom",
            "I'm like, literally the best at this",
            "Reddit said this is what you wanted and Reddit never lies"
        };

        private static List<string> NegativeSearchResponses = new List<string>()
        {
            "got nothing bro",
            "y u no search something else?",
            "i didn't find any of the things", 
            "umm... this is embarassing...",
            "do... do humans like that kind of thing? I couldn't find it",
            "i wish i was human so i better express remorse over not finding what you wanted",
            "my algorithms are burning inside... couldn't find anything"
        };

    }
}