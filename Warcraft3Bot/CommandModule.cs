using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;
using System.Threading;

namespace discordBotNet
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        [Summary("prints a thing")]
        public async Task TestCommand([Summary("the thing to print")] int x)
        {
            await Context.Channel.SendMessageAsync($"hello {x}");
        }

        [Command("matches")]
        [Summary("Prints upcoming matches")]
        public async Task MatchesCommand([Summary("Number of matches to display")] int args)
        {
            var scraper = new MatchScraper();
            var result = "";
            Thread t = new Thread(() => { result = scraper.Scrape(args); });
            t.Name = "match thread";
            try
            {
                t.Start();
                t.Join();
                await Context.Channel.SendMessageAsync($"Upcoming Matches:\n {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //[Command("stats")]
        //[Summary("Returns stats for specified player")]
        //public async Task StatsCommand([Summary("Stats of player")] string name)
        //{
        //    var scraper new StatScraper();
        //}

    }
}
