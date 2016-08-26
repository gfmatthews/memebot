using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using MemeBot.Services;

namespace MemeBot.Dialogs
{
    [Serializable]
    public class ChitChatDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("You said: " + message.Text);
            context.Wait(MessageReceivedAsync);
        }

        public async Task SendGreeting(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            String greetingString = await ChitChatService.GreetPerson();
            await context.PostAsync(greetingString);
            context.Wait(MessageReceivedAsync);
        }

        public async Task DismissBot(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            String dismissString = await ChitChatService.DismissBot();
            await context.PostAsync(dismissString);
            context.Wait(MessageReceivedAsync);
        }

        public async Task HelpText(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            String helpString = await ChitChatService.SendHelpString();
            await context.PostAsync(helpString);
            context.Wait(MessageReceivedAsync);
        }
    }
}