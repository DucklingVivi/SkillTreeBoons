using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using tModPorter;

namespace SkillTreeBoons
{
    [Label("Skill Tree Boons Config")]
    public class SkillTreeBoonsConfig : ModConfig
    {

        [Label("Enforce Config")]
        [Tooltip("Enforce config overwriting on other mods\nDISABLING WILL REMOVE BALANCE")]
        [DefaultValue(true)]
        public bool enforceConfig;

        [Label("Enforce Difficulty")]
        [Tooltip("Enforce difficulty lockout\nDISABLING WILL REMOVE BALANCE")]
        [DefaultValue(true)]
        public bool enforceDifficulty;

        [Label("Change Calamity")]
        [Tooltip("Changes calamity to be more balances\nDISABLING WILL REMOVE BALANCE")]
        [DefaultValue(true)]
        public bool changeCalamity;

        [Label("Disable Accessories")]
        [Tooltip("Disables extra accessories from items and mastermode\nDISABLING WILL REMOVE BALANCE")]
        [DefaultValue(true)]
        public bool demonHeartDisabled;
        public override ConfigScope Mode => ConfigScope.ServerSide;
        private static SkillTreeBoonsConfig _instance;
        public static SkillTreeBoonsConfig Instance => _instance ??= ModContent.GetInstance<SkillTreeBoonsConfig>();
        public override void OnLoaded()
        {
            _instance = ModContent.GetInstance<SkillTreeBoonsConfig>();
        }
    }
}
