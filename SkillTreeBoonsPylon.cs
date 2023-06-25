using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SkillTreeBoons
{
    public class SkillTreeBoonsPylon : GlobalPylon
    {


        public override bool? ValidTeleportCheck_PreNPCCount(TeleportPylonInfo pylonInfo, ref int defaultNecessaryNPCCount)
        {

            return base.ValidTeleportCheck_PreNPCCount(pylonInfo, ref defaultNecessaryNPCCount);
        }

    }
}
