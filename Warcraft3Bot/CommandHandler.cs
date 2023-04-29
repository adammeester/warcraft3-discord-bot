using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace discordBotNet
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsnyc;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                        services: null);
        }
        public async Task HandleCommandAsnyc(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            int argPos = 0;

            //preventing detection of commands from the bot itself or from messages without the correct prefix
            bool validPrefix = !message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos);
            if (validPrefix || message.Author.IsBot)
            {
                return;
            }

            //getting context
            var context = new SocketCommandContext(_client, message);

            //executing the command
            var result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
