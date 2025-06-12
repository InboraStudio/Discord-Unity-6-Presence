using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Discord;

namespace UnityDiscordPresence
{
    class Program
    {
        private static Discord.Discord discord;
        private static long startTimestamp;
        private const string APP_ID = "1382561570973945856";

        static void Main(string[] args)
        {
            Console.Title = "Unity Editor Discord Presence";
            Console.WriteLine("🔧 Starting Unity Rich Presence...");

            InitializeDiscordClient();

            while (true)
            {
                try
                {
                    if (discord != null)
                    {
                        discord.RunCallbacks();
                        UpdatePresence();
                    }
                    Thread.Sleep(5000); // Wait 5 seconds before checking again
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    Thread.Sleep(5000); // Wait before retrying on error
                }
            }
        }

        static void InitializeDiscordClient() 
        {
            Log("🔧 Starting Unity Rich Presence...");
            try
            {
                if (!IsDiscordRunning())
                {
                    Console.WriteLine("❌ Discord is not running. Please start Discord first.");
                    Environment.Exit(1);
                }

                startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                discord = new Discord.Discord(long.Parse(APP_ID), (long)CreateFlags.Default);
                Console.WriteLine("✅ Connected to Discord");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Failed to initialize Discord: {e.Message}");
                Environment.Exit(1);
            }
        }

        static void UpdatePresence()
        {
            try
            {
                // Try different Unity process names
                Process unityProcess = null;
                string[] possibleProcessNames = { "Unity", "Unity.exe", "UnityHub", "Unity Hub" };
                
                foreach (string processName in possibleProcessNames)
                {
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length > 0)
                    {
                        unityProcess = processes[0];
                        Console.WriteLine($"Found Unity process: {processName}");
                        break;
                    }
                }

                if (unityProcess != null)
                {
                    // Wait a bit for the window title to be available
                    if (string.IsNullOrEmpty(unityProcess.MainWindowTitle))
                    {
                        Console.WriteLine("Waiting for Unity window title...");
                        Thread.Sleep(1000);
                    }

                    string title = unityProcess.MainWindowTitle;
                    string projectName = ExtractProjectName(title);

                    if (projectName == "Unknown Project")
                    {
                        projectName = GetProjectNameFromRecentlyUsed();
                    }

                    var activity = new Activity
                    {
                        State = "Unity Editor",
                        Details = $"Editing: {projectName}",
                        Timestamps = { Start = startTimestamp },
                        Assets =
                        {
                            LargeImage = "unity_logo",
                            LargeText = "Unity Editor",
                            SmallImage = "csharp",
                            SmallText = "C# Background Tool"
                        },
                        Buttons = new[]
                        {
                            new ActivityButton
                            {
                                Label = "Join",
                                Url = "https://github.com/inborastudio"
                            }
                        }
                    };

                    discord.GetActivityManager().UpdateActivity(activity, result =>
                    {
                        if (result != Result.Ok)
                        {
                            Console.WriteLine($"❌ Failed to update activity: {result}");
                        }
                        else
                        {
                            Console.WriteLine($"✅ Updated Rich Presence: {projectName}");
                        }
                    });
                }
                else
                {
                    // Clear presence if Unity isn't running
                    discord.GetActivityManager().ClearActivity(result =>
                    {
                        if (result != Result.Ok)
                        {
                            Console.WriteLine($"❌ Failed to clear activity: {result}");
                        }
                        else
                        {
                            Console.WriteLine("❌ Unity Editor is not running...");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error updating presence: {ex.Message}");
            }
        }

        static string ExtractProjectName(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Window title is empty or null");
                return "Unknown Project";
            }

            Console.WriteLine($"Extracting project name from title: {title}");
            
            // Try different patterns
            string[] patterns = new[] { " - ", " – ", " — ", " - ", " - " }; // Different types of dashes
            
            foreach (var pattern in patterns)
            {
                var parts = title.Split(new[] { pattern }, StringSplitOptions.None);
                if (parts.Length >= 2)
                {
                    // Try to find the part that looks like a project name
                    foreach (var part in parts)
                    {
                        string trimmed = part.Trim();
                        // Skip Unity version numbers and scene names
                        if (!trimmed.Contains("Unity") && !trimmed.EndsWith(".unity"))
                        {
                            Console.WriteLine($"Found project name: {trimmed}");
                            return trimmed;
                        }
                    }
                }
            }

            // If we couldn't find a project name in the title, try to extract it from the full title
            if (title.Contains("Unity"))
            {
                // Remove Unity version and scene name
                string cleaned = title.Replace("Unity", "").Trim();
                foreach (var pattern in patterns)
                {
                    cleaned = cleaned.Replace(pattern, " ").Trim();
                }
                // Remove scene name if present
                if (cleaned.EndsWith(".unity"))
                {
                    cleaned = cleaned.Substring(0, cleaned.LastIndexOf(".")).Trim();
                }
                if (!string.IsNullOrWhiteSpace(cleaned))
                {
                    Console.WriteLine($"Extracted project name from cleaned title: {cleaned}");
                    return cleaned;
                }
            }

            Console.WriteLine("Could not extract project name from title");
            return "Unknown Project";
        }

        static string GetProjectNameFromRecentlyUsed()
        {
            try
            {
                // Try different Unity versions
                string[] unityVersions = { "2022.3", "2021.3", "2020.3", "2019.4", "2023.1", "2023.2" };
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                foreach (string version in unityVersions)
                {
                    string path = Path.Combine(appDataPath, "Unity", $"Editor-{version}", "RecentlyUsedAssetPaths.txt");
                    Console.WriteLine($"Checking path: {path}");
                    
                    if (File.Exists(path))
                    {
                        Console.WriteLine($"Found RecentlyUsedAssetPaths.txt for Unity {version}");
                        string[] lines = File.ReadAllLines(path);
                        if (lines.Length > 0)
                        {
                            string lastPath = lines[0].Trim();
                            Console.WriteLine($"Most recent path: {lastPath}");
                            string projectName = Path.GetFileName(Path.GetDirectoryName(lastPath));
                            Console.WriteLine($"Extracted project name: {projectName}");
                            return projectName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error reading RecentlyUsedAssetPaths: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            return "Unknown Project";
        }

        static bool IsDiscordRunning()
        {
            Process[] processes = Process.GetProcessesByName("Discord");
            if (processes.Length == 0)
            {
                processes = Process.GetProcessesByName("DiscordPTB");
                if (processes.Length == 0)
                {
                    processes = Process.GetProcessesByName("DiscordCanary");
                }
            }
            return processes.Length != 0;
        }
        private static void Log(string message)
        {
            File.AppendAllText("UnityDiscordPresence.log", $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
