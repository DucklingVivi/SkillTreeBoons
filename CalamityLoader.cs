using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using Terraria;
using IL.Terraria.ModLoader.Config.UI;
using Terraria.ModLoader;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace SkillTreeBoons
{
    public class CalamityLoader
    {
        private static Mod _calamity = null;
        public static Mod Calamity => _calamity;

        public void Init()
        {
            removedDifficultyList.Clear();
            ModLoader.TryGetMod("CalamityMod", out _calamity);
            removedDifficultyList.Add("NoDifficulty");
            removedDifficultyList.Add("WhereMalice");
        }
        public void Load()
        {
            
            ModifyCalamity();
        }
        public bool CalamityLoaded => Calamity != null;

        public static List<string> removedDifficultyList = new List<string>();

        private void ModifyCalamity()
        {
            if (CalamityLoaded)
            {
                Assembly calamityAssembly = Calamity.GetType().Assembly;
                
                
                Type RippersUIType = calamityAssembly.GetType("CalamityMod.UI.Rippers.RipperUI");
                if (RippersUIType != null)
                {
                    MethodInfo DrawRippers = RippersUIType.GetMethod("Draw");
                    if (DrawRippers != null) HookEndpointManager.Modify(DrawRippers, ChangeAdrenalineRageCheck);
                }


                Type CalamityPlayerType = calamityAssembly.GetType("CalamityMod.CalPlayer.CalamityPlayer");
                if (CalamityPlayerType != null)
                {

                    MethodInfo UpdateRippers = CalamityPlayerType.GetMethod("UpdateRippers", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (UpdateRippers != null) HookEndpointManager.Modify(UpdateRippers, ChangeAdrenalineRageCheck);
                    MethodInfo ProcessTriggers = CalamityPlayerType.GetMethod("ProcessTriggers");
                    if (ProcessTriggers != null) HookEndpointManager.Modify(ProcessTriggers, ChangeProcessTriggers);
                    if (ProcessTriggers != null) HookEndpointManager.Modify(ProcessTriggers, ChangeAdrenalineRageCheck);

                    MethodInfo UpdateDead = CalamityPlayerType.GetMethod("UpdateDead");
                    if (UpdateDead != null) HookEndpointManager.Modify(UpdateDead, ChangeUpdateDead);
                }
                Type HermitsBoxofOneHundredMedicines = calamityAssembly.GetType("CalamityMod.Items.Pets.HermitsBoxofOneHundredMedicines");
                if(HermitsBoxofOneHundredMedicines != null)
                {
                    MethodInfo UseStyle = HermitsBoxofOneHundredMedicines.GetMethod("UseStyle");
                    if (UseStyle != null) HookEndpointManager.Modify(UseStyle, ChangeHermitUseStyle);
                }
                Type CelestialOnionAccessorySlot = calamityAssembly.GetType("CalamityMod.Items.PermanentBoosters.CelestialOnionAccessorySlot");
                if(CelestialOnionAccessorySlot != null)
                {
                    MethodInfo IsEnabled = CelestialOnionAccessorySlot.GetMethod("IsEnabled");
                    if (IsEnabled != null) HookEndpointManager.Modify(IsEnabled, ChangeCelestialOnionIsEnabled);
                }
                Type ModeIndicatorUI = calamityAssembly.GetType("CalamityMod.UI.ModeIndicator.ModeIndicatorUI");
                if (ModeIndicatorUI != null)
                {
                    MethodInfo ManageHexIcons = ModeIndicatorUI.GetMethod("ManageHexIcons", BindingFlags.Static | BindingFlags.Public);
                    if (ManageHexIcons != null) HookEndpointManager.Modify(ManageHexIcons, ChangeManageHexIcons);
                }
                Type MiscWorldStateSystem = calamityAssembly.GetType("CalamityMod.MiscWorldStateSystem");
                if (MiscWorldStateSystem != null)
                {
                    MethodInfo LoadWorldData = MiscWorldStateSystem.GetMethod("LoadWorldData", BindingFlags.Instance | BindingFlags.Public);
                    if (LoadWorldData != null) HookEndpointManager.Modify(LoadWorldData, ChangeLoadWorldData);
                }
            }
            else
            {
                SkillTreeBoons.Instance.Logger.Debug("Calamity not loaded, proceding without calamity");
            }
        }
        
        private void ChangeLoadWorldData(ILContext il)
        {
            Assembly calamityAssembly = Calamity.GetType().Assembly;
            FieldInfo revengeField = calamityAssembly.GetType("CalamityMod.World.CalamityWorld").GetField("revenge", BindingFlags.Static | BindingFlags.Public);
            FieldInfo deathField = calamityAssembly.GetType("CalamityMod.World.CalamityWorld").GetField("death", BindingFlags.Static | BindingFlags.Public);

            var c = new ILCursor(il);
            while (c.TryGotoNext(i => i.MatchRet()))
            {
                c.Index++;
            }
            c.Index--;
            c.Emit(OpCodes.Ldsfld, revengeField);
            c.Emit(OpCodes.Ldsfld, deathField);
            c.EmitDelegate((bool revenge, bool death) =>
            {
                if (SkillTreeBoonsConfig.Instance.enforceDifficulty && !revenge && !death)
                {
                    SkillTreeBoons.Instance.Logger.Info("Difficulty was not revenge/death, Enforcing difficulty, can be disabled in config");
                    return true;
                }
                return revenge;
            });
            c.Emit(OpCodes.Stsfld, revengeField);

        }

        private void ChangeManageHexIcons(ILContext il)
        {
            Assembly calamityAssembly = Calamity.GetType().Assembly;

            MethodInfo SwitchDifficulty = calamityAssembly.GetType("CalamityMod.UI.ModeIndicator.ModeIndicatorUI").GetMethod("SwitchToDifficulty");
            FieldInfo GetDifficultyTiers = calamityAssembly.GetType("CalamityMod.Systems.DifficultyModeSystem").GetField("DifficultyTiers", BindingFlags.Static | BindingFlags.Public);
            
            MethodInfo ChangeDifficultyListMethod = typeof(CalamityLoader).GetMethod("ChangeDifficultyList", BindingFlags.Static | BindingFlags.Public);
            var c = new ILCursor(il);

            while(c.TryGotoNext(i => i.MatchLdsfld(GetDifficultyTiers))){
                c.Index++;
                c.Emit(OpCodes.Call, ChangeDifficultyListMethod);
            }

        }
        public static List<object[]> ChangeDifficultyList(List<object[]> orig)
        {
            if(!SkillTreeBoonsConfig.Instance.enforceDifficulty)
            {
                return orig;
            }
            List<object[]> objects = new List<object[]>();
            foreach (var obj in orig)
            {
                List<object> newarr = new List<object>();
                foreach (var item in obj)
                {
                    if (!removedDifficultyList.Contains(item.GetType().Name))
                    {
                        
                        newarr.Add(item);
                    }
                }
                if (newarr.Count > 0)
                {
                    objects.Add(newarr.ToArray());
                }
            }
            return objects;
        }
        private void ChangeCelestialOnionIsEnabled(ILContext il)
        {
            var c = new ILCursor(il);
            var label = il.DefineLabel();
            c.EmitDelegate<Func<bool>>(() =>
            {
                return SkillTreeBoonsConfig.Instance.demonHeartDisabled;
            });
            c.Emit(OpCodes.Brfalse, label);
            c.Emit(OpCodes.Ldc_I4_0);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        }
        private void ChangeHermitUseStyle(ILContext il)
        {
            FieldInfo info = typeof(Player).GetField("altFunctionUse", BindingFlags.Public | BindingFlags.Instance);
            var c = new ILCursor(il);
            var label = il.DefineLabel();
            c.GotoNext(i => i.MatchLdfld(info));
            c.Index++;
            c.EmitDelegate<Func<int, int>>((i) =>
            {
                return SkillTreeBoonsConfig.Instance.changeCalamity ? 9999 : i;
            });
        }
        private void ChangeUpdateDead(ILContext il)
        {
            Assembly calamityAssembly = Calamity.GetType().Assembly;
            Type CalamityPlayerType = calamityAssembly.GetType("CalamityMod.CalPlayer.CalamityPlayer");

            FieldInfo field = CalamityPlayerType.GetField("areThereAnyDamnBosses", BindingFlags.Public | BindingFlags.Static);

            var c = new ILCursor(il);
            var label = il.DefineLabel();
            c.GotoNext(i=>i.MatchLdsfld(field));
            c.Index++;
            c.EmitDelegate<Func<bool>>(() =>
            {
                return SkillTreeBoonsConfig.Instance.changeCalamity;
            });
            c.Emit(OpCodes.Brfalse, label);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        }

        private void ChangeProcessTriggers(ILContext il)
        {
            Assembly calamityAssembly = Calamity.GetType().Assembly;
            Type CalamityPlayerType = calamityAssembly.GetType("CalamityMod.CalPlayer.CalamityPlayer");
            FieldInfo field = CalamityPlayerType.GetField("rageModeActive", BindingFlags.Public | BindingFlags.Instance);


            var c = new ILCursor(il);

            c.GotoNext(i=>i.MatchLdfld(field));
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((bool val,object player) =>
            {
                object obj = player.GetType().GetProperty("Player").GetGetMethod().Invoke(player, new object[] { });
                if (obj is Player player1 && SkillTreeBoonsConfig.Instance.changeCalamity)
                {
                    return player1.GetModPlayer<SkillTreeBoonsPlayer>().boonRage ? val : true;
                }
                return val;
            });
        }
        private void ChangeAdrenalineRageCheck(ILContext il)
        {
            var c = new ILCursor(il);

            while (c.TryGotoNext(i => i.MatchCallvirt("CalamityMod.CalPlayer.CalamityPlayer", "get_RageEnabled")))
            {
                c.Index++;
                c.EmitDelegate<Func<bool, bool>>((val) =>
                {
                    if (Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().boonRage || !SkillTreeBoonsConfig.Instance.changeCalamity)
                    {
                        return val;
                    }
                    return false;
                });
            }
            c.Index = 0;
            while (c.TryGotoNext(i => i.MatchCallvirt("CalamityMod.CalPlayer.CalamityPlayer", "get_AdrenalineEnabled")))
            {
                c.Index++;
                c.EmitDelegate<Func<bool, bool>>((val) =>
                {
                    if (Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().boonAdrenaline || !SkillTreeBoonsConfig.Instance.changeCalamity)
                    {
                        return val;
                    }
                    return false;
                });
            }

        }

    }
}
