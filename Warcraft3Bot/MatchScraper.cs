using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace discordBotNet
{
    class MatchScraper
    {
        private int _lineArg;
        private string m_Wc3HomePageURL = "https://liquipedia.net/warcraft/Main_Page";
        private string m_Wc3HomePageLocal = "warcraft3matches.html";

        public string Scrape(int userArg)
        {
            string rString = "";
            DownloadFile(m_Wc3HomePageURL, m_Wc3HomePageLocal);
            string[] lines = File.ReadAllLines(m_Wc3HomePageLocal);
            _lineArg = userArg;

            var count = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains("Upcoming Matches"))
                {
                    for (int n = i; n < lines.Length; n++)
                    {
                        if (count == _lineArg)
                        {
                            return rString;
                        }
                        string matchLine = lines[n];
                        bool leftTeam = matchLine.Contains("team-left");
                        if (leftTeam || matchLine.Contains("team-right"))
                        {
                            var titleString = "title=\"";
                            var endIndex = -1;
                            StringTokenData playerNameData;
                            StringTokenData raceNameData;
                            StringTokenData countryNameData;
                            StringTokenData teamNameData;
                            if (matchLine.Contains("team-template-text") || matchLine.Contains("team-template-image"))
                            {

                                var teamTitleString = "data-highlightingclass=\"";
                                teamNameData = FindStringInBetween(matchLine, teamTitleString, "\"", 0, out endIndex);

                                Console.WriteLine($"{teamNameData.m_Token}");
                                rString += teamNameData.m_Token;
                                if (leftTeam)
                                {
                                    Console.WriteLine("     vs. ");
                                    rString += "     vs.     ";
                                }
                            }

                            if (leftTeam)
                            {
                                playerNameData = FindStringInBetween(matchLine, titleString, "\"", 0, out endIndex);
                                raceNameData = FindStringInBetween(matchLine, titleString, "\"", playerNameData.m_EndIndex, out endIndex);
                                countryNameData = FindStringInBetween(matchLine, titleString, "\"", raceNameData.m_EndIndex, out endIndex);
                            }
                            else
                            {
                                countryNameData = FindStringInBetween(matchLine, titleString, "\"", 0, out endIndex);
                                raceNameData = FindStringInBetween(matchLine, titleString, "\"", countryNameData.m_EndIndex, out endIndex);
                                playerNameData = FindStringInBetween(matchLine, titleString, "\"", raceNameData.m_EndIndex, out endIndex);
                            }

                            if (!matchLine.Contains("team-template-text") && !matchLine.Contains("team-template-image"))
                            {
                                Console.WriteLine($"{playerNameData.m_Token} ({raceNameData.m_Token})");
                                rString += $"{playerNameData.m_Token} ({raceNameData.m_Token})\n";
                                Console.WriteLine($"Country: {countryNameData.m_Token}");
                                rString += $"Country: {countryNameData.m_Token}\n";
                                if (leftTeam)
                                {
                                    Console.WriteLine("     vs.\n");
                                    rString += "     vs.\n";
                                }
                            }
                        }
                        if (matchLine.Contains("match-filler"))
                        {
                            count += 1;
                            var endIndex = -1;
                            string matchString = "&amp;#8203;\">";
                            string timeZoneString = "data-tz=\"";
                            StringTokenData matchData;
                            StringTokenData timeZoneData;
                            matchData = FindStringInBetween(matchLine, matchString, "<", 0, out endIndex);
                            timeZoneData = FindStringInBetween(matchLine, timeZoneString, "\"", matchData.m_EndIndex, out endIndex);
                            Console.WriteLine($"Match Time: {matchData.m_Token} {timeZoneData.m_Token}\n");
                            rString += $"Match Time: {matchData.m_Token} {timeZoneData.m_Token}\n\n";
                        }
                    }
                }
            }
            return rString;
        }
        private static void DownloadFile(string remoteFilename, string localFilename)
        {
            WebClient client = new WebClient();
            client.DownloadFile(remoteFilename, localFilename);
        }

        private struct StringTokenData
        {
            public int m_EndIndex;
            public string m_Token;
        }
        private StringTokenData FindStringInBetween(string line, string searchStringStart, string searchStringEnd, int startIndex, out int endIndex)
        {
            int titleIndex = line.IndexOf(searchStringStart, startIndex) + searchStringStart.Length;
            endIndex = line.IndexOf(searchStringEnd, titleIndex);
            return new StringTokenData() { m_Token = line.Substring(titleIndex, endIndex - titleIndex), m_EndIndex = endIndex };
        }

    }
}
