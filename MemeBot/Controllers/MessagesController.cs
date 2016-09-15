using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using MemeBot.Dialogs;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace MemeBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        string botMentionText;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity message)
        {
            // check if activity is of type message
            if (message != null && message.GetActivityType() == ActivityTypes.Message)
            {
                // Group Conversation Case
                if ((bool)message.Conversation.IsGroup)
                {
                    bool wasBotMentioned = false;
                    await Task.Run(() => wasBotMentioned = WasBotMentionedInGroupMessage(message));

                    if (wasBotMentioned)
                    {
                        // filter out the mention words
                        message.Text = message.Text.Replace(botMentionText, "");
                        await Conversation.SendAsync(message, () => new LuisMemeIntentDialog());
                    }
                }

                // Single user conversation case
                else
                {
                    await Conversation.SendAsync(message, () => new LuisMemeIntentDialog());
                }
            }
            else
            {
                HandleSystemMessage(message);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        // TODO: When c# lets you pass tuples as return types (e.g. c# 7, do that instead of this hacky workaround)
        private bool WasBotMentionedInGroupMessage(Activity message)
        {
            string botIDOnThisChannel = message.Recipient.Id;
            IEnumerable<Mention> list = message.GetMentions().ToList().Where(mention => mention.Mentioned.Id == botIDOnThisChannel);

            if (list.Count() > 0)
            {
                this.botMentionText = list.First().Text;
                // the bot was mentioned because we have something in the list
                return true;
            }
            else
                return false;
        }
    }
}