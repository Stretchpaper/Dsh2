using Dsh.Extennal_Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Dsh.Commands
{
    public class ChatFilteringCommands : ApplicationCommandModule
    {
        [SlashCommand("conection", "Пеоевірка чи сервер у БД")]
        public async Task ChkConnection(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));

            ulong serverId = ctx.Guild.Id;


            DB db = new DB();
            db.OpenConnection();

            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `discordservers` WHERE `ServerID` = @sI", db.GetConnection());

            command.Parameters.Add("@sI", MySqlDbType.VarChar).Value = serverId;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
                await ctx.Channel.SendMessageAsync("Ви у базі");
            else
                await ctx.Channel.SendMessageAsync("Ні, Ви не у базі");

            db.CloseConnection();
        }

        [SlashCommand("show", "Показати заборонені слова на сервері")]
        public async Task ShowWords(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));

            ulong serverId = ctx.Guild.Id;

            DB db = new DB();
            db.OpenConnection();

            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT `word` FROM `wordslist` WHERE `ServerID` = @sID;", db.GetConnection());
            command.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {table.Rows[i]["word"].ToString()}");
                }

                var emb = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.IndianRed)
                        .WithTitle("Список слів")
                        .WithDescription(sb.ToString())
                    );

                await ctx.Channel.SendMessageAsync(emb);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("У базі даних відсутні слова.");
            }

            db.CloseConnection();
        }


        [SlashCommand("add", "Додати слово до списку заборонених слів")]
        public async Task AddCom(InteractionContext ctx, [Option("Слово", "Слово, яке ви хочете додати")] string word)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));

            ulong serverId = ctx.Guild.Id;

            DB db = new DB();
            db.OpenConnection();

            MySqlCommand selectCommand = new MySqlCommand("SELECT COUNT(*) FROM `wordslist` WHERE `ServerID` = @sID AND `word` = @word;", db.GetConnection());
            selectCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;
            selectCommand.Parameters.Add("@word", MySqlDbType.VarChar).Value = word;

            int wordCount = Convert.ToInt32(selectCommand.ExecuteScalar());

            if (wordCount > 0)
            {
                // Слово уже присутствует в базе данных
                await ctx.Channel.SendMessageAsync("Помилка: Слово вже існує.");
            }
            else
            {
                // Добавление слова в базу данных
                MySqlCommand insertCommand = new MySqlCommand("INSERT INTO `wordslist` (`ServerID`, `word`) VALUES (@sID, @word);", db.GetConnection());
                insertCommand.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;
                insertCommand.Parameters.Add("@word", MySqlDbType.VarChar).Value = word;

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Слово успешно добавлено
                    await ctx.Channel.SendMessageAsync("Слово успішно додано.");
                }
                else
                {
                    // Возникла ошибка при добавлении слова
                    await ctx.Channel.SendMessageAsync("Помилка: Не вдалося додати слово.");
                }
            }

            db.CloseConnection();
        }

        [SlashCommand("remove", "Видалити слово з списку заборонених слів")]
        public async Task RemoveWord(InteractionContext ctx, [Option("Word", "Слово для видалення")] string word)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));

            ulong serverId = ctx.Guild.Id;

            DB db = new DB();
            db.OpenConnection();

            MySqlCommand command = new MySqlCommand("DELETE FROM `wordslist` WHERE `ServerID` = @sID AND `word` = @word;", db.GetConnection());
            command.Parameters.Add("@sID", MySqlDbType.VarChar).Value = serverId;
            command.Parameters.Add("@word", MySqlDbType.VarChar).Value = word;

            int rowsAffected = command.ExecuteNonQuery();

            db.CloseConnection();

            if (rowsAffected > 0)
            {
                await ctx.Channel.SendMessageAsync($"Слово \"{word}\" видалено зі списку заборонених слів.");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Слово \"{word}\" не знайдено у списку заборонених слів.");
            }
        }
    }
}