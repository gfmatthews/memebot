using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using ImgFlipAPI.APISource.Models;
using ImgFlipAPI.APISource;
using MemeBot.Services;
using MemeBot.Dialogs.InteractiveDialogConstructs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;

namespace MemeBot.Dialogs
{
    [LuisModel("d4328580-e51c-4137-8649-2247d54592d5", "cec4391143a5429b975b0e226e5aa6e1")]
    [Serializable]
    class LuisMemeIntentDialog : LuisDialog<MemeCreateOrder>
    {
        private const String ENTITY_MEME_TEXT_TOP = "meme.creation.text::toptext";
        private const String ENTITY_MEME_TEXT_BOTTOM = "meme.creation.text::bottomtext";
        private const String ENTITY_MEME_TEXT = "meme.creation.text";
        private const String ENTITY_MEME_SEARCH_TERM = "meme.search.term";
        MemeText memetext;

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"What? I don't... I don't even. Contents of my algorithimic understanding:" + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        #region [INTENT] Meme Creation
        [LuisIntent("meme.create")]
        [LuisIntent("meme.create.interactive")]
        public async Task CreateMeme(IDialogContext context, LuisResult result)
        {
            int memetype = 0;
            MemeCreateOrder order = new MemeCreateOrder();
            memetype = await ExtractMemeTypeReccomendationFromResult(result);
            memetext = await Task.Run(() => ExtractMemeTextReccomendationFromResult(result));

            // got a memetype back from the reccomender? fill it in
            if (memetype != 0)
            {
                order.memeOptions = (PopularMemeTypes)memetype;
            }
            // got text back from the reccomendation engine? fill that in too
            if ((memetext.TopText != string.Empty) || (memetext.BottomText != string.Empty))
            {
                // check if the meme text entity reccomendations came back with anything.  If the user used pre-filling syntaxt intents
                // then we should just pass those in to the order
                order.toptext = (memetext.TopText == String.Empty) ? null : memetext.TopText;
                order.bottomtext = (memetext.BottomText == String.Empty) ? null : memetext.BottomText;
            }

            // if at this point we have enough information to create the meme without prompts, we bypass 
            // the interactive conversation and just go
            if (order.HasEnoughInformationToCreateMeme)
            {
                Activity reply = context.MakeMessage() as Activity;
                await CaptionService.GenerateResultForMemeCreate(memetype, memetext.TopText, memetext.BottomText, reply);
                await context.PostAsync(reply);

                context.Wait(MessageReceived);
            }
            // otherwise, spin the converation from where we are (starting from whatever point we're at)
            else
            {
                IFormDialog<MemeCreateOrder> createOrderDialog = MakeMemeCreateInteractiveDialog(order);
                context.Call(createOrderDialog, FinalizeMemeCreation);
            }
        }

        private async Task FinalizeMemeCreation(IDialogContext context, IAwaitable<MemeCreateOrder> result)
        {
            MemeCreateOrder order;
            try
            {
                order = await result;

            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("We'll try again some other time");
                return;
            }
            Activity reply = context.MakeMessage() as Activity;
            await CaptionService.GenerateResultForMemeCreate(order, reply);
            await context.PostAsync(reply);

            context.Wait(MessageReceived);
            
        }
        #endregion

        #region [INTENT] Meme Search
        [LuisIntent("meme.search")]
        [LuisIntent("meme.search.interactive")]
        public async Task FindMeme(IDialogContext context, LuisResult result)
        {
            EntityRecommendation memesearchterm;
            if (!result.TryFindEntity(ENTITY_MEME_SEARCH_TERM, out memesearchterm))
            {
                memesearchterm = new EntityRecommendation(ENTITY_MEME_TEXT) { Entity = "" };
            }
            if (memesearchterm.Entity == "")
            {
                PromptDialog.Text(context, CompleteSearchDialog, "Got it, what meme are you looking for again?", null, 2);
            }
            else
            {
                Activity reply = context.MakeMessage() as Activity;
                await SearchService.GenerateResultForSearch(memesearchterm.Entity, reply);
                await context.PostAsync(reply);
            }
            context.Wait(MessageReceived);
        }

        private async Task CompleteSearchDialog(IDialogContext context, IAwaitable<string> result)
        {
            String searchTerm;
            try
            {
                searchTerm = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("No problem, we'll look again some other time.  I'll go back to browsing the internet.");
                return;
            }
            Activity reply = context.MakeMessage() as Activity;
            await SearchService.GenerateResultForSearch(searchTerm, reply);
            await context.PostAsync(reply);


            context.Wait(MessageReceived);
        }
        #endregion

        #region [INTENT] Chitchat 
        [LuisIntent("chitchat.greeting")]
        public async Task GreetPerson(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ChitChatService.GreetPerson());
            context.Wait(MessageReceived);
        }

        [LuisIntent("chitchat.help")]
        public async Task SendHelpString(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ChitChatService.SendHelpString());
            context.Wait(MessageReceived);
        }

        [LuisIntent("chitchat.dismiss")]
        public async Task DismissBot(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(await ChitChatService.DismissBot());
            context.Wait(MessageReceived);
        }
        #endregion

        #region Entity Extraction
        /// <summary>
        /// Returns the identifier of the memeID as found in the result during creation intents
        /// </summary>
        /// <param name="result">the LuisResult object from a creation intent</param>
        /// <returns></returns>
        private async Task<int> ExtractMemeTypeReccomendationFromResult(LuisResult result)
        {
            EntityRecommendation memetype = null;
            // for every entity key in the meme lookup dictionary, find the corresponding entities in the result
            foreach (string entity_key in CaptionService.MemeLookupDictionary.Keys)
            {
                // the amazing parallelizable foreach loop
                await Task.Run(() => result.TryFindEntity(entity_key, out memetype));
                // if we found something
                if (memetype != null)
                {
                    PopularMemeTypes memeTypeToCreate;
                    CaptionService.MemeLookupDictionary.TryGetValue(memetype.Type, out memeTypeToCreate);
                    return (int)memeTypeToCreate;
                }
            }

            // this means that nothing was found during the match for known meme IDs
            if (memetype == null)
            {
                // so we should look and see if we did ID a search term
                EntityRecommendation memesearchterm;
                if (result.TryFindEntity(ENTITY_MEME_SEARCH_TERM, out memesearchterm))
                {
                    int memeID;
                    // run a search to find the memeID to use
                    List<Meme> listOfMemes = await SearchService.GetMemeForSearchTerm(memesearchterm.Entity);

                    // TODO: have the user pick from the list
                    Meme x = listOfMemes[0];
                    
                    // parse out the id from the meme object and return that
                    int.TryParse(x.id, out memeID);
                    return memeID;
                }
            }

            // if nothing else we will just return null
            return 0;
        }

        /// <summary>
        /// Extracts the memetext from a luis result object
        /// </summary>
        /// <param name="result">the LUIS result object to inspect</param>
        /// <returns></returns>
        private MemeText ExtractMemeTextReccomendationFromResult(LuisResult result)
        {
            EntityRecommendation memetexttop;
            EntityRecommendation memetextbottom;
            MemeText returnMemeTextObject = new MemeText();

            // Look for the top text and bottom text reccomendation first
            // if both don't have values, only then look for a the entity reccomendation for generalized text
            if (!result.TryFindEntity(ENTITY_MEME_TEXT_TOP, out memetexttop))
            {
                memetexttop = new EntityRecommendation(ENTITY_MEME_TEXT) { Entity = "" };
            }
            if (!result.TryFindEntity(ENTITY_MEME_TEXT_BOTTOM, out memetextbottom))
            {
                memetextbottom = new EntityRecommendation(ENTITY_MEME_TEXT) { Entity = "" };
            }

            // only in this case do we check the general entity reccomendation field
            if ((memetextbottom.Entity == "") && (memetexttop.Entity == ""))
            {
                // if we do find something put it the bottom text field
                if (!result.TryFindEntity(ENTITY_MEME_TEXT, out memetextbottom))
                {
                    memetextbottom = new EntityRecommendation(ENTITY_MEME_TEXT) { Entity = "" };
                }
            }

            returnMemeTextObject.TopText = memetexttop.Entity;
            returnMemeTextObject.BottomText = memetextbottom.Entity;

            return returnMemeTextObject;
        }
        #endregion

        #region Interactive Form Creation
        internal IFormDialog<MemeCreateOrder> MakeMemeCreateInteractiveDialog()
        {
            FormDialog<MemeCreateOrder> memeOrderFormDialog = new FormDialog<MemeCreateOrder>(new MemeCreateOrder(), MemeCreateOrder.BuildForm, FormOptions.PromptInStart, null);
            return memeOrderFormDialog;
        }

        internal IFormDialog<MemeCreateOrder> MakeMemeCreateInteractiveDialog(MemeCreateOrder orderInProgress)
        {
            FormDialog<MemeCreateOrder> memeOrderFormDialog = new FormDialog<MemeCreateOrder>(orderInProgress, MemeCreateOrder.BuildForm, FormOptions.PromptInStart, null);
            return memeOrderFormDialog;
        }
        #endregion



        /// <summary>
        /// Used to pass around MemeText objects
        /// </summary>
        [Serializable]
        private struct MemeText
        {
            public String TopText;
            public String BottomText;
        };
    }

}
