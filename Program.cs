using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

class Program
{
    private static readonly object ConsoleLock = new object();

    static void Main(string[] args)
    {
        const string toolName = "Network-VlAN-Segmentation-Scanner";
        Console.Title = toolName;

        while (true)
        {
            Console.Clear();
            PrintBanner(toolName);
            WriteLineColor("This utility validates VLAN segmentation by testing gateway reachability across VLANs.", ConsoleColor.Gray);
            WriteLineColor("A successful response may indicate a potential segmentation gap.", ConsoleColor.Gray);
            WriteLineColor("Ensure the IP list file is correctly formatted before continuing.\n", ConsoleColor.Gray);

            // Describe the expected file content and structure
            WriteLineColor("Input File Format (CSV)", ConsoleColor.Cyan);
            WriteLineColor("Each line must contain a VLAN name and IP address, separated by a comma.", ConsoleColor.Gray);
            WriteLineColor("Example entries:", ConsoleColor.DarkGray);
            WriteLineColor("vlan1,10.10.5.1", ConsoleColor.White);
            WriteLineColor("vlan2,10.10.6.2\n", ConsoleColor.White);
            WriteLineColor("Optional: add 'Google DNS,8.8.8.8' to validate internet connectivity.", ConsoleColor.Gray);
            WriteLineColor("--------------------------------------------------------------------------------------\n", ConsoleColor.DarkGray);


            WriteColor("Enter the full path to the IP list file: ", ConsoleColor.Yellow);
            string ipListPath = Console.ReadLine();


            if (!File.Exists(ipListPath))
            {
                WriteLineColor("File not found. Verify the path and try again.", ConsoleColor.Red);
                if (ShouldExit())
                {
                    break;
                }
                continue;
            }


            string[] lines = File.ReadAllLines(ipListPath);

            if (lines.Length == 0)
            {
                WriteLineColor("The file is empty. Add a header and at least one VLAN/IP entry.", ConsoleColor.Red);
                if (ShouldExit())
                {
                    break;
                }
                continue;
            }

            if (!IsValidHeader(lines[0]))
            {
                WriteLineColor("Invalid CSV header. Expected: VLAN,IP", ConsoleColor.Red);
                WriteLineColor("Example header: VLAN,IP", ConsoleColor.DarkYellow);
                if (ShouldExit())
                {
                    break;
                }
                continue;
            }

            if (lines.Length == 1)
            {
                WriteLineColor("No data rows found. Add VLAN/IP rows below the header.", ConsoleColor.Red);
                if (ShouldExit())
                {
                    break;
                }
                continue;
            }


            ConcurrentBag<(string VlanName, string IpAddress, IPStatus Status)> pingResults = new ConcurrentBag<(string, string, IPStatus)>();


            Parallel.ForEach(lines.Skip(1).Select((line, index) => new { line, lineNumber = index + 2 }), entry =>
            {
                if (string.IsNullOrWhiteSpace(entry.line))
                {
                    WriteLineColor($"Skipping line {entry.lineNumber}: empty row.", ConsoleColor.DarkYellow);
                    return;
                }

                var parts = entry.line.Split(',');
                if (parts.Length != 2)
                {
                    WriteLineColor($"Skipping line {entry.lineNumber}: expected format VLAN,IP.", ConsoleColor.DarkYellow);
                    return;
                }

                string vlanName = parts[0].Trim();
                string ipAddress = parts[1].Trim();

                if (string.IsNullOrWhiteSpace(vlanName) || string.IsNullOrWhiteSpace(ipAddress))
                {
                    WriteLineColor($"Skipping line {entry.lineNumber}: VLAN or IP is missing.", ConsoleColor.DarkYellow);
                    return;
                }

                if (!IPAddress.TryParse(ipAddress, out _))
                {
                    WriteLineColor($"Skipping line {entry.lineNumber}: '{ipAddress}' is not a valid IP address.", ConsoleColor.DarkYellow);
                    return;
                }


                WriteLineColor($"Pinging {vlanName} ({ipAddress})...", ConsoleColor.DarkYellow);

                PingReply reply = Ping(ipAddress);
                if (reply != null)
                {
                    pingResults.Add((vlanName, ipAddress, reply.Status));
                }
            });


            var successfulPings = pingResults.Where(r => r.Status == IPStatus.Success).OrderBy(r => r.VlanName).ToList();

            PrintResultsTable(successfulPings, pingResults.Count);

            Console.Beep(400, 5000);

            if (ShouldExit())
            {
                break;
            }
        }
    }

    static bool ShouldExit()
    {
        WriteColor("\nType 'q' to quit, or press Enter to run another scan: ", ConsoleColor.Yellow);
        string choice = Console.ReadLine();
        return string.Equals(choice?.Trim(), "q", StringComparison.OrdinalIgnoreCase);
    }

    static void PrintResultsTable(
        System.Collections.Generic.IReadOnlyCollection<(string VlanName, string IpAddress, IPStatus Status)> successfulPings,
        int totalProcessed)
    {
        WriteLineColor("\nScan Summary", ConsoleColor.Cyan);
        WriteLineColor($"Processed entries: {totalProcessed}", ConsoleColor.Gray);
        WriteLineColor($"Successful pings: {successfulPings.Count}", ConsoleColor.Gray);

        if (successfulPings.Count == 0)
        {
            WriteLineColor("No successful pings detected.", ConsoleColor.DarkYellow);
            return;
        }

        int vlanWidth = Math.Max("VLAN Name".Length, successfulPings.Max(r => r.VlanName.Length));
        int ipWidth = Math.Max("IP Address".Length, successfulPings.Max(r => r.IpAddress.Length));

        string border = $"+-{new string('-', vlanWidth)}-+-{new string('-', ipWidth)}-+";
        string header = $"| {"VLAN Name".PadRight(vlanWidth)} | {"IP Address".PadRight(ipWidth)} |";

        WriteLineColor("\nSuccessful Pings", ConsoleColor.Green);
        WriteLineColor(border, ConsoleColor.DarkGray);
        WriteLineColor(header, ConsoleColor.Green);
        WriteLineColor(border, ConsoleColor.DarkGray);

        foreach (var result in successfulPings)
        {
            string row = $"| {result.VlanName.PadRight(vlanWidth)} | {result.IpAddress.PadRight(ipWidth)} |";
            WriteLineColor(row, ConsoleColor.Green);
        }

        WriteLineColor(border, ConsoleColor.DarkGray);
    }

    static bool IsValidHeader(string headerLine)
    {
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            return false;
        }

        var parts = headerLine.Split(',');
        if (parts.Length != 2)
        {
            return false;
        }

        string first = parts[0].Trim().ToLowerInvariant();
        string second = parts[1].Trim().ToLowerInvariant();

        bool isVlanHeader = first == "vlan" || first == "vlan name";
        bool isIpHeader = second == "ip" || second == "ip address";

        return isVlanHeader && isIpHeader;
    }

    static void PrintBanner(string toolName)
    {
        string[] bannerLines =
        {
            toolName,
            "VLAN Reachability and Segmentation Validation",
            "Built by Majid Alkindi"
        };

        int innerWidth = Math.Max(84, bannerLines.Max(line => line.Length));
        string border = "+" + new string('-', innerWidth + 2) + "+";

        WriteLineColor(border, ConsoleColor.DarkCyan);
        foreach (string line in bannerLines)
        {
            ConsoleColor color = ConsoleColor.White;
            if (line == toolName)
            {
                color = ConsoleColor.Cyan;
            }
            else if (line.StartsWith("Built by", StringComparison.OrdinalIgnoreCase))
            {
                color = ConsoleColor.Yellow;
            }

            WriteLineColor($"| {CenterText(line, innerWidth)} |", color);
        }
        WriteLineColor(border, ConsoleColor.DarkCyan);
        Console.WriteLine();
    }

    static void WriteLineColor(string text, ConsoleColor color)
    {
        lock (ConsoleLock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }

    static void WriteColor(string text, ConsoleColor color)
    {
        lock (ConsoleLock)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }

    static string CenterText(string text, int width)
    {
        if (text.Length >= width)
        {
            return text;
        }

        int leftPadding = (width - text.Length) / 2;
        int rightPadding = width - text.Length - leftPadding;
        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
    }

    static PingReply Ping(string ipAddress)
    {
        using (Ping pingSender = new Ping())
        {
            PingOptions options = new PingOptions();
            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            try
            {
                PingReply reply = pingSender.Send(ipAddress, timeout, buffer, options);
                return reply;
            }
            catch (Exception ex)
            {
                WriteLineColor($"Error pinging {ipAddress}: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }
    }
}