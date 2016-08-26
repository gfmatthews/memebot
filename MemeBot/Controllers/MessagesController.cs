using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using MemeBot.Dialogs;
using System.Net.Http;
using MemeBot.Services;

namespace MemeBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity message)
        {
            ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));
            // check if activity is of type message
            if (message != null && message.GetActivityType() == ActivityTypes.Message)
            {
                UserIntentActivities intent = CommandParseService.ParseIntentFromString(message.Text);

                if (intent == UserIntentActivities.ChitChatGreeting)
                {
                    await Conversation.SendAsync(message, () => new ChitChatDialog());
                }
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
    }
}