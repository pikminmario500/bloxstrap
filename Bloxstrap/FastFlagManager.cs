using Bloxstrap.Enums.FlagPresets;

namespace Bloxstrap
{
    public class FastFlagManager : JsonManager<Dictionary<string, object>>
    {
        public override string ClassName => nameof(FastFlagManager);

        public override string LOG_IDENT_CLASS => ClassName;
        
        public override string FileLocation => Path.Combine(Paths.Modifications, "ClientSettings\\ClientAppSettings.json");

        public bool Changed => !OriginalProp.SequenceEqual(Prop);

        public static IReadOnlyDictionary<string, string> PresetFlags = new Dictionary<string, string>
        {
            { "Rendering.Framerate", "DFIntTaskSchedulerTargetFps" },
            { "Rendering.ManualFullscreen", "FFlagHandleAltEnterFullscreenManually" },
            { "Rendering.DisableScaling", "DFFlagDisableDPIScale" },
            { "Rendering.MSAA", "FIntDebugForceMSAASamples" },
            { "Rendering.DisablePostFX", "FFlagDisablePostFx" },
            { "Rendering.ShadowIntensity", "FIntRenderShadowIntensity" },

            { "Rendering.Mode.D3D11", "FFlagDebugGraphicsPreferD3D11" },
            { "Rendering.Mode.D3D10", "FFlagDebugGraphicsPreferD3D11FL10" },

            { "Rendering.Lighting.Voxel", "DFFlagDebugRenderForceTechnologyVoxel" },
            { "Rendering.Lighting.ShadowMap", "FFlagDebugForceFutureIsBrightPhase2" },
            { "Rendering.Lighting.Future", "FFlagDebugForceFutureIsBrightPhase3" },

            { "Rendering.TextureQuality.OverrideEnabled", "DFFlagTextureQualityOverrideEnabled" },
            { "Rendering.TextureQuality.Level", "DFIntTextureQualityOverride" },
            { "Rendering.TerrainTextureQuality", "FIntTerrainArraySliceSize" },

            { "UI.Hide", "DFIntCanHideGuiGroupId" },
            { "UI.FontSize", "FIntFontSizePadding" },

            { "UI.FullscreenTitlebarDelay", "FIntFullscreenTitleBarTriggerDelayMillis" },
            
            //{ "UI.Menu.Style.V2Rollout", "FIntNewInGameMenuPercentRollout3" },
            //{ "UI.Menu.Style.EnableV4.1", "FFlagEnableInGameMenuControls" },
            //{ "UI.Menu.Style.EnableV4.2", "FFlagEnableInGameMenuModernization" },
            //{ "UI.Menu.Style.EnableV4Chrome", "FFlagEnableInGameMenuChrome" },
            //{ "UI.Menu.Style.ReportButtonCutOff", "FFlagFixReportButtonCutOff" },


            //{ "UI.Menu.Style.ABTest.1", "FFlagEnableMenuControlsABTest" },
            //{ "UI.Menu.Style.ABTest.2", "FFlagEnableV3MenuABTest3" },
            //{ "UI.Menu.Style.ABTest.3", "FFlagEnableInGameMenuChromeABTest3" },
            //{ "UI.Menu.Style.ABTest.4", "FFlagEnableInGameMenuChromeABTest4" },

            // divider

            { "Network.DisableTelemetry1", "FFlagDebugDisableTelemetryEphemeralCounter" },
            { "Network.DisableTelemetry2", "FFlagDebugDisableTelemetryEphemeralStat" },
            { "Network.DisableTelemetry3", "FFlagDebugDisableTelemetryEventIngest" },
            { "Network.DisableTelemetry4", "FFlagDebugDisableTelemetryPoint" },
            { "Network.DisableTelemetry5", "FFlagDebugDisableTelemetryV2Counter" },
            { "Network.DisableTelemetry6", "FFlagDebugDisableTelemetryV2Event" },
            { "Network.DisableTelemetry7", "FFlagDebugDisableTelemetryV2Stat" },

            { "Audio.DisableVC", "DFFlagVoiceChat4" },

            { "Rendering.DisableD3D", "FFlagDebugGraphicsDisableDirect3D11" },
            { "Rendering.Mode.Metal", "FFlagDebugGraphicsPreferMetal" },
            { "Rendering.Mode.Vulkan", "FFlagDebugGraphicsPreferVulkan" },
            { "Rendering.Mode.OpenGL", "FFlagDebugGraphicsPreferOpenGL" },

            { "Rendering.HyperThreading1", "FFlagDebugCheckRenderThreading" },
            { "Rendering.HyperThreading2", "FFlagRenderDebugCheckThreading2" },

            { "Rendering.OcclusionCulling1", "DFFlagUseVisBugChecks" },
            { "Rendering.OcclusionCulling2", "FFlagEnableVisBugChecks27" },
            { "Rendering.OcclusionCulling3", "FFlagVisBugChecksThreadYield" },
            { "Rendering.OcclusionCulling4", "FIntEnableVisBugChecksHundredthPercent27" },

            { "Rendering.BetterPreloading", "DFIntNumAssetsMaxToPreload" },
            { "Rendering.Lighting.UseGPU", "FFlagFastGPULightCulling3" },
            { "Rendering.MovePrerender", "FFlagMovePrerender" },

            { "Rendering.ForceLowQuality", "DFIntDebugFRMQualityLevelOverride" },
            { "Rendering.NoGrass1", "FIntFRMMinGrassDistance" },
            { "Rendering.NoGrass2", "FIntFRMMaxGrassDistance" },
            { "Rendering.NoGrass3", "FIntRenderGrassDetailStrands" },

            { "UI.PreloadFonts", "FFlagPreloadAllFonts" }
        };

        public static IReadOnlyDictionary<RenderingMode, string> RenderingModes => new Dictionary<RenderingMode, string>
        {
            { RenderingMode.Default, "None" },
            { RenderingMode.Vulkan, "Vulkan" },
            { RenderingMode.D3D11, "D3D11" },
            { RenderingMode.D3D10, "D3D10" },
            { RenderingMode.OpenGL, "OpenGL" },
            { RenderingMode.Metal, "Metal" }
        };

        public static IReadOnlyDictionary<LightingMode, string> LightingModes => new Dictionary<LightingMode, string>
        {
            { LightingMode.Default, "None" },
            { LightingMode.Voxel, "Voxel" },
            { LightingMode.ShadowMap, "ShadowMap" },
            { LightingMode.Future, "Future" }
        };

        public static IReadOnlyDictionary<MSAAMode, string?> MSAAModes => new Dictionary<MSAAMode, string?>
        {
            { MSAAMode.Default, null },
            { MSAAMode.x1, "1" },
            { MSAAMode.x2, "2" },
            { MSAAMode.x4, "4" }
        };

        public static IReadOnlyDictionary<TextureQuality, string?> TextureQualityLevels => new Dictionary<TextureQuality, string?>
        {
            { TextureQuality.Default, null },
            { TextureQuality.Level0, "0" },
            { TextureQuality.Level1, "1" },
            { TextureQuality.Level2, "2" },
            { TextureQuality.Level3, "3" },
        };

        // this is one hell of a dictionary definition lmao
        // since these all set the same flags, wouldn't making this use bitwise operators be better?
        //public static IReadOnlyDictionary<InGameMenuVersion, Dictionary<string, string?>> IGMenuVersions => new Dictionary<InGameMenuVersion, Dictionary<string, string?>>
        //{
        //    {
        //        InGameMenuVersion.Default,
        //        new Dictionary<string, string?>
        //        {
        //            { "V2Rollout", null },
        //            { "EnableV4", null },
        //            { "EnableV4Chrome", null },
        //            { "ABTest", null },
        //            { "ReportButtonCutOff", null }
        //        }
        //    },

        //    {
        //        InGameMenuVersion.V1,
        //        new Dictionary<string, string?>
        //        {
        //            { "V2Rollout", "0" },
        //            { "EnableV4", "False" },
        //            { "EnableV4Chrome", "False" },
        //            { "ABTest", "False" },
        //            { "ReportButtonCutOff", "False" }
        //        }
        //    },

        //    {
        //        InGameMenuVersion.V2,
        //        new Dictionary<string, string?>
        //        {
        //            { "V2Rollout", "100" },
        //            { "EnableV4", "False" },
        //            { "EnableV4Chrome", "False" },
        //            { "ABTest", "False" },
        //            { "ReportButtonCutOff", null }
        //        }
        //    },

        //    {
        //        InGameMenuVersion.V4,
        //        new Dictionary<string, string?>
        //        {
        //            { "V2Rollout", "0" },
        //            { "EnableV4", "True" },
        //            { "EnableV4Chrome", "False" },
        //            { "ABTest", "False" },
        //            { "ReportButtonCutOff", null }
        //        }
        //    },

        //    {
        //        InGameMenuVersion.V4Chrome,
        //        new Dictionary<string, string?>
        //        {
        //            { "V2Rollout", "0" },
        //            { "EnableV4", "True" },
        //            { "EnableV4Chrome", "True" },
        //            { "ABTest", "False" },
        //            { "ReportButtonCutOff", null }
        //        }
        //    }
        //};

        // all fflags are stored as strings
        // to delete a flag, set the value as null
        public void SetValue(string key, object? value)
        {
            const string LOG_IDENT = "FastFlagManager::SetValue";

            if (value is null)
            {
                if (Prop.ContainsKey(key))
                    App.Logger.WriteLine(LOG_IDENT, $"Deletion of '{key}' is pending");

                Prop.Remove(key);
            }
            else
            {
                if (Prop.ContainsKey(key))
                {
                    if (key == Prop[key].ToString())
                        return;

                    App.Logger.WriteLine(LOG_IDENT, $"Changing of '{key}' from '{Prop[key]}' to '{value}' is pending");
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Setting of '{key}' to '{value}' is pending");
                }

                Prop[key] = value.ToString()!;
            }
        }

        // this returns null if the fflag doesn't exist
        public string? GetValue(string key)
        {
            // check if we have an updated change for it pushed first
            if (Prop.TryGetValue(key, out object? value) && value is not null)
                return value.ToString();

            return null;
        }

        public void SetPreset(string prefix, object? value)
        {
            foreach (var pair in PresetFlags.Where(x => x.Key.StartsWith(prefix)))
                SetValue(pair.Value, value);
        }

        public void SetPresetEnum(string prefix, string target, object? value)
        {
            foreach (var pair in PresetFlags.Where(x => x.Key.StartsWith(prefix)))
            {
                if (pair.Key.StartsWith($"{prefix}.{target}"))
                    SetValue(pair.Value, value);
                else
                    SetValue(pair.Value, null);
            }
        }

        public string? GetPreset(string name) => GetValue(PresetFlags[name]);

        public T GetPresetEnum<T>(IReadOnlyDictionary<T, string> mapping, string prefix, string value) where T : Enum
        {
            foreach (var pair in mapping)
            {
                if (pair.Value == "None")
                    continue;

                if (GetPreset($"{prefix}.{pair.Value}") == value)
                    return pair.Key;
            }

            return mapping.First().Key;
        }

        public void CheckManualFullscreenPreset()
        {
            if (GetPreset("Rendering.Mode.Vulkan") == "True" || GetPreset("Rendering.Mode.OpenGL") == "True")
            {
                SetPreset("Rendering.ManualFullscreen", null);
                SetPreset("Rendering.DisableD3D", "True");
            }
            else
            {
                SetPreset("Rendering.ManualFullscreen", "False");
                SetPreset("Rendering.DisableD3D", null);
            }
        }

        public void CheckTelemetryPreset()
        {
            if (GetPreset("Network.DisableTelemetry1") == "True")
            {
                SetPreset("Network.DisableTelemetry2", "True");
                SetPreset("Network.DisableTelemetry3", "True");
                SetPreset("Network.DisableTelemetry4", "True");
                SetPreset("Network.DisableTelemetry5", "True");
                SetPreset("Network.DisableTelemetry6", "True");
                SetPreset("Network.DisableTelemetry7", "True");
            }
            else
            {
                SetPreset("Network.DisableTelemetry2", null);
                SetPreset("Network.DisableTelemetry3", null);
                SetPreset("Network.DisableTelemetry4", null);
                SetPreset("Network.DisableTelemetry5", null);
                SetPreset("Network.DisableTelemetry6", null);
                SetPreset("Network.DisableTelemetry7", null);
            }
        }

        public void CheckHyperThreadingPreset()
        {
            if (GetPreset("Rendering.HyperThreading1") == "True")
                SetPreset("Rendering.HyperThreading2", "True");
            else
                SetPreset("Rendering.HyperThreading2", null);
        }

        public void CheckOcclusionCullingPreset()
        {
            if (GetPreset("Rendering.OcclusionCulling1") == "True")
            {
                SetPreset("Rendering.OcclusionCulling2", "True");
                SetPreset("Rendering.OcclusionCulling3", "True");
                SetPreset("Rendering.OcclusionCulling4", "100");
            }
            else
            {
                SetPreset("Rendering.OcclusionCulling2", null);
                SetPreset("Rendering.OcclusionCulling3", null);
                SetPreset("Rendering.OcclusionCulling4", null);
            }
        }

        public void CheckGrassPreset()
        {
            if (GetPreset("Rendering.NoGrass1") == "0")
            {
                SetPreset("Rendering.NoGrass2", "0");
                SetPreset("Rendering.NoGrass3", "0");
            }
            else
            {
                SetPreset("Rendering.NoGrass2", null);
                SetPreset("Rendering.NoGrass3", null);
            }
        }

        public override void Save()
        {
            // convert all flag values to strings before saving

            foreach (var pair in Prop)
                Prop[pair.Key] = pair.Value.ToString()!;

            base.Save();

            // clone the dictionary
            OriginalProp = new(Prop);
        }

        public override void Load(bool alertFailure = true)
        {
            base.Load(alertFailure);

            // clone the dictionary
            OriginalProp = new(Prop);

            CheckManualFullscreenPreset();
            CheckTelemetryPreset();
            CheckHyperThreadingPreset();
            CheckOcclusionCullingPreset();
            CheckGrassPreset();
        }
    }
}
