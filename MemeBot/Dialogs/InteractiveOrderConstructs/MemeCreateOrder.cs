using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using ImgFlipAPI.APISource;
using ImgFlipAPI.APISource.Models;

namespace MemeBot.Dialogs.InteractiveOrderConstructs
{
    [Serializable]
    class MemeCreateOrder
    {
        //public PopularMemeTypes? MemeBase;
        public String Bottom;
        public String Top;

        public static IForm<MemeCreateOrder> BuildForm()
        {
            FormBuilder<MemeCreateOrder> newOrder = new FormBuilder<MemeCreateOrder>(false);
            newOrder.Message("hello");
            return newOrder.Build();
        }
    }
}