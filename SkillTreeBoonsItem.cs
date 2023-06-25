using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SkillTreeBoons
{
    public class SkillTreeBoonsItem : GlobalItem
    {

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.DemonHeart && SkillTreeBoonsConfig.Instance.demonHeartDisabled)
            {
                tooltips.RemoveAll(i =>
                {
                    return i.Name == "Tooltip0" || i.Name == "Consumable";
                });
                if (Main.keyState.PressingShift())
                {
                    TooltipLine line = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", SkillTreeBoons.Instance.DisplayName + ": DISABLED EXTRA ACCESSORY");
                    line.OverrideColor = Color.Red;
                    tooltips.Add(line);
                    TooltipLine line2 = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", "Can be enabled in config");
                    line2.OverrideColor = Color.Red;
                    tooltips.Add(line2);
                }
                else
                {
                    TooltipLine line = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", SkillTreeBoons.Instance.DisplayName + ": Hold shift for info");
                    line.OverrideColor = Color.Gray;
                    tooltips.Add(line);
                }
                
                
            }
            if(item.ModItem != null)
            {
                if (item.ModItem.Name == "HermitsBoxofOneHundredMedicines" && SkillTreeBoonsConfig.Instance.changeCalamity)
                {
                    tooltips.RemoveAll(i =>
                    {
                        return i.Name == "Tooltip4" || i.Name == "Tooltip5";
                    });
                    if (Main.keyState.PressingShift())
                    {
                        TooltipLine line = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", SkillTreeBoons.Instance.DisplayName + ": DISABLED FULL HEALTH MECHANIC");
                        line.OverrideColor = Color.Red;
                        tooltips.Add(line);
                        TooltipLine line2 = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", "Can be enabled in config");
                        line2.OverrideColor = Color.Red;
                        tooltips.Add(line2);
                    }
                    else
                    {
                        TooltipLine line = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", SkillTreeBoons.Instance.DisplayName + ": Hold shift for info");
                        line.OverrideColor = Color.Gray;
                        tooltips.Add(line);
                    }
                   
                }
                if (item.ModItem.Name == "CelestialOnion" && SkillTreeBoonsConfig.Instance.demonHeartDisabled)
                {
                    tooltips.RemoveAll(i =>
                    {
                        return i.Name == "Tooltip0" || i.Name == "Tooltip1" || i.Name == "Tooltip2" || i.Name == "Consumable";
                    });
                    if (Main.keyState.PressingShift())
                    {
                        TooltipLine line = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", SkillTreeBoons.Instance.DisplayName + ": DISABLED EXTRA ACCESSORY");
                        line.OverrideColor = Color.Red;
                        tooltips.Add(line);
                        TooltipLine line2 = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", "Can be enabled in config");
                        line2.OverrideColor = Color.Red;
                        tooltips.Add(line2);
                    }
                    else
                    {
                        TooltipLine line = new TooltipLine(SkillTreeBoons.Instance, "SkillTreeBoonsInfo", SkillTreeBoons.Instance.DisplayName + ": Hold shift for info");
                        line.OverrideColor = Color.Gray;
                        tooltips.Add(line);
                    }
                        
                }
            }
            

        }
        public override bool ConsumeItem(Item item, Player player)
        {

            if (item.type == ItemID.DemonHeart)
            {
                if (SkillTreeBoonsConfig.Instance.demonHeartDisabled)
                {
                    return false;
                }
            }
            return base.ConsumeItem(item, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.DemonHeart)
            {
                if (SkillTreeBoonsConfig.Instance.demonHeartDisabled)
                {
                    Main.NewText("DEMON HEART IS DISABLED FOR BALANCE", Color.Red);
                    Main.NewText("Can be enabled in config", Color.Red);
                    return false;
                }
            }
            if (item.ModItem != null)
            {
                if(item.ModItem.Name == "CelestialOnion")
                {
                    if (SkillTreeBoonsConfig.Instance.demonHeartDisabled)
                    {
                        Main.NewText("CELESTIAL ONION IS DISABLED FOR BALANCE", Color.Red);
                        Main.NewText("Can be enabled in config", Color.Red);
                        return false;
                    }
                }
            }
            return base.CanUseItem(item, player);
        }
    }
}
