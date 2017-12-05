using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace ChooseLanguageBot.Dialogs
{
    [Serializable]
    public class LanguageChoiceDialog : IDialog<object>
    {
        public const string LCID = "LCID";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (!context.PrivateConversationData.ContainsKey(LCID))
            {
                PromptDialog.Choice(context, this.OnLanguageSelected, Option.CreateListOption(), "Please choose a language.", "Not a valid language", 3);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task OnLanguageSelected(IDialogContext context, IAwaitable<Option> result)
        {
            try
            {

                Option optionSelected = await result;

                switch (optionSelected.Text)
                {
                    case "French":
                    case "US English":
                    case "Ukrainian":
                    case "German":
                        context.PrivateConversationData.SetValue(LCID, optionSelected.Locale);
                        context.Done(optionSelected);
                        break;
                    default:
                        context.Wait(MessageReceivedAsync);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps. Please choose a language!");
                context.Wait(this.MessageReceivedAsync);
            }
        }
        [Serializable]

        public class Option
        {
            public string Locale { get; set; }
            public string Text { get; set; }
            public Option()
            {
                Locale = "";
                Text = "";
            }

            public override string ToString()
            {
                return this.Text;
            }
            public static List<Option> CreateListOption()
            {
                List<Option> list = new List<Option>();

                var english = new Option();
                english.Locale = "en-US";
                english.Text = "US English";
                var ukrainian = new Option();
                ukrainian.Locale = "uk-UA";
                ukrainian.Text = "Ukrainian";
                var french = new Option();
                french.Locale = "fr-FR";
                french.Text = "French";
                var german = new Option();
                german.Locale = "de-DE";
                german.Text = "German";

                list.Add(english);
                list.Add(ukrainian);
                list.Add(french);
                list.Add(german);
                return list;
            }
        }
    }
}