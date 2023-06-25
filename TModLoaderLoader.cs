
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using SkillTreeBoons.SkillTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;


namespace SkillTreeBoons
{
    public class TModLoaderLoader
    {
        public static Dictionary<string, Dictionary<string, List<ConfigEntry>>> configdict;
        public static List<ConfigEntry> currentConfigEntry;
        public static void addConfigEntry(string mod, string config, string name, int index = -1, bool hidden = false, object value = null)
        {
            if(!configdict.ContainsKey(mod))
            {
                configdict.Add(mod, new Dictionary<string,List<ConfigEntry>>());
            }
            if (!configdict[mod].ContainsKey(config))
            {
                configdict[mod].Add(config, new List<ConfigEntry>());
            }
            object[] newval = null;
            if (value != null)
            {
                newval = new object[] { value };
            }
           
            configdict[mod][config].Add(new ConfigEntry(name, index, hidden, newval));
        }

        public void AddConfigEntries()
        {
            configdict = new Dictionary<string, Dictionary<string, List<ConfigEntry>>>();
            //ORE EXCAVATOR
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Server", "showWelcome", -1, false, false);
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Server", "oreMultiplier", -1, true, 1);
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Server", "teleportLoot", -1, true, false);
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Server", "safeItems", -1, true, false);
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Server", "creativeMode", -1, true, false);
            
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Client", "doSpecials", -1, true, false);
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Client", "refillMana", -1, true, false);
            addConfigEntry("OreExcavator", "OreExcavatorConfig_Client", "showWelcome080", -1, false, false);
            //WINGSLOT
            addConfigEntry("WingSlotExtra", "WingSlotConfig", "AllowAccessorySlots", -1, true, true);
            //CALAMITY VANITIES
            addConfigEntry("CalValEX", "CalValEXConfig", "GroundMountLol", -1, true, false);
            //CALAMITY MOD
            addConfigEntry("CalamityMod", "CalamityConfig", "WikiStatusMessage", -1, false, false);
            addConfigEntry("CalamityMod", "CalamityConfig", "Header", 34, true, null);
            addConfigEntry("CalamityMod", "CalamityConfig", "FasterBaseSpeed", -1, true, false);
            addConfigEntry("CalamityMod", "CalamityConfig", "HigherJumpHeight", -1, true, false);
            addConfigEntry("CalamityMod", "CalamityConfig", "FasterJumpSpeed", -1, true, false);
            addConfigEntry("CalamityMod", "CalamityConfig", "FasterTilePlacement", -1, true, false);
            addConfigEntry("CalamityMod", "CalamityConfig", "FasterFallHotkey", -1, true, false);
            addConfigEntry("CalamityMod", "CalamityConfig", "EarlyHardmodeProgressionRework", -1, true, true);
        }
        public void Init()
        {
            AddConfigEntries();
        }
        public void ModifyConfig()
        {
            foreach (string mod in configdict.Keys)
            {
                if (!ModLoader.HasMod(mod)) continue;
                object[] args = new object[] { ModLoader.GetMod(mod), null };
                foreach (string config in configdict[mod].Keys)
                {
                    args[1] = config;
                    
                    object configObj = typeof(ConfigManager).GetMethod("GetConfig", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(Mod), typeof(string) }).Invoke(null, args);
                    if(configObj == null) continue;
                    foreach (ConfigEntry entry in configdict[mod][config])
                    {


                        //SkillTreeBoons.Instance.Logger.Debug($"Setting {mod} {config} {entry.name} to {entry.value[0]}");
                        var property = configObj.GetType().GetProperty(entry.name);
                        if (property == null) {
                            var field = configObj.GetType().GetField(entry.name);
                            if(field == null)
                            {
                                SkillTreeBoons.Instance.Logger.Debug($"Could not find {mod} {config} {entry.name}");
                                continue;
                            }
                            field.SetValue(configObj, entry.value[0]);
                            continue;
                        }

                        if (entry.value == null) continue;
                        
                        property.GetSetMethod().Invoke(configObj, entry.value);
                    }
                    typeof(ConfigManager).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { configObj });
                    typeof(ConfigManager).GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { configObj });
                }

            }
        }
        public void Load()
        {
            
            Assembly modloaderAssembly = typeof(ModLoader).Assembly;
            Type UIModConfigType = modloaderAssembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig");
            if (UIModConfigType != null)
            {
                MethodInfo WrapIt = UIModConfigType.GetMethod("WrapIt", BindingFlags.Public | BindingFlags.Static);
                if (WrapIt != null) HookEndpointManager.Modify(WrapIt, ChangeWrapIt);
                MethodInfo Activate = UIModConfigType.GetMethod("OnActivate", BindingFlags.Public | BindingFlags.Instance);
                if (WrapIt != null) HookEndpointManager.Modify(Activate, ChangeActivate);
            }
            Type UIWorldCreation = typeof(Terraria.GameContent.UI.States.UIWorldCreation);
            if (UIWorldCreation != null)
            {
                MethodInfo AddWorldDifficultyOptions = UIWorldCreation.GetMethod("AddWorldDifficultyOptions", BindingFlags.NonPublic | BindingFlags.Instance);
                if (AddWorldDifficultyOptions != null) HookEndpointManager.Modify(AddWorldDifficultyOptions, ChangeAddWorldDifficultyOptions);
                MethodInfo ClickDifficultyOption = UIWorldCreation.GetMethod("ClickDifficultyOption", BindingFlags.NonPublic | BindingFlags.Instance);
                if (ClickDifficultyOption != null) HookEndpointManager.Modify(ClickDifficultyOption, ChangeClickDifficultyOption);
                MethodInfo SetDefaultOptions = UIWorldCreation.GetMethod("SetDefaultOptions", BindingFlags.NonPublic | BindingFlags.Instance);
                if (SetDefaultOptions != null) HookEndpointManager.Modify(SetDefaultOptions, ChangeSetDefaultOptions);
            }
            Type UIWorldListItem = typeof(UIWorldListItem);
            if(UIWorldListItem != null)
            {
                MethodInfo TryMovingToRejectionMenuIfNeeded = UIWorldListItem.GetMethod("TryMovingToRejectionMenuIfNeeded", BindingFlags.NonPublic | BindingFlags.Instance);
                if (TryMovingToRejectionMenuIfNeeded != null) HookEndpointManager.Modify(TryMovingToRejectionMenuIfNeeded, ChangeTryMovingToRejectionMenuIfNeeded);
            }
            Type UICharacterListItem = typeof(UICharacterListItem);
            if (UICharacterListItem != null)
            {
                MethodInfo PlayGame = UICharacterListItem.GetMethod("PlayGame", BindingFlags.NonPublic | BindingFlags.Instance);
                if (PlayGame != null) HookEndpointManager.Modify(PlayGame, ChangePlayGame);
            }
            Type Player = typeof(Player);
            if(Player != null)
            {
                //MethodInfo ItemCheck_UseDemonHeart = Player.GetMethod("ItemCheck_UseDemonHeart", BindingFlags.NonPublic | BindingFlags.Instance);
                //if (ItemCheck_UseDemonHeart != null) HookEndpointManager.Modify(ItemCheck_UseDemonHeart, ChangeItemCheck_UseDemonHeart);
                MethodInfo IsAValidEquipmentSlotForIteration = Player.GetMethod("IsAValidEquipmentSlotForIteration", BindingFlags.Public | BindingFlags.Instance);
                if (IsAValidEquipmentSlotForIteration != null) HookEndpointManager.Modify(IsAValidEquipmentSlotForIteration, ChangeIsAValidEquipmentSlotForIteration);
            }

        }
        private void ChangeIsAValidEquipmentSlotForIteration(ILContext il)
        {
            var c = new ILCursor(il);
            var label = il.DefineLabel();
            c.EmitDelegate(() =>
            {
                return SkillTreeBoonsConfig.Instance.demonHeartDisabled;
            });
            c.Emit(OpCodes.Brfalse, label);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate((Player player, int i) =>
            {
                SkillTreeBoonsPlayer modplayer = player.GetModPlayer<SkillTreeBoonsPlayer>();
                if(modplayer == null)
                {
                    return true;
                }
                switch (i)
                {
                    default:
                        return true;
                    case 8:
                    case 18:
                        {
                            bool result = true;
                            if (!Main.gameMenu && !modplayer.demonheartAccessory)
                            {
                                result = false;
                            }
                            return result;
                        }
                    case 9:
                    case 19:
                        {
                            bool result = true;
                            if (!Main.gameMenu && !modplayer.masterModeAccessory)
                            {
                                result = false;
                            }
                            return result;
                        }
                }
            });
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);

        }
        /*private void ChangeItemCheck_UseDemonHeart(ILContext il)
        {
            MethodInfo method2 = typeof(Player).GetMethod("ApplyItemTime", BindingFlags.Public | BindingFlags.Instance);
            var c = new ILCursor(il);
            var label = il.DefineLabel();
            
            c.GotoNext(x => x.MatchCall(method2));
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate((Player player, Item item) =>
            {
                if (SkillTreeBoonsConfig.Instance.demonHeartDisabled)
                {
                    Main.NewText("DEMON HEART IS DISABLED FOR BALANCE", Color.Red);
                    Main.NewText("Can be enabled in config", Color.Red);
                    return true;
                }
                
                return false;
            });
            c.Emit(OpCodes.Brfalse, label);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);

        }*/
        private void ChangePlayGame(ILContext il)
        {
            FieldInfo field = typeof(UICharacterListItem).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance);
            var c = new ILCursor(il);
            var label = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld,field);
            c.EmitDelegate<Func<Terraria.IO.PlayerFileData, bool>>((data) =>
            {
                if(data.Player.difficulty == 3) {
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                    Main.statusText = Language.GetTextValue("Mods.SkillTreeBoons.UI.RejectPlayerMessage");
                    Main.menuMode = 1000001;
                    return true;
                }
                return false;
            });
            c.Emit(OpCodes.Brfalse, label);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        }
        private void ChangeTryMovingToRejectionMenuIfNeeded(ILContext il)
        {

            var c = new ILCursor(il);
            var label = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<int, bool>>((i) =>
            {
                if (!SkillTreeBoonsConfig.Instance.enforceDifficulty) return false;
                Main.statusText = "";
                if (i == 3 || i == 0)
                {
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                    Main.statusText = Language.GetTextValue("Mods.SkillTreeBoons.UI.RejectWorldMessage");
                    Main.menuMode = 1000000;
                    return true;
                }
                return false;
            });
            c.Emit(OpCodes.Brfalse, label);
            c.Emit(OpCodes.Ldc_I4_1);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        }
        private void ChangeSetDefaultOptions(ILContext il)
        {
            Type worldDiffucultyID = typeof(UIWorldCreation).GetNestedType("WorldDifficultyId", BindingFlags.NonPublic);
            FieldInfo field = typeof(UIWorldCreation).GetField("_optionDifficulty", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo UpdatePreviewPlate = typeof(UIWorldCreation).GetMethod("UpdatePreviewPlate", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo getDefault = typeof(TModLoaderLoader).GetMethod("GetDefaultDifficultyOption", BindingFlags.Public | BindingFlags.Static);
            MethodInfo disablePlayerMode = typeof(TModLoaderLoader).GetMethod("DisablePlayerModeCheck", BindingFlags.Public | BindingFlags.Static);

            var c = new ILCursor(il);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, getDefault);
            c.Emit(OpCodes.Stfld, field);
            c.GotoNext(i => i.MatchLdcI4(3));
            c.Index++;
            
            c.Emit(OpCodes.Call, disablePlayerMode);
            
            c.GotoNext(i=> i.MatchLdelemRef(), i=> i.MatchLdcI4(0));
            c.Index++;
            c.Index++;
           
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Call, getDefault);

            c.GotoNext(i=>i.MatchStloc(1));

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, UpdatePreviewPlate);


            //SkillTreeBoons.Instance.Logger.Debug(il.ToString());
        }
        public static int DisablePlayerModeCheck(int i)
        {
            return SkillTreeBoonsConfig.Instance.enforceDifficulty ? 99 : i;
        }

        public static int GetDefaultDifficultyOption()
        {

            return SkillTreeBoonsConfig.Instance.enforceDifficulty ? 1 : 0;
        }
        private void ChangeClickDifficultyOption(ILContext il)
        {
            var c = new ILCursor(il);
            Type worldDiffucultyID = typeof(UIWorldCreation).GetNestedType("WorldDifficultyId", BindingFlags.NonPublic);
            Type GroupOptionButton = typeof(GroupOptionButton<>).MakeGenericType(worldDiffucultyID);
            FieldInfo field = typeof(UIWorldCreation).GetField("_optionDifficulty", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodBase optionBase = GroupOptionButton.GetProperty("OptionValue", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
            while (c.TryGotoNext(x => x.MatchCallvirt(optionBase)))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, field);
                var method = typeof(TModLoaderLoader).GetMethod("ModifyClickDifficulty", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { worldDiffucultyID });
                c.Emit(OpCodes.Call, method);
            }


        }
        public static T ModifyClickDifficulty<T>(T value, T value2)
        {
            var type = value.GetType();
            string prev = type.GetEnumName(value2);
            string current = type.GetEnumName(value);
            //SkillTreeBoons.Instance.Logger.Debug($"old: {prev}  new: {current}");
            if ((current == "Creative" || current == "Normal") && SkillTreeBoonsConfig.Instance.enforceDifficulty)
            {
                return (T)type.GetField(prev).GetValue(value);
            }
            return value;
        }
        private void ChangeAddWorldDifficultyOptions(ILContext il)
        {
            var c = new ILCursor(il);
            Type worldDiffucultyID = typeof(UIWorldCreation).GetNestedType("WorldDifficultyId", BindingFlags.NonPublic);
            Type GroupOptionButton = typeof(GroupOptionButton<>).MakeGenericType(worldDiffucultyID);
            if (!c.TryGotoNext(x => x.MatchCallvirt("Terraria.UI.UIElement", "Append")))
            {
                SkillTreeBoons.Instance.Logger.Debug("Could not find IL position");
            };
            c.Index--;
            c.Emit(OpCodes.Ldloc, 7);
            c.Emit(OpCodes.Call, typeof(TModLoaderLoader).GetMethod("ModifyGroupOption", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { worldDiffucultyID }));

        }
        public static void ModifyGroupOption<T>(GroupOptionButton<T> value )
        {
            
            Type worldDiffucultyID = typeof(UIWorldCreation).GetNestedType("WorldDifficultyId", BindingFlags.NonPublic);
            object normalval = worldDiffucultyID.GetField("Normal", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            object creativeval = worldDiffucultyID.GetField("Creative", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            if ((value.OptionValue.Equals(normalval) || value.OptionValue.Equals(creativeval)) && SkillTreeBoonsConfig.Instance.enforceDifficulty)
            {
                var texture = ModContent.Request<Texture2D>("SkillTreeBoons/IconDifficultyMaster", ReLogic.Content.AssetRequestMode.ImmediateLoad);
                value.GetType().GetField("_iconTexture", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(value, texture);
                value.SetColor(Color.Gray, 0.9f);

                LocalizedText text = Language.GetText("Mods.SkillTreeBoons.UI.DisabledButtonDescription");
                value.GetType().GetField("Description", BindingFlags.Public | BindingFlags.Instance).SetValue(value, text);
            }
            
        }
        public void PostSetupContent()
        {
            if (SkillTreeBoonsConfig.Instance.enforceConfig)
            {
                SkillTreeBoons.Instance.Logger.Debug("RESETTING CONFIGS TO BASE");
                ModifyConfig();
            }
        }
        
        private void ChangeWrapIt(ILContext il)
        {
            var c = new ILCursor(il);

            var label = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_2);
            c.Emit(OpCodes.Ldarg, 4);
            c.Emit(OpCodes.Call, typeof(TModLoaderLoader).GetMethod("ShouldBeHidden", BindingFlags.Public | BindingFlags.Static));
            c.Emit(OpCodes.Brfalse_S, label);
            c.Emit(OpCodes.Ldnull);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        }
        private void ChangeActivate(ILContext il)
        {
            Assembly modloaderAssembly = typeof(ModLoader).Assembly;
            Type value = modloaderAssembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig");
            var c = new ILCursor(il);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, value.GetField("modConfig", BindingFlags.NonPublic | BindingFlags.Instance));
            c.EmitDelegate<Action<Terraria.ModLoader.Config.ModConfig>>((value) =>
            {
                if (configdict.ContainsKey(value.Mod.Name))
                {
                    currentConfigEntry = configdict[value.Mod.Name][value.Name];
                    return;
                }
                currentConfigEntry = new List<ConfigEntry>();
            });
        }
        public static bool ShouldBeHidden(Terraria.ModLoader.Config.UI.PropertyFieldWrapper property, int index)
        {
            if (!SkillTreeBoonsConfig.Instance.enforceConfig) return false;
            foreach(ConfigEntry entry in currentConfigEntry)
            {
                if(entry.name == property.Name && (entry.index == index || entry.index == -1) && entry.hidden)
                {
                    return true;
                }
            }
            return false;
        }
        public struct ConfigEntry
        {
            public string name;
            public int index;
            public bool hidden;
            public object[] value;
            public ConfigEntry(string name, int index = -1, bool hidden = false, object[] value = null)
            {
                this.name = name;
                this.index = index;
                this.hidden = hidden;
                this.value = value;
            }
            
        }
    }
}
