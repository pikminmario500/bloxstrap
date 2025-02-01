using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using Bloxstrap.Enums.FlagPresets;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class FastFlagsViewModel : NotifyPropertyChangedViewModel
    {
        private Dictionary<string, object>? _preResetFlags;

        public event EventHandler? RequestPageReloadEvent;
        
        public event EventHandler? OpenFlagEditorEvent;

        private void OpenFastFlagEditor() => OpenFlagEditorEvent?.Invoke(this, EventArgs.Empty);

        public ICommand OpenFastFlagEditorCommand => new RelayCommand(OpenFastFlagEditor);

        public bool UseFastFlagManager
        {
            get => App.Settings.Prop.UseFastFlagManager;
            set => App.Settings.Prop.UseFastFlagManager = value;
        }

        public IReadOnlyDictionary<RenderingMode, string> RenderingModes => FastFlagManager.RenderingModes;

        public RenderingMode SelectedRenderingMode
        {
            get => App.FastFlags.GetPresetEnum(RenderingModes, "Rendering.Mode", "True");
            set
            {
                App.FastFlags.SetPresetEnum("Rendering.Mode", RenderingModes[value], "True");
                App.FastFlags.CheckManualFullscreenPreset();
            }
        }

        public bool FixDisplayScaling
        {
            get => App.FastFlags.GetPreset("Rendering.DisableScaling") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisableScaling", value ? "True" : null);
        }

        //public IReadOnlyDictionary<InGameMenuVersion, Dictionary<string, string?>> IGMenuVersions => FastFlagManager.IGMenuVersions;

        //public InGameMenuVersion SelectedIGMenuVersion
        //{
        //    get
        //    {
        //        // yeah this kinda sucks
        //        foreach (var version in IGMenuVersions)
        //        {
        //            bool flagsMatch = true;

        //            foreach (var flag in version.Value)
        //            {
        //                foreach (var presetFlag in FastFlagManager.PresetFlags.Where(x => x.Key.StartsWith($"UI.Menu.Style.{flag.Key}")))
        //                { 
        //                    if (App.FastFlags.GetValue(presetFlag.Value) != flag.Value)
        //                        flagsMatch = false;
        //                }
        //            }

        //            if (flagsMatch)
        //                return version.Key;
        //        }

        //        return IGMenuVersions.First().Key;
        //    }

        //    set
        //    {
        //        foreach (var flag in IGMenuVersions[value])
        //            App.FastFlags.SetPreset($"UI.Menu.Style.{flag.Key}", flag.Value);
        //    }
        //}

        public IReadOnlyDictionary<LightingMode, string> LightingModes => FastFlagManager.LightingModes;

        public LightingMode SelectedLightingMode
        {
            get => App.FastFlags.GetPresetEnum(LightingModes, "Rendering.Lighting", "True");
            set => App.FastFlags.SetPresetEnum("Rendering.Lighting", LightingModes[value], "True");
        }

        public bool FullscreenTitlebarDisabled
        {
            get => int.TryParse(App.FastFlags.GetPreset("UI.FullscreenTitlebarDelay"), out int x) && x > 5000;
            set => App.FastFlags.SetPreset("UI.FullscreenTitlebarDelay", value ? "3600000" : null);
        }

        public bool GuiHidingEnabled
        {
            get => App.FastFlags.GetPreset("UI.Hide") == "32380007";
            set => App.FastFlags.SetPreset("UI.Hide", value ? "32380007" : null);
        }

        public bool DisablePostFX
        {
            get => App.FastFlags.GetPreset("Rendering.DisablePostFX") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisablePostFX", value ? "True" : null);
        }

        public bool ResetConfiguration
        {
            get => _preResetFlags is not null;

            set
            {
                if (value)
                {
                    _preResetFlags = new(App.FastFlags.Prop);
                    App.FastFlags.Prop.Clear();
                }
                else
                {
                    App.FastFlags.Prop = _preResetFlags!;
                    _preResetFlags = null;
                }

                RequestPageReloadEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        // divider

        public bool DisableTelemetry
        {
            get => App.FastFlags.GetPreset("Network.DisableTelemetry1") == "True";
            set
            {
                App.FastFlags.SetPreset("Network.DisableTelemetry1", value ? "True" : null);
                App.FastFlags.CheckTelemetryPreset();
            }
        }

        public bool DisableVC
        {
            get => App.FastFlags.GetPreset("Audio.DisableVC") == "False";
            set => App.FastFlags.SetPreset("Audio.DisableVC", value ? "False" : null);
        }

        public bool HyperThreadingEnabled
        {
            get => App.FastFlags.GetPreset("Rendering.HyperThreading1") == "True";
            set
            {
                App.FastFlags.SetPreset("Rendering.HyperThreading1", value ? "True" : null);
                App.FastFlags.CheckHyperThreadingPreset();
            }
        }

        public bool OcclusionCullingEnabled
        {
            get => App.FastFlags.GetPreset("Rendering.OcclusionCulling1") == "True";
            set
            {
                App.FastFlags.SetPreset("Rendering.OcclusionCulling1", value ? "True" : null);
                App.FastFlags.CheckOcclusionCullingPreset();
            }
        }

        public bool BetterPreloadingEnabled
        {
            get => App.FastFlags.GetPreset("Rendering.BetterPreloading") == "2147483647";
            set => App.FastFlags.SetPreset("Rendering.BetterPreloading", value ? "2147483647" : null);
        }

        public bool UseGPUForLightingEnabled
        {
            get => App.FastFlags.GetPreset("Rendering.Lighting.UseGPU") == "True";
            set => App.FastFlags.SetPreset("Rendering.Lighting.UseGPU", value ? "True" : null);
        }

        public bool MovePrerender
        {
            get => App.FastFlags.GetPreset("Rendering.MovePrerender") == "True";
            set => App.FastFlags.SetPreset("Rendering.MovePrerender", value ? "True" : null);
        }

        public bool PreloadFontsEnabled
        {
            get => App.FastFlags.GetPreset("UI.PreloadFonts") == "True";
            set => App.FastFlags.SetPreset("UI.PreloadFonts", value ? "True" : null);
        }

        public bool DisableBetaBadge
        {
            get => App.FastFlags.GetPreset("UI.VCBetaBadge1") == "False";
            set {
                App.FastFlags.SetPreset("UI.VCBetaBadge1", value ? "False" : null);
                App.FastFlags.CheckVCBetaBadgePreset();
            }
        }
    }
}
