using Dsh.Extennal_Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dsh.Commands
{
    [SlashCommandGroup("music", "Music commands")]
    public class MusicCommands : ApplicationCommandModule
    {
        public DiscordChannel GLuserVC;
        private ulong GLSeverID;
        private InteractionContext GLctx;

        private async Task Conn_PlaybackFinished(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
        {
            DB db = new DB();
            try
            {
                db.OpenConnection();

                MySqlCommand selectCommand = new MySqlCommand("SELECT `SongName` FROM `musictable` WHERE `ServerID` = @sID AND `QueuePos` = 1;", db.GetConnection());
                selectCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = GLSeverID;

                string songName = selectCommand.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(songName))
                {
                    return;
                }

                MySqlCommand updateCommand = new MySqlCommand("UPDATE `musictable` SET `QueuePos` = `QueuePos` - 1 WHERE `ServerID` = @sID;", db.GetConnection());
                updateCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = GLSeverID;
                updateCommand.ExecuteNonQuery();

                string musicDescription = $"Now Playing: {songName}";

                var nowPlayingEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Purple,
                    Title = $"Successfully joined channel {GLuserVC.Name} and playing music",
                    Description = musicDescription
                };

                await GLctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .AddEmbed(nowPlayingEmbed)).ConfigureAwait(false);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Помилка бази даних: {ex.Message}");
                var errorr = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "Помилка"
                };

                await GLctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .AddEmbed(errorr)).ConfigureAwait(false);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        [SlashCommand("play", "Plays music in a voice channel")]
        public async Task PlayMusic(InteractionContext ctx, [Option("query", "The search query or URL")] string query)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            GLctx = ctx;
            //PRE-EXECUTION CHECKS
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Please enter a VC!!!")).ConfigureAwait(false);
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Connection is not Established!!!")).ConfigureAwait(false);
                return;
            }

            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Please enter a valid VC!!!")).ConfigureAwait(false);
                return;
            }

            //Connecting to the VC and playing music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC).ConfigureAwait(false);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            conn.PlaybackFinished += Conn_PlaybackFinished;

            if (conn == null)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Lavalink Failed to connect!!!")).ConfigureAwait(false);
                return;
            }

            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Failed to find music with query: {query}")).ConfigureAwait(false);
                return;
            }

            ulong serverId = ctx.Guild.Id;

            var musicTrack = searchQuery.Tracks.First();

            string queuePosstr = string.Empty;
            if (conn.CurrentState.CurrentTrack == null)
            {
                await conn.PlayAsync(musicTrack).ConfigureAwait(false);

                string musicDescription = $"Now Playing: {musicTrack.Title} \n" +
                                            $"Author: {musicTrack.Author} \n" +
                                            $"URL: {musicTrack.Uri}";

                var nowPlayingEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Purple,
                    Title = $"Successfully joined channel {userVC.Name} and playing music",
                    Description = musicDescription
                };

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(nowPlayingEmbed)).ConfigureAwait(false);
            }
            else
            {
                DB db = new DB();
                try
                {
                    db.OpenConnection();

                    MySqlCommand selectCommand = new MySqlCommand("SELECT COUNT(*) FROM `musictable` WHERE `ServerID` = @sID;", db.GetConnection());
                    selectCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;

                    int wordCount = Convert.ToInt32(selectCommand.ExecuteScalar());

                    int queuePos = wordCount + 1;

                    MySqlCommand insertCommand = new MySqlCommand("INSERT INTO `musictable` (`QueuePos`, `ServerID`, `SongName`) VALUES (@pos ,@sID, @song);", db.GetConnection());
                    insertCommand.Parameters.Add("@pos", MySqlDbType.Int64).Value = queuePos;
                    insertCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;
                    insertCommand.Parameters.Add("@song", MySqlDbType.VarChar).Value = searchQuery.Tracks.First().Title;

                    queuePosstr = queuePos.ToString();

                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        string musicDescription = $"Song: {musicTrack.Title}\n" +
                                                  $"Author: {musicTrack.Author}\n" +
                                                  $"Queue position: {queuePosstr}";
                        var AddQRforPlay = new DiscordEmbedBuilder()
                        {
                            Color = DiscordColor.Purple,
                            Title = $"Song add to queue",
                            Description = musicDescription
                        };

                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .AddEmbed(AddQRforPlay)).ConfigureAwait(false);
                    }
                    else
                    {
                        var errorr = new DiscordEmbedBuilder()
                        {
                            Color = DiscordColor.Red,
                            Title = "Помилка"
                        };

                        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .AddEmbed(errorr)).ConfigureAwait(false);
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Помилка бази даних: {ex.Message}");
                    var errorr = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Помилка"
                    };

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(errorr)).ConfigureAwait(false);
                }
                finally
                {
                    db.CloseConnection();
                }
            }
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

            ulong serverId = ctx.Guild.Id;

            DB db = new DB();
            try
            {
                db.OpenConnection();

                MySqlCommand deleteCommand = new MySqlCommand("DELETE FROM `musictable` WHERE `ServerID` = @sID;", db.GetConnection());
                deleteCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;
                deleteCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Помилка бази даних: {ex.Message}");
                // Обробка помилки бази даних
            }
            finally
            {
                db.CloseConnection();
            }

            var stopEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "Stopped the Track",
                Description = "Successfully disconnected from the voice channel and cleared the music queue"
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(stopEmbed));
        }
    }
}