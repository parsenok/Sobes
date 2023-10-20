using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Newtonsoft.Json;





class PokerSummary
{
    public int TotalHands { get; set; }
    public decimal TotalWinnings { get; set; }
    public Dictionary<string, decimal> PlayerWinnings { get; set; }

    public PokerSummary()
    {
        TotalHands = 0;
        TotalWinnings = 0;
        PlayerWinnings = new Dictionary<string, decimal>();
    }
    public static decimal ExtractWinnings(string line)
    {
        string pattern = @"\$\d+\.\d+";
        var match = Regex.Match(line, pattern);
        if (match.Success)
        {
            decimal winnings = decimal.Parse(match.Value.Substring(1), CultureInfo.InvariantCulture);
            return winnings;
        }
        return 0;
    }
    public static string ExtractPlayerName(string line)
    {
        string pattern = @"\b\w+\d+\b";
        var match = Regex.Match(line, pattern);
        if (match.Success)
        {
            return match.Value;
        }
        return "Имя игрока не найдено";
    }
}

class Program
{
    static void Main(string[] args)
    {
        PokerSummary summary = new PokerSummary();

        void workWithZip()
        {
            using (ZipArchive archive = ZipFile.OpenRead("E:\\Задачки к собесам\\2013-08-11_ps_nl600_sh.zip"))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (Stream stream = entry.Open())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();
                        string[] lines = content.Split('\n');

                        foreach (string line in lines)
                        {
                            if (line.Contains("PokerStars Game #"))
                            {
                                summary.TotalHands++;
                            }
                            else if (line.Contains("collected $"))
                            {
                                decimal winnings = PokerSummary.ExtractWinnings(line);
                                summary.TotalWinnings += winnings;
                                string playerName = PokerSummary.ExtractPlayerName(line);
                                if (!summary.PlayerWinnings.ContainsKey(playerName))
                                {
                                    summary.PlayerWinnings[playerName] = 0;
                                }
                                summary.PlayerWinnings[playerName] += winnings;
                            }
                        }
                    }
                }
            }
        }

        workWithZip();
        Console.WriteLine("Общее количество рук: " + summary.TotalHands);
        Console.WriteLine("Суммарный выигрыш всех игроков: $" + summary.TotalWinnings);

        Console.WriteLine("Суммарный выигрыш по каждому игроку в отдельности:");
        foreach (var playerWinnings in summary.PlayerWinnings)
        {
            Console.WriteLine(playerWinnings.Key + ": $" + playerWinnings.Value);
        }
        try
        {
            string jsonData = JsonConvert.SerializeObject(summary, Formatting.Indented);
            File.WriteAllText("E:\\Задачки к собесам\\output.json", jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка записи в JSON-файл: " + ex.Message);
        }
    }
}
