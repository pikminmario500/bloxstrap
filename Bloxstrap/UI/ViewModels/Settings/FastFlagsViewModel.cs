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

        public int FramerateLimit
        {
            get => int.TryParse(App.FastFlags.GetPreset("Rendering.Framerate1"), out int x) ? x : 0;
            set
            {
                App.FastFlags.SetPreset("Rendering.Framerate1", value == 0 ? null : value);
                App.FastFlags.SetPreset("Rendering.Framerate2", value == 0 ? null : "False");
            }
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
            set => App.FastFlags.SetPresetEnum("Rendering.Mode", RenderingModes[value], "True");
        }

        public bool FixDisplayScaling
        {
            get => App.FastFlags.GetPreset("Rendering.DisableScaling") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisableScaling", value ? "True" : null);
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

        public IReadOnlyDictionary<GraphicsQuality, string?> GraphicsQualities => FastFlagManager.GraphicsQualityLevels;

        public GraphicsQuality SelectedGraphicsQuality
        {
            get => GraphicsQualities.Where(x => x.Value == App.FastFlags.GetPreset("Rendering.GraphicsQuality")).FirstOrDefault().Key;
            set
            {
                if (value == GraphicsQuality.Default)
                    App.FastFlags.SetPreset("Rendering.GraphicsQuality", null);
                else
                    App.FastFlags.SetPreset("Rendering.GraphicsQuality", GraphicsQualities[value]);
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

        public bool DisableGrass
        {
            get => App.FastFlags.GetPreset("Rendering.Grass1") == "0";
            set
            {
                App.FastFlags.SetPreset("Rendering.Grass1", value ? "0" : null);
                App.FastFlags.SetPreset("Rendering.Grass2", value ? "0" : null);
                App.FastFlags.SetPreset("Rendering.Grass3", value ? "0" : null);
            }
        }

        public bool MovePrerender
        {
            get => App.FastFlags.GetPreset("Rendering.MovePrerender") == "True";
            set => App.FastFlags.SetPreset("Rendering.MovePrerender", value ? "True" : null);
        }

        public bool EnableGPULightCulling
        {
            get => App.FastFlags.GetPreset("Rendering.GPULightCulling") == "True";
            set => App.FastFlags.SetPreset("Rendering.GPULightCulling", value ? "True" : null);
        }

        public bool DisableVC
        {
            get => App.FastFlags.GetPreset("Audio.VC") == "False";
            set => App.FastFlags.SetPreset("Audio.VC", value ? "False" : null);
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
    }
}
