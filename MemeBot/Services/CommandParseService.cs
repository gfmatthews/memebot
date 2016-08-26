using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemeBot.Services
{
    public static class CommandParseService
    {
        private static Dictionary<String, UserIntentActivities> commandDict = new Dictionary<string, UserIntentActivities>()
        {
            {"hello", UserIntentActivities.ChitChatGreeting },
            {"hi", UserIntentActivities.ChitChatGreeting },
        };

        public static UserIntentActivities ParseIntentFromString(String term)
        {
            UserIntentActivities result = UserIntentActivities.None;
            try
            {
                commandDict.TryGetValue(term, out result);
                return result;
            }
            catch (ArgumentNullException)
            {
                return UserIntentActivities.None;
            }
        }
    }

    public enum UserIntentActivities
    {
        None,
        ChitChatGreeting,
        ChitChatHelp,
        ChitChatDismiss,
        MemeCreate,
        MemeFind

    }
}