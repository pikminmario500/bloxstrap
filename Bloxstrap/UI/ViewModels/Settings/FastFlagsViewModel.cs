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

        public IReadOnlyDictionary<MSAAMode, string?> MSAALevels => FastFlagManager.MSAAModes;

        public MSAAMode SelectedMSAALevel
        {
            get => MSAALevels.FirstOrDefault(x => x.Value == App.FastFlags.GetPreset("Rendering.MSAA")).Key;
            set => App.FastFlags.SetPreset("Rendering.MSAA", MSAALevels[value]);
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

        public IReadOnlyDictionary<TextureQuality, string?> TextureQualities => FastFlagManager.TextureQualityLevels;

        public TextureQuality SelectedTextureQuality
        {
            get => TextureQualities.Where(x => x.Value == App.FastFlags.GetPreset("Rendering.TextureQuality.Level")).FirstOrDefault().Key;
            set
            {
                if (value == TextureQuality.Default)
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality", null);
                }
                else
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality.OverrideEnabled", "True");
                    App.FastFlags.SetPreset("Rendering.TextureQuality.Level", TextureQualities[value]);
                }
            }
        }

        public bool DisablePostFX
        {
            get => App.FastFlags.GetPreset("Rendering.DisablePostFX") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisablePostFX", value ? "True" : null);
        }

        public bool DisablePlayerShadows
        {
            get => App.FastFlags.GetPreset("Rendering.ShadowIntensity") == "0";
            set => App.FastFlags.SetPreset("Rendering.ShadowIntensity", value ? "0" : null);
        }

        public int? FontSize
        {
            get => int.TryParse(App.FastFlags.GetPreset("UI.FontSize"), out int x) ? x : 1;
            set => App.FastFlags.SetPreset("UI.FontSize", value == 1 ? null : value);
        }

        public bool DisableTerrainTextures
        {
            get => App.FastFlags.GetPreset("Rendering.TerrainTextureQuality") == "0";
            set => App.FastFlags.SetPreset("Rendering.TerrainTextureQuality", value ? "0" : null);
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

        public bool ForceLowQualityEnabled
        {
            get => App.FastFlags.GetPreset("Rendering.ForceLowQuality") == "1";
            set => App.FastFlags.SetPreset("Rendering.ForceLowQuality", value ? "1" : null);
        }

        public bool RemoveGrass
        {
            get => App.FastFlags.GetPreset("Rendering.NoGrass1") == "0";
            set
            {
                App.FastFlags.SetPreset("Rendering.NoGrass1", value ? "0" : null);
                App.FastFlags.CheckGrassPreset();
            }
        }

        public bool PreloadFontsEnabled
        {
            get => App.FastFlags.GetPreset("UI.PreloadFonts") == "True";
            set => App.FastFlags.SetPreset("UI.PreloadFonts", value ? "True" : null);
        }
    }
}
