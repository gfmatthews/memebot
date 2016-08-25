using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MemeBot.Services
{
    public static class ChitChatService
    {
        private static Random rndGenerator = new Random();


        public static async Task<String> GreetPerson()
        {
            List<String> PossibleGreetResponses = new List<string>()
            {
                "HI THERE! I AM THE AMAZING MEMEBOT AND I LOVE TYPING IN ALL CAPS!",
                "Wow! I've been browsing the internet all week and boy are there a lot of memes out there.",
                "It's awesome to see you!  I am back and boy have I gotten some upgrades!",
                "It's a wonderful day to sports isn't it?",
                "Greetings Meme-human, what can I do for you?",
                "Howdy! My master has been really busy tweaking me, I've gotten exposed to a lot more memes lately. What can I make for you?"
            };

            return PossibleGreetResponses[rndGenerator.Next(0, PossibleGreetResponses.Count)];

        }

        public static async Task<String> SendHelpString()
        {
            List<String> PossibleHelpResponses = new List<string>()
            {
                "You can say things like: make a meme of grumpy cat saying i tried to make a meme once it was awful",
                "Try searching for common memes by saying: find me a meme of batman",
                "Just want to see a meme? Try saying: show me doge",
                "Want a protip? Try saying: make a meme of shut up and take my money with new Surface Book? on top and shut up and take my money on the bottom"
            };

            return PossibleHelpResponses[rndGenerator.Next(0, PossibleHelpResponses.Count)];

        }

        public static async Task<String> DismissBot()
        {
            List<String> PossibleDismissResponses = new List<string>()
            {
                "Want me to stop listening? Send me a message with just the word Goodbye",
                "Am I unwanted? Send me a Goodbye to tell me to go away",
                "I get it.  You hate me.  *sniff*.  Send me a Goodbye and I'll go away... I guess...",
                "Done with me so soon? Send me a Goodbye to stop the conversation."
            };

            return PossibleDismissResponses[rndGenerator.Next(0, PossibleDismissResponses.Count)];
        }
    }
}