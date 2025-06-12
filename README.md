# Discord-Unity-6-Presence-
A Discord Unity Presence to show you'r useing Unity Engine on Your Discord Profile

A lightweight Windows C# console application that displays custom [Discord Rich Presence](https://discord.com/rich-presence) when the Unity Editor is running. It automatically detects the active Unity project and updates the presence with real-time project details.

## Features

- Detects if Unity Editor is running
- Extracts the currently open Unity project name from:
  - The Unity Editor window title, or
  - Unity's `RecentlyUsedAssetPaths.txt` metadata
- Displays detailed Rich Presence in Discord, including:
  - Current project name
  - Custom images (e.g., Unity logo)
  - Timestamp (when app started)
  - Button linking to GitHub or any external site
- Automatically clears the presence when Unity is closed
- Updates every 5 seconds

## Example Display

![image](https://github.com/user-attachments/assets/58ba1343-9e01-4b10-8cc0-e841495051df)



```
Unity Editor
Editing: MyUnityProject
[Join] (button open!
s GitHub)
```

## Requirements

- Windows OS
- [.NET Framework 4.7.2+ or .NET Core 3.1+/6+](https://dotnet.microsoft.com/en-us/download)
- [Discord Desktop App](https://discord.com/download)
- Discord Developer Application (App ID and Assets configured)
- [Lachee’s DiscordRPC library](https://github.com/Lachee/discord-rpc-csharp)

## Setup

**Directly Run .Exe**

   ```bash
   download the release file and open Bin folder and the tun Unity Discoird Presene.exe
   ```

1. **Clone the repository**

   ```bash
   git clone https://github.com/InboraStudio/Discord-Unity-6-Presence.git
   cd unity-discord-presence
   ```

2. **Open the solution in Visual Studio**

3. **Install the NuGet package**

   - Right-click the project → `Manage NuGet Packages`
   - Install: `DiscordRPC` by `Lachee`

4. **Add your Discord Application ID**

   Replace the `discordAppID` in `Program.cs`:

   ```csharp
   string discordAppID = "YOUR_DISCORD_APP_ID";
   ```

5. **Add your Rich Presence assets**

   - Go to [Discord Developer Portal](https://discord.com/developers/applications)
   - Upload image assets under `Rich Presence > Art Assets`
   - Use the keys (names) in `LargeImageKey` and `SmallImageKey`

6. **Build and run the executable**

   - Press `F5` or build it as a `.exe`
   - Leave it running in the background while using Unity

## Customization

You can modify:
- Button label and URL
- Update frequency
- Presence text styling
- Additional logic (e.g., current scene detection)

## Known Limitations

- Only works on Windows
- Unity project detection may be inaccurate if Unity is still loading or minimized
- Requires Discord desktop client to be running

## License

MIT License. See [LICENSE](LICENSE) for details.

## Credits

- Built using [Lachee's DiscordRPC](https://github.com/Lachee/discord-rpc-csharp)
- Inspired by the need for better Unity activity tracking on Discord
