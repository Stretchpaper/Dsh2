using Dsh.Commands;
using Dsh.Extennal_Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dsh
{
    internal class Bot
    {
        public DiscordClient Client { get; set; }
        public InteractivityExtension Extension { get; set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJSON = JsonConvert.DeserializeObject<ConfigJSON>(json);

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJSON.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1)
            });

            Client.GuildMemberAdded += Discord_GuildMemberAdded;
            Client.ComponentInteractionCreated += ButtonPreesReaction;
            Client.MessageCreated += DellunneedntMes;
            Client.VoiceStateUpdated += OnBotVoiceStateUpdated;

            var slashCommandsConfig = Client.UseSlashCommands();

            slashCommandsConfig.RegisterCommands<PlayCommands>();
            slashCommandsConfig.RegisterCommands<AdminCommands>();
            slashCommandsConfig.RegisterCommands<ChatFilteringCommands>();
            slashCommandsConfig.RegisterCommands<FolkCommands>();
            slashCommandsConfig.RegisterCommands<MusicCommands>();
            
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "suki.nathan.to",
                Port = 443,
                Secured = true
            };

            var lavlainkConfig = new LavalinkConfiguration
            {
                Password = "adowbongmanacc",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = Client.UseLavalink();

            Client.Ready += OnClientReady;

            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavlainkConfig);
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        public async Task OnBotVoiceStateUpdated(DiscordClient client, VoiceStateUpdateEventArgs e)
        {
            var botVC = e.Guild.CurrentMember.VoiceState?.Channel;

            if (botVC == null)
            {
                ulong serverId = e.Guild.Id;

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
            }
        }



        private async Task DellunneedntMes(DiscordClient sender, MessageCreateEventArgs e)
        {
            var message = e.Message;
            var content = message.Content;

            if (message.Author.IsBot)
                return;

            string forbiddenWord;
            if (ContainsForbiddenWord(content, message.Channel.Guild.Id, out forbiddenWord))
            {
                content = content.Replace(forbiddenWord, string.Empty);

                await message.DeleteAsync(content);

                await message.RespondAsync("ЗАСУДЖУЮ >:(");

                var member = await message.Channel.Guild.GetMemberAsync(message.Author.Id);

                if (!member.IsOwner && !member.Permissions.HasPermission(Permissions.Administrator))
                {
                    var timeDuration = DateTime.Now + TimeSpan.FromMinutes(5);
                    await member.TimeoutAsync(timeDuration);
                }
            }
        }

        private bool ContainsForbiddenWord(string content, ulong serverId, out string forbiddenWord)
        {
            forbiddenWord = null;

            DB db = new DB();
            db.OpenConnection();

            MySqlCommand command = new MySqlCommand("SELECT `word` FROM `wordslist` WHERE `ServerID` = @sID", db.GetConnection());
            command.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                forbiddenWord = reader.GetString("word");
                if (content.Contains(forbiddenWord))
                {
                    reader.Close();
                    db.CloseConnection();
                    return true;
                }
            }

            reader.Close();
            db.CloseConnection();

            return false;
        }

        private async Task ButtonPreesReaction(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            var bruh = new GlobalClass();
            var win = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Green)
                    .WithTitle("Ви вийграли")
                    .WithDescription($"Бот вибрав: {bruh.BotRes}")
                );
            var draw = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Yellow)
                    .WithTitle("Нічия")
                    .WithDescription($"Бот вибрав: {bruh.BotRes}")
                );
            var lost = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle("Ви програли")
                    .WithDescription($"Бот вибрав: {bruh.BotRes}")
                );
            var idasha = e.Interaction.Data.CustomId;
            switch (idasha)
            {
                case "1":
                    bruh.UserRes = "Ножиці";
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await Task.Delay(10);
                    switch (bruh.BotRes)
                    {
                        case "Камінь":
                            await e.Channel.SendMessageAsync(win);
                            break;
                        case "Ножиці":
                            await e.Channel.SendMessageAsync(draw);
                            break;
                        case "Папір":
                            await e.Channel.SendMessageAsync(lost);
                            break;
                    }
                    break;
                case "2":
                    bruh.UserRes = "Папір";
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await Task.Delay(10);
                    switch (bruh.BotRes)
                    {
                        case "Камінь":
                            await e.Channel.SendMessageAsync(lost);
                            break;
                        case "Ножиці":
                            await e.Channel.SendMessageAsync(win);
                            break;
                        case "Папір":
                            await e.Channel.SendMessageAsync(draw);
                            break;
                    }
                    break;
                case "3":
                    bruh.UserRes = "Камінь";
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await Task.Delay(10);
                    switch (bruh.BotRes)
                    {
                        case "Камінь":
                            await e.Channel.SendMessageAsync(draw);
                            break;
                        case "Ножиці":
                            await e.Channel.SendMessageAsync(lost);
                            break;
                        case "Папір":
                            await e.Channel.SendMessageAsync(win);
                            break;
                    }
                    break;
            }
        }

        private async Task Discord_GuildMemberAdded(DiscordClient s, GuildMemberAddEventArgs e)
        {
            var systemChannel = e.Guild.SystemChannel;
            if (systemChannel != null)
            {
                await systemChannel.SendMessageAsync($"Шановний(а), {e.Member.Mention}! Ласкаво просимо на сервер!");
            }
        }

        
    }
}
