using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace Dsh.Commands
{
    public class FolkCommands : ApplicationCommandModule
    {
        [SlashCommand("SlavaUkrayini", "Ґрунт, База, так би мовити — Основа.")]
        public async Task DelayTestCommand(InteractionContext ctx) { await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Героям Слава!")); }


        [SlashCommand("poll", "Створити опитування")]
        public async Task PollCommand(InteractionContext ctx, [Option("Питання", "Основна тема/питання опитування")] string Question,
                                                              [Option("термін", "Час, встановлений для цього опитування")] long TimeLimit,
                                                              [Option("варіант1", "Варіант 1")] string Option1,
                                                              [Option("варіант2", "Варіант 2")] string Option2,
                                                              [Option("варіант3", "Варіант 3")] string Option3,
                                                              [Option("варіант4", "Варіант 4")] string Option4)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent("..."));

            var interactvity = ctx.Client.GetInteractivity();
            TimeSpan timer = TimeSpan.FromSeconds(TimeLimit); 

            DiscordEmoji[] optionEmojis = { DiscordEmoji.FromName(ctx.Client, ":one:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":two:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":three:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":four:", false) }; 

            string optionsString = optionEmojis[0] + " | " + Option1 + "\n" +
                                   optionEmojis[1] + " | " + Option2 + "\n" +
                                   optionEmojis[2] + " | " + Option3 + "\n" +
                                   optionEmojis[3] + " | " + Option4; 

            var pollMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Azure)
                .WithTitle(string.Join(" ", Question))
                .WithDescription(optionsString)
            ); //Making the Poll message

            var putReactOn = await ctx.Channel.SendMessageAsync(pollMessage); 

            foreach (var emoji in optionEmojis)
            {
                await putReactOn.CreateReactionAsync(emoji);
                Thread.Sleep(1000); 
            }

            var result = await interactvity.CollectReactionsAsync(putReactOn, timer); 

            int count1 = 0; 
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in result) 
            {
                if (emoji.Emoji == optionEmojis[0])
                {
                    count1++;
                }
                if (emoji.Emoji == optionEmojis[1])
                {
                    count2++;
                }
                if (emoji.Emoji == optionEmojis[2])
                {
                    count3++;
                }
                if (emoji.Emoji == optionEmojis[3])
                {
                    count4++;
                }
            }

            int totalVotes = count1 + count2 + count3 + count4;

            string resultsString = optionEmojis[0] + ": " + count1 + " Голосів \n" +
                                   optionEmojis[1] + ": " + count2 + " Голосів \n" +
                                   optionEmojis[2] + ": " + count3 + " Голосів \n" +
                                   optionEmojis[3] + ": " + count4 + " Голосів \n\n" +
                                   "Загальна кількість голосів " + totalVotes; 

            var resultsMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Green)
                .WithTitle("Результат опитування")
                .WithDescription(resultsString)
            );

            await ctx.Channel.SendMessageAsync(resultsMessage);
        }

        [SlashCommand("createEmbed", "Створити повідомлення з під заголовком")]
        public async Task CreateEmbed(InteractionContext ctx, [Option("Заголовок", "Ваш заголовок")] string title,
                                                            [Option("Повідомлення", "Текст повідомлення")] string message,
                                                            [Choice("Зелений", "green")]
                                                            [Choice("Червоний", "red")]
                                                            [Choice("Жовтий", "yellow")]
                                                            [Option("Колір", "Колір повідомлення")] string color)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent("..."));

            DiscordColor discordColor = DiscordColor.White;

            switch (color)
            {
                case "green":
                    discordColor = DiscordColor.Green;
                    break;
                case "red":
                    discordColor = DiscordColor.Red;
                    break;
                case "yellow":
                    discordColor = DiscordColor.Yellow;
                    break;
            }

            message = message.Replace("\\n", "\n");

            var builder = new DiscordMessageBuilder()
                .WithContent(title)
                .WithEmbed(new DiscordEmbedBuilder()
                    .WithDescription(message)
                    .WithColor(discordColor)
                    .Build());

            await ctx.Channel.SendMessageAsync(builder);
        }

    }
}
