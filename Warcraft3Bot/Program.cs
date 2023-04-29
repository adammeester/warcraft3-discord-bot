using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace discordBotNet
{
    class Program
    {
        public static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var bot = new ScraperBot();
            await bot.Connect();
        }
    }
}
