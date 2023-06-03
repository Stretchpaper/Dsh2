using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsh
{
    public class AdminCommands : ApplicationCommandModule
    {
        [SlashCommand("ban", "Забороняє користувачеві доступ до сервера")]
        public async Task Ban(InteractionContext ctx, [Option("користувач", "Користувач, якого ви хочете заборонити")] DiscordUser user,
                                                      [Option("причина", "Причина заборони")] string reason = null)
        {
            await ctx.DeferAsync();

            if (ctx.Member.Permissions.HasPermission(Permissions.Administrator))
            {
                var member = (DiscordMember)user;
                await ctx.Guild.BanMemberAsync(member, 0, reason);

                var banMessage = new DiscordEmbedBuilder()
                {
                    Title = "Заблоковано користувача " + member.Username,
                    Description = "Причина: " + reason,
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(banMessage));
            }
            else
            {
                var nonAdminMessage = new DiscordEmbedBuilder()
                {
                    Title = "У доступі відмовлено.",
                    Description = "Для виконання цієї команди потрібно мати права адміністратора",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }

        [SlashCommand("kick", "Вигнати користувача з сервера")]
        public async Task Kick(InteractionContext ctx, [Option("користувач", "Користувач, якого ви хочете вигнати")] DiscordUser user)
        {
            await ctx.DeferAsync();

            if (ctx.Member.Permissions.HasPermission(Permissions.Administrator))
            {
                var member = (DiscordMember)user;
                await member.RemoveAsync();

                var kickMessage = new DiscordEmbedBuilder()
                {
                    Title = member.Username + " було вигнано з сервера",
                    Description = "вигнали " + ctx.User.Username,
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(kickMessage));
            }
            else
            {
                var nonAdminMessage = new DiscordEmbedBuilder()
                {
                    Title = "У доступі відмовлено.",
                    Description = "Для виконання цієї команди потрібно мати права адміністратора",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }

        [SlashCommand("timeout", "Тайм-аут користувачу")]
        public async Task Timeout(InteractionContext ctx, [Option("користувач", "Користувач, для якого ви хочете встановити тайм-аут")] DiscordUser user,
                                                          [Option("тривалість", "Тривалість тайм-ауту в секундах")] long duration)
        {
            await ctx.DeferAsync();

            var member = await ctx.Guild.GetMemberAsync(user.Id);

            if (ctx.Member.IsOwner || member.Permissions.HasPermission(Permissions.Administrator))
            {
                var timeDuration = DateTime.Now + TimeSpan.FromSeconds(duration);
                await member.TimeoutAsync(timeDuration);

                var timeoutMessage = new DiscordEmbedBuilder()
                {
                    Title = member.Username + "закінчився таймаут",
                    Description = "Тривалість: " + TimeSpan.FromSeconds(duration).ToString(),
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(timeoutMessage));
            }
            else
            {
                var nonAdminMessage = new DiscordEmbedBuilder()
                {
                    Title = "У доступі відмовлено.",
                    Description = "Для виконання цієї команди потрібно мати права адміністратора або бути власником сервера",
                    Color = DiscordColor.Red
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }

        [SlashCommand("clear", "Вигнати користувача з сервера")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Clear(InteractionContext ctx, [Option("count", "Кількість повідомлень, які потрібно видалити.")] long count)
        {
            await ctx.DeferAsync();
            if (count <= 0 || count > 100)
            {
                var ssuccessMessage = await ctx.Channel.SendMessageAsync("Кількість повідомлень для видалення повинна бути в межах від 1 до 100.");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Кількість повідомлень для видалення повинна бути в межах від 1 до 100."));
                return;
            }
            var messages = await ctx.Channel.GetMessagesAsync((int)count + 1);

            foreach (var message in messages)
            {
                await ctx.Channel.DeleteMessageAsync(message);
                await Task.Delay(1000); 
            }

            var successMessage = await ctx.Channel.SendMessageAsync($"Успішно видалено {count} повідомлень.");

            await Task.Delay(5000);

            await successMessage.DeleteAsync();
        }

        [SlashCommand("sendMessage", "Відправити повідомлення у особисті")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task SendMessageToU(InteractionContext ctx, [Option("Користувач", "Кому відправити")] DiscordUser user,
                                                                [Option("Повідомлення", "Що ви хочете відправити")] string message)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("..."));
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            var DmChennel = await member.CreateDmChannelAsync();
            await DmChennel.SendMessageAsync(message);
        }

        [SlashCommand("giverole", "Видає ролі гравцям")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task GiveeRole(InteractionContext ctx, [Option("користувач", "Ведіть імя користувача якому хочете видати роль")] DiscordUser user,
                                                            [Option("роль", "Ведіть роль яку хочете видати користувачу")] DiscordRole roles)
        {
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            var role = ctx.Guild.GetRole(roles.Id);
            await member.GrantRoleAsync(role).ConfigureAwait(false);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"Користувачу **{user.Username}** видана роль **{role.Name}**"));
        }

        [SlashCommand("removerole", "Забирає ролі у гравців")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveRole(InteractionContext ctx, [Option("користувач", "Введіть ім'я користувача, якому потрібно забрати роль")] DiscordUser user,
                                                             [Option("роль", "Введіть роль, яку потрібно забрати у користувача")] DiscordRole role)
        {
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);
            DiscordRole targetRole = ctx.Guild.GetRole(role.Id);
            await member.RevokeRoleAsync(targetRole).ConfigureAwait(false);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent($"Роль **{targetRole.Name}** успішно забрана у користувача **{user.Username}**."));
        }
    }
}
