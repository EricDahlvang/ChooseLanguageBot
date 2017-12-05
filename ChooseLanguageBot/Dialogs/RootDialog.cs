using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;

namespace ChooseLanguageBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (!context.PrivateConversationData.ContainsKey(LanguageChoiceDialog.LCID))
            {
                await context.Forward(new LanguageChoiceDialog(), AfterLanguageChoice, activity, CancellationToken.None);                
            }
            else
            {
                var lcid = context.PrivateConversationData.GetValueOrDefault<string>(LanguageChoiceDialog.LCID);
                
                int length = (activity.Text ?? string.Empty).Length;
                await context.PostAsync($"You sent {activity.Text} which was {length} characters.  Your chosen language code is: {lcid}");
                await context.PostAsync($"{Resources.Resource.String1}");

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task AfterLanguageChoice(IDialogContext context, IAwaitable<object> result)
        {
            var lcid = context.PrivateConversationData.GetValueOrDefault<string>(LanguageChoiceDialog.LCID);            
            await context.PostAsync($"Your chosen language code is: {lcid}");            
        }
    }
}