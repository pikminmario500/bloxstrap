namespace Bloxstrap.AppData
{
    public class RobloxStudioData : CommonAppData, IAppData
    {
        public string ProductName => "Roblox Studio";

        public string BinaryType => "WindowsStudio64";

        public string RegistryName => "RobloxStudio";

        public string ProcessName => "RobloxStudioBeta";

        public override string ExecutableName => "RobloxStudioBeta.exe";

        public override JsonManager<DistributionState> DistributionStateManager => App.StudioState;

        protected override string PackageMapUrl => "https://raw.githubusercontent.com/bloxstraplabs/config/refs/heads/main/package-maps/studiodata.json";
    }
}
