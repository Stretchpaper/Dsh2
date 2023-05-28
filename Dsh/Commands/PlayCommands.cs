using Dsh.Extennal_Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;

namespace Dsh.Commands
{
    public class PlayCommands : ApplicationCommandModule
    {
        [SlashCommand("flip", "Кинути монетку")]
        public async Task flipMon(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));

            var coin = new Random().Next(2) == 0 ? "Герб" : "Копійка";

            var cop = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                    .WithColor(DiscordColor.Gold)
                    .WithTitle("Результат")
                    .WithDescription($"Випало: **{coin}**")
                    .WithImageUrl("https://i.postimg.cc/KzRB2fRx/image.png")
                );
            var ger = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                    .WithColor(DiscordColor.Gold)
                    .WithTitle("Результат")
                    .WithDescription($"Випало: **{coin}**")
                    .WithImageUrl("https://i.postimg.cc/90XGk2Zg/50-kopiyok-Ukraine-removebg-preview.png")
                );
            switch (coin)
            {
                case "Герб":
                    await ctx.Channel.SendMessageAsync(ger);
                    break;
                case "Копійка":
                    await ctx.Channel.SendMessageAsync(cop);
                    break;
            }
        }

        [SlashCommand("rockpaperscissors", "Гра камінь ножиці папір")]
        public async Task rockpaperscissorscom(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));
            DiscordEmoji[] elementsEmojis = { DiscordEmoji.FromName(ctx.Client, ":scissors:", false),
                                              DiscordEmoji.FromName(ctx.Client, ":newspaper:", false),
                                              DiscordEmoji.FromName(ctx.Client, ":new_moon:", false)};
            var myButton = new DiscordButtonComponent(ButtonStyle.Secondary, "1", elementsEmojis[0]);
            var myButton1 = new DiscordButtonComponent(ButtonStyle.Success, "2", elementsEmojis[1]);
            var myButton2 = new DiscordButtonComponent(ButtonStyle.Primary, "3", elementsEmojis[2]);
            var emb = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                    .WithColor(DiscordColor.White)
                    .WithTitle("Виберіть")
                    .WithDescription($"Камінь{elementsEmojis[2]} , ножиці{elementsEmojis[0]} або папір{elementsEmojis[1]}")
                    )
                .AddComponents(myButton2)
                .AddComponents(myButton)
                .AddComponents(myButton1);
            var embMessage = await ctx.Channel.SendMessageAsync(emb);

            await Task.Delay(TimeSpan.FromSeconds(15));

            await embMessage.DeleteAsync();
        }

        [SlashCommand("rolldice", "Кинути кубик")]
        public async Task RollDiceCom(InteractionContext ctx, [Choice("D4", 4)]
                                                              [Choice("D6", 6)]
                                                              [Choice("D8", 8)]
                                                              [Choice("D10", 10)]
                                                              [Choice("D12", 12)]
                                                              [Choice("D20", 20)]
                                                              [Option("грані","Скілько граний кубик ви хочити кинути")] long dices)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));
            DiscordEmoji[] elementsEmojis = { DiscordEmoji.FromName(ctx.Client, ":black_square_button:", false), //withe
                                              DiscordEmoji.FromName(ctx.Client, ":black_large_square:", false),//bla
                                              DiscordEmoji.FromName(ctx.Client, ":blue_square:", false)};//emp
            DiscordEmoji numberEmoji = null;
            DiscordEmoji druganumberEmoji = null;
            DiscordEmoji[] D6Emojis = { DiscordEmoji.FromName(ctx.Client, ":black_square_button:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":white_circle:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":black_large_square:", false) };
            switch (dices)
            {
                case 4:
                    var sidesD4 = new Random().Next(1, 5);
                    switch (sidesD4)
                    {
                        case 1:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 2:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 3:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":three:", false);
                            break;
                        case 4:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":four:", false);
                            break;
                    }
                    var sideD4 = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.White)
                    .WithTitle("Результат: " + sidesD4)
                    .WithDescription($"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}"));
                    await ctx.Channel.SendMessageAsync(sideD4);
                    break;
                case 6:
                    var sidesD6 = new Random().Next(1, 7);
                    switch (sidesD6)
                    {
                        case 1:
                            var side1 = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()

                                    .WithColor(DiscordColor.White)
                                    .WithTitle("Результат " + sidesD6)
                                    .WithDescription($"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}"));
                            await ctx.Channel.SendMessageAsync(side1);
                            break;
                        case 2:
                            var side2 = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithColor(DiscordColor.White)
                                    .WithTitle("Результат " + sidesD6)
                                    .WithDescription($"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}"));
                            await ctx.Channel.SendMessageAsync(side2);
                            break;
                        case 3:
                            var side3 = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithColor(DiscordColor.White)
                                    .WithTitle("Результат " + sidesD6)
                                    .WithDescription($"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}"));
                            await ctx.Channel.SendMessageAsync(side3);
                            break;
                        case 4:
                            var side4 = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithColor(DiscordColor.White)
                                    .WithTitle("Результат " + sidesD6)
                                    .WithDescription($"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}"));
                            await ctx.Channel.SendMessageAsync(side4);
                            break;
                        case 5:
                            var side5 = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithColor(DiscordColor.White)
                                    .WithTitle("Результат " + sidesD6)
                                    .WithDescription($"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[2]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}"));
                            await ctx.Channel.SendMessageAsync(side5);
                            break;
                        case 6:
                            var side6 = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithColor(DiscordColor.White)
                                    .WithTitle("Результат " + sidesD6)
                                    .WithDescription($"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[1]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[1]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[1]}{D6Emojis[1]}{D6Emojis[1]}{D6Emojis[0]}\n" +
                                                     $"{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}{D6Emojis[0]}"));
                            break;
                    }
                    break;
                case 8:
                    var sides = new Random().Next(1, 9);
                    switch (sides)
                    {
                        case 1:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 2:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 3:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":three:", false);
                            break;
                        case 4:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":four:", false);
                            break;
                        case 5:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":five:", false);
                            break;
                        case 6:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":six:", false);
                            break;
                        case 7:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":seven:", false);
                            break;
                        case 8:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":eight:", false);
                            break;
                    }
                    var side = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.White)
                    .WithTitle("Результат " + sides)
                    .WithDescription($"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                     $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}"));
                    await ctx.Channel.SendMessageAsync(side);
                    break;
                case 10:
                    var sidesD10 = new Random().Next(1, 11);
                    switch (sidesD10)
                    {
                        case 1:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 2:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 3:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":three:", false);
                            break;
                        case 4:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":four:", false);
                            break;
                        case 5:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":five:", false);
                            break;
                        case 6:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":six:", false);
                            break;
                        case 7:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":seven:", false);
                            break;
                        case 8:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":eight:", false);
                            break;
                        case 9:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":nine:", false);
                            break;
                        case 10:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":keycap_ten:", false);
                            break;
                    }
                    var sideD10 = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.White)
                        .WithTitle("Результат " + sidesD10)
                        .WithDescription($"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                         $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                         $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                         $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                                         $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                                         $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}"));
                    await ctx.Channel.SendMessageAsync(sideD10);
                    break;
                case 12:
                    var sidesD12 = new Random().Next(1, 13);
                    switch (sidesD12)
                    {
                        case 1:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 2:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 3:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":three:", false);
                            break;
                        case 4:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":four:", false);
                            break;
                        case 5:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":five:", false);
                            break;
                        case 6:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":six:", false);
                            break;
                        case 7:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":seven:", false);
                            break;
                        case 8:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":eight:", false);
                            break;
                        case 9:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":nine:", false);
                            break;
                        case 10:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":keycap_ten:", false);
                            break;
                        case 11:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 12:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                    }
                    string a = null;
                    if (druganumberEmoji == null)
                    {
                        a = $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                            $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                            $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}";
                    }
                    else
                    {
                        a = $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                            $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{druganumberEmoji}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                            $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                            $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}";
                    }
                    var sideD12 = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.White)
                            .WithTitle("Результат " + sidesD12)
                            .WithDescription(a));
                    await ctx.Channel.SendMessageAsync(sideD12);
                    break;
                case 20:
                    var sidesD20 = new Random().Next(1, 21);
                    switch (sidesD20)
                    {
                        case 1:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 2:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 3:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":three:", false);
                            break;
                        case 4:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":four:", false);
                            break;
                        case 5:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":five:", false);
                            break;
                        case 6:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":six:", false);
                            break;
                        case 7:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":seven:", false);
                            break;
                        case 8:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":eight:", false);
                            break;
                        case 9:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":nine:", false);
                            break;
                        case 10:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":keycap_ten:", false);
                            break;
                        case 11:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            break;
                        case 12:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 13:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":three:", false);
                            break;
                        case 14:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":four:", false);
                            break;
                        case 15:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":five:", false);
                            break;
                        case 16:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":six:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            break;
                        case 17:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":seven:", false);
                            break;
                        case 18:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":eight:", false);
                            break;
                        case 19:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":one:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":nine:", false);
                            break;
                        case 20:
                            numberEmoji = DiscordEmoji.FromName(ctx.Client, ":two:", false);
                            druganumberEmoji = DiscordEmoji.FromName(ctx.Client, ":zero:", false);
                            break;
                    }
                    string b = null;
                    if (druganumberEmoji == null)
                    {
                        b = $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}";
                    }
                    else
                    {
                        b = $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{numberEmoji}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[2]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{druganumberEmoji}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[2]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}\n" +
                             $"{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[0]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}{elementsEmojis[1]}";
                    }
                    var sideD20 = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.White)
                            .WithTitle("Результат " + sidesD20)
                            .WithDescription(b));
                    await ctx.Channel.SendMessageAsync(sideD20);
                    break;
            }
        }
    }
}
