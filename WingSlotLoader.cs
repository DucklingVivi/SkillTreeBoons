
using Terraria;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using CalamityMod.Items.Pets;

namespace SkillTreeBoons
{
    public class WingSlotLoader
    {

        private static Mod _wingSlot = null;
        public static Mod WingSlot => _wingSlot;
        public void Init()
        {
            ModLoader.TryGetMod("WingSlotExtra", out _wingSlot);
        }
        public void Load()
        {
            ModifyWingSlot();
        }
        public bool WingSlotLoaded => WingSlot != null;
        private void ModifyWingSlot()
        {
            if (WingSlotLoaded)
            {
                Assembly wingSlotAssembly = WingSlot.GetType().Assembly;

                Type WingSlotExtraSlotType = wingSlotAssembly.GetType("WingSlotExtra.WingSlotExtraSlot");
                if(WingSlotExtraSlotType != null)
                {
                    MethodInfo Visible = WingSlotExtraSlotType.GetMethod("IsVisibleWhenNotEnabled");
                    if (Visible != null) HookEndpointManager.Modify(Visible, ChangeVisible);
                    MethodInfo Enabled = WingSlotExtraSlotType.GetMethod("IsEnabled");
                    if (Enabled != null) HookEndpointManager.Modify(Enabled, ChangeEnabled);
                }

            }
        }
        private void ChangeEnabled(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var label = il.DefineLabel();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<ModAccessorySlot, bool>>((val) => {
                if (!Main.player[Main.myPlayer].TryGetModPlayer<SkillTreeBoonsPlayer>(out SkillTreeBoonsPlayer player)) return true;
                //SkillTreeBoons.Instance.Logger.Debug(val);
                if(val.Name == "WingSlotExtra") return player.boonWingSlot;
                return true;
            });
            c.Emit(OpCodes.Brtrue_S, label);
            c.Emit(OpCodes.Ldc_I4_0);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
            
        }
        private void ChangeVisible(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<ModAccessorySlot, bool>>((val) =>{
                if(!val.FunctionalItem.IsAir || !val.VanityItem.IsAir || !val.DyeItem.IsAir || ModAccessorySlot.Player.GetModPlayer<SkillTreeBoonsPlayer>().boonWingSlot)
                {
                    return true;
                }
                return false;
            });
            c.Emit(OpCodes.Ret);
        }
    }
}
