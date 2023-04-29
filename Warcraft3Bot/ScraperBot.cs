using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace discordBotNet
{
    class ScraperBot
    {
        private DiscordSocketClient _client;
        private string _token;
        private CommandHandler _commandHandler;
        private CommandService _commandService;

        public ScraperBot()
        {
            Console.WriteLine("hello");
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        public async Task Connect()
        {
            //token is stored locally in a text file. Will adjust this in future
            _token = System.IO.File.ReadAllText(@"C:\Users\Adam\Desktop\Token.txt");
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
            _commandHandler = new CommandHandler(_client, _commandService);
            await _commandHandler.InstallCommandsAsync();

            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot,
                _token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
