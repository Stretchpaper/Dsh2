using Dsh.Extennal_Classes;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dsh.Commands
{
    //[SlashCommandGroup("music", "Music commands")]
    public class MusicCommands : ApplicationCommandModule
    {
        private Queue<LavaSong> q = new Queue<LavaSong>();
        private DiscordChannel lastChannel = null;

        private async Task Conn_PlaybackFinished(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
        {
            if (q.Count != 0)
            {
                var track = q.Dequeue();
                await sender.PlayAsync(track.lavaTrack);
                string musicDescription = $"Now Playing: {track.lavaTrack.Title} \n" +
                                            $"Author: {track.lavaTrack.Author} \n" +
                                            $"URL: {track.lavaTrack.Uri}";

                var embed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.DarkGreen,
                    Title = $"Зараз грає",
                    Description = musicDescription
                };
                await track.requestChannel.SendMessageAsync(embed: embed);
                lastChannel = track.requestChannel;
            }
        }


        [SlashCommand("play", "playing the music!")]
        public async Task PlayCommand(InteractionContext ctx, [Option("Назва", "Будь ласка, введіть назву пісні або посилання")][RemainingText] string search)
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            LavalinkGuildConnection conn;
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                var embed1 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Yellow,
                    Title = $"Ви не знаходитесь в голосовому каналі!"
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed1));
                return;

            }
            conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                conn = await node.ConnectAsync(ctx.Member.VoiceState?.Channel);
            }
            else conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            conn.PlaybackFinished += Conn_PlaybackFinished;

            var loadResult = await node.Rest.GetTracksAsync(search);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                var embed1 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = $"Error!",
                    Description = "Пісню не знайдено!"
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed1));
                return;
            }
            var track = loadResult.Tracks.First();


            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                string musicDescription = $"Now Playing: {track.Title} \n" +
                                            $"Author: {track.Author} \n" +
                                            $"URL: {track.Uri}";

                var embed1 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Purple,
                    Title = $"Зараз грає",
                    Description = musicDescription
                };

                await conn.PlayAsync(track);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed1));
            }
            else
            {
                LavaSong s = new LavaSong(track, ctx.Channel);
                q.Enqueue(s);
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                string musicDescription = $"Song: {track.Title}\n" +
                                            $"Author: {track.Author}";
                var embed1 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Purple,
                    Title = $"Song add to queue",
                    Description = musicDescription
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed1));
            }
        }

        [SlashCommand("Skip", "Гортайте пісні.")]
        public async Task SkipCommand(InteractionContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            var node = lava.GetIdealNodeConnection();
            var conn = node.GetGuildConnection(ctx.Member.Guild);

            if (conn != null)
            {
                try
                {
                    await conn.SeekAsync(conn.CurrentState.CurrentTrack.Length);
                    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                    var embed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Purple,
                        Title = $"Song add to queue",
                        Description = "Пісню пропущено"
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                }
                catch
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                    var embed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Magenta,
                        Title = $"Пісню не вдалося пропустити",
                        Description = "Щось пішло не так :/"
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                }
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                var embed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Magenta,
                    Title = $"Пісню не вдалося пропустити",
                    Description = "Щось пішло не так :/"
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
        }

        private async Task HandleMusicAction(InteractionContext ctx, string action)
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

            var node = lavalinkInstance.ConnectedNodes.Values.FirstOrDefault();
            var conn = node?.GetGuildConnection(ctx.Guild);

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

            switch (action)
            {
                case "pause":
                    await conn.PauseAsync();
                    var pausedEmbed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Yellow,
                        Title = "Track Paused"
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(pausedEmbed));
                    break;

                case "resume":
                    await conn.ResumeAsync();
                    var resumedEmbed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Green,
                        Title = "Resumed"
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(resumedEmbed));
                    break;

                case "stop":
                    await conn.StopAsync();
                    await conn.DisconnectAsync();
                    var stopEmbed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Stopped the Track",
                        Description = "Successfully disconnected from the voice channel and cleared the music queue"
                    };
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(stopEmbed));
                    break;
            }
        }

        [SlashCommand("pause", "Pauses the currently playing music")]
        public async Task PauseMusic(InteractionContext ctx)
        {
            await HandleMusicAction(ctx, "pause");
        }

        [SlashCommand("resume", "Resumes the paused music")]
        public async Task ResumeMusic(InteractionContext ctx)
        {
            await HandleMusicAction(ctx, "resume");
        }

        [SlashCommand("stop", "Stops the currently playing music")]
        public async Task StopMusic(InteractionContext ctx)
        {
            await HandleMusicAction(ctx, "stop");
        }
    }
}
