﻿namespace Bloxstrap.AppData
{
    public class RobloxPlayerData : CommonAppData, IAppData
    {
        public string ProductName => "Roblox";

        public string BinaryType => "WindowsPlayer";

        public string RegistryName => "RobloxPlayer";

        public override string ExecutableName => "RobloxPlayerBeta.exe";

        public override string Directory => Path.Combine(Paths.Roblox, "Player");

        public AppState State => App.State.Prop.Player;

        public override IReadOnlyDictionary<string, string> PackageDirectoryMap { get; set; } = new Dictionary<string, string>()
        {
            { "RobloxApp.zip", @"" }
        };
    }
}
