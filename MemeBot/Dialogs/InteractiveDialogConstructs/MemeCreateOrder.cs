using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using ImgFlipAPI.APISource;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using MemeBot.Services;
using Microsoft.Bot.Connector;

namespace MemeBot.Dialogs.InteractiveDialogConstructs
{
    [Serializable]
    public class MemeCreateOrder
    {
        [Prompt("Got it, we can make a meme together, what's the base image?  Just enter a common meme type like Doge or Grumpy Cat.",
                "What's the base of the meme?",
                "On it.  What type of meme are we making?",
                "What's the image base?")]
        public PopularMemeTypes? memeOptions = null;
        [Prompt("Okay, and the text you want on the top?",
                "The text you want on the top of the image? By the way, you just type c if you want to skip this part",
                "Image text on the top?",
                "What's on the top of the image?")]
        public String toptext = null;
        [Prompt("Last, what about on the bottom?",
                "The text on the bottom part of the image?",
                "Image text on the bottom?",
                "Anything on the bottom? Beteedubs, you can just type c to skip this step")]
        public String bottomtext = null;

        public static IForm<MemeCreateOrder> BuildForm()
        {
            return new FormBuilder<MemeCreateOrder>()
                .Field(nameof(MemeCreateOrder.memeOptions))
                .Field(nameof(MemeCreateOrder.toptext))
                .Field(nameof(MemeCreateOrder.bottomtext))
                .Build();
        }

        /// <summary>
        /// Returns true if the order contains enough information to create a meme without additional prompts, false otherwise.
        /// </summary>
        public bool HasEnoughInformationToCreateMeme
        {
            get
            {
                // if a meme has been set
                if (memeOptions != null)
                {
                    // and in addition to that the top text or bottom text fields have been set
                    if( toptext!=null || bottomtext!=null )
                    {
                        // we have enough information
                        return true;
                    }
                }
                // otherwise we don't
                return false;
            }
            private set
            {
                // literally, literally do absolutely nothing
            }
        }

    }
}