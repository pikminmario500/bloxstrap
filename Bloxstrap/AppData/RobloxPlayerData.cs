namespace Bloxstrap.AppData
{
    public class RobloxPlayerData : CommonAppData, IAppData
    {
        public string ProductName => "Roblox";

        public string BinaryType => "WindowsPlayer";

        public string RegistryName => "RobloxPlayer";

        public string ProcessName => "RobloxPlayerBeta";

        public override string ExecutableName => "RobloxPlayerBeta.exe";

        public override JsonManager<DistributionState> DistributionStateManager => App.PlayerState;

        protected override string PackageMapUrl => "https://raw.githubusercontent.com/bloxstraplabs/config/refs/heads/main/package-maps/playerdata.json";
    }
}
