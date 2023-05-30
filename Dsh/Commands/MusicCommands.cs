using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsh.Commands
{
    public class MusicCommands : ApplicationCommandModule
    {
        [SlashCommand("play", "Plays music in a voice channel")]
        public async Task PlayMusic(InteractionContext ctx, [Option("query", "The search query or URL")] string query)
        {
            var userVC = ctx.Member.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // ПЕРЕД ЗАПУСКОМ ПЕРЕВІРКИ
            if (userVC == null)
            {
                // Перевірка, чи користувач перебуває у голосовому каналі
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                // Перевірка, чи встановлене з'єднання з Lavalink
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Connection is not established!")
                    .AsEphemeral(true));
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                // Перевірка, чи введено дійсний голосовий канал
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a valid voice channel!")
                    .AsEphemeral(true));
                return;
            }

            // Підключення до голосового каналу та відтворення музики
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(ctx.Guild);
            if (conn == null)
            {
                // Перевірка, чи вдалося підключитися до голосового каналу
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Failed to connect to the voice channel!")
                    .AsEphemeral(true));
                return;
            }

            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                // Перевірка, чи вдалося знайти музику за запитом
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"Failed to find music with query: {query}")
                    .AsEphemeral(true));
                return;
            }

            var musicTrack = searchQuery.Tracks.First();

            await conn.PlayAsync(musicTrack);

            string musicDescription = $"Now Playing: {musicTrack.Title} \n" +
                                      $"Author: {musicTrack.Author} \n" +
                                      $"URL: {musicTrack.Uri}";

            var nowPlayingEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Purple,
                Title = $"Successfully joined channel {userVC.Name} and playing music",
                Description = musicDescription
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(nowPlayingEmbed));
        }


        [SlashCommand("pause", "Pauses the currently playing music")]
        public async Task PauseMusic(InteractionContext ctx)
        {
            var userVC = ctx.Member.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // PRE-EXECUTION CHECKS
            if (userVC == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Connection is not established!")
                    .AsEphemeral(true));
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a valid voice channel!")
                    .AsEphemeral(true));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Failed to connect to the voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("No tracks are playing!")
                    .AsEphemeral(true));
                return;
            }

            await conn.PauseAsync();

            var pausedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Title = "Track Paused"
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(pausedEmbed));
        }

        [SlashCommand("resume", "Resumes the paused music")]
        public async Task ResumeMusic(InteractionContext ctx)
        {
            var userVC = ctx.Member.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // PRE-EXECUTION CHECKS
            if (userVC == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Connection is not established!")
                    .AsEphemeral(true));
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a valid voice channel!")
                    .AsEphemeral(true));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Failed to connect to the voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("No tracks are playing!")
                    .AsEphemeral(true));
                return;
            }

            await conn.ResumeAsync();

            var resumedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Resumed"
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(resumedEmbed));
        }

        [SlashCommand("stop", "Stops the currently playing music")]
        public async Task StopMusic(InteractionContext ctx)
        {
            var userVC = ctx.Member.VoiceState?.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            // PRE-EXECUTION CHECKS
            if (userVC == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Connection is not established!")
                    .AsEphemeral(true));
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please enter a valid voice channel!")
                    .AsEphemeral(true));
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Failed to connect to the voice channel!")
                    .AsEphemeral(true));
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("No tracks are playing!")
                    .AsEphemeral(true));
                return;
            }

            await conn.StopAsync();
            await conn.DisconnectAsync();

            var stopEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "Stopped the Track",
                Description = "Successfully disconnected from the voice channel"
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(stopEmbed));
        }
    }
}

