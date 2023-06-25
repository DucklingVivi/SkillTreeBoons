
using SkillTreeBoons.SkillTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using rail;
using System.Reflection;

namespace SkillTreeBoons
{
    public class SkillTreeBoonsPlayer : ModPlayer
    {
        public List<int> availableGroups = new List<int>();
        public List<int> availableNodes = new List<int>();
        public List<int> allocatedNodes = new List<int>();


        public bool boonAdrenaline = false;
        public bool boonRage = false;
        public bool boonWingSlot = false;
        public float boonToolTime = 1f;
        public int boonHealthRespawn = 0;
        public bool demonheartAccessory = false;
        public bool masterModeAccessory = false;
        public int boonAdditionalAccessories = 0;
        //IN SECONDS
        public int respawnBase = 900;
        /**IN PERCENT**/
        public float boonRespawnSpeed = 1f;

        public int boonAdditionalMinions = 0;
        public int boonAdditionalSentries = 0;
        public int boonMaxLife = 0;
        public int boonMaxMana = 0;
        public float boonMaxLifePercent = 1f;
        public float boonMaxManaPercent = 1f;
        public float boonLifeRegen = 0f;
        public float boonManaRegen = 0f;
        public bool boonUncapMana = false;
        public float boonMeleeDamage = 1f;
        public float boonRangedDamage = 1f;
        public float boonMagicDamage = 1f;
        public float boonMinionDamage = 1f;
        public float boonRogueDamage = 1f;
        public float boonDamage = 1f;
        public static DamageClass rogueDamageClass;










        private bool needsRespawnHeal = false;
        
        public override void Load()
        {
            
            base.SetStaticDefaults();
        }
        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Assembly assembly = CalamityLoader.Calamity.GetType().Assembly;
                Type rogueDamageClassType = assembly.GetType("CalamityMod.RogueDamageClass");
                FieldInfo info = rogueDamageClassType.GetField("Instance",BindingFlags.NonPublic | BindingFlags.Static);
                DamageClass damageClass = (DamageClass)info.GetValue(null);
                rogueDamageClass = damageClass;
             
            }
           
            base.SetStaticDefaults();
        }
        public override void clientClone(ModPlayer clientClone)
        {
            SkillTreeBoonsPlayer clone = clientClone as SkillTreeBoonsPlayer;

            if (clone == null)
            {
                return;
            }

            clone.availableGroups = new List<int>(availableGroups);
            clone.allocatedNodes = new List<int>(allocatedNodes);


        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            SkillTreeBoonsPlayer clone = clientPlayer as SkillTreeBoonsPlayer;
            if (clone == null)
            {
                return;
            }
            if (clone.allocatedNodes != allocatedNodes)
            {
                var packet = Mod.GetPacket();
                packet.Write((byte)PacketMessageType.SyncTree);
                packet.Write((byte)Player.whoAmI);
                packet.Write(allocatedNodes.Count);
                for (int i = 0; i < allocatedNodes.Count; i++)
                {
                    packet.Write(allocatedNodes[i]);
                }
                packet.Send();
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (allocatedNodes.Count > 0)
            {
                tag.Add("allocatedNodes", allocatedNodes);
            }

        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("allocatedNodes"))
            {
                allocatedNodes = new List<int>(tag.GetList<int>("allocatedNodes"));
            }

            ParseSkillTree();
        }
        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
            ParseSkillTree();
        }
        public void AllocateNode(int i)
        {

            Dictionary<int, Node> NodeData = Node.GetNodes();
            if (allocatedNodes.Contains(i)) return;
            if (!availableNodes.Contains(i) && !Node.startingNodes.Contains(i)) return;
            allocatedNodes.Add(i);
            ParseSkillTree();

        }

        public bool TryRemoveNode(int i)
        {

            Dictionary<int, Node> NodeData = Node.GetNodes();
            //NodeData[i].connections.contain
            allocatedNodes.Remove(i);
            ParseSkillTree();
            return true;
        }

        public void ParseSkillTree()
        {
            availableGroups.Clear();
            availableNodes.Clear();
            boonToolTime = 1f;
            boonRespawnSpeed = 1f;
            boonHealthRespawn = 0;
            boonAdditionalAccessories = 0;
            boonAdrenaline = false;
            boonRage = false;
            boonWingSlot = false;


            boonAdditionalMinions = 0;
            boonAdditionalSentries = 0;
            boonMaxLife = 0;
            boonMaxMana = 0;
            boonMaxLifePercent = 1f;
            boonMaxManaPercent = 1f;
            boonLifeRegen = 0f;
            boonManaRegen = 0f;
            boonUncapMana = false;
            boonDamage = 1f;
            boonMeleeDamage = 1f;
            boonRangedDamage = 1f;
            boonMagicDamage = 1f;
            boonMinionDamage = 1f;
            boonRogueDamage = 1f;


            Dictionary<int, Node> NodeData = Node.GetNodes();
            Dictionary<int, Group> GroupData = Group.GetGroups();
            

            foreach (int i in Group.startingGroups)
            {
                if (!availableGroups.Contains(i)) availableGroups.Add(i);
            }
         


            foreach (int id in allocatedNodes)
            {
                boonAdrenaline |= NodeData[id].adrenaline;
                boonRage |= NodeData[id].rage;
                boonWingSlot |= NodeData[id].wingslot;

                boonAdditionalAccessories += NodeData[id].additionalAccessories;
                boonToolTime *= NodeData[id].toolSpeed;
                boonRespawnSpeed *= NodeData[id].respawnSpeed;
                boonHealthRespawn += NodeData[id].respawnHealth;


                boonAdditionalMinions += NodeData[id].additionalMinions;
                boonAdditionalSentries += NodeData[id].additionalSentries;
                boonMaxLife += NodeData[id].maxLife;
                boonMaxMana += NodeData[id].maxMana;
                boonMaxLifePercent *= NodeData[id].maxLifePercent;
                boonMaxManaPercent *= NodeData[id].maxManaPercent;
                boonLifeRegen += NodeData[id].lifeRegen;
                boonManaRegen += NodeData[id].manaRegen;
                boonUncapMana |= NodeData[id].uncapMana;
                boonMeleeDamage *= NodeData[id].meleeDamage;
                boonRangedDamage *= NodeData[id].rangedDamage;
                boonMagicDamage *= NodeData[id].magicDamage;
                boonMinionDamage *= NodeData[id].minionDamage;
                boonRogueDamage *= NodeData[id].rogueDamage;
                boonDamage *= NodeData[id].damage;


                if (NodeData[id].unlocksGroup != -1)
                {
                    if (!availableGroups.Contains(NodeData[id].unlocksGroup)) availableGroups.Add(NodeData[id].unlocksGroup);
                }
                foreach (int id2 in NodeData[id].connections)
                {
                    if (!allocatedNodes.Contains(id2) && id != id2)
                    {
                        if (!availableNodes.Contains(id2)) availableNodes.Add(id2);
                    }
                }
            }
            foreach (int i in availableGroups)
            {
                foreach (int j in GroupData[i].nodes)
                {
                    if (NodeData[j].startingNode && !allocatedNodes.Contains(j))
                    {
                        if (!availableNodes.Contains(j)) availableNodes.Add(j);
                    }
                }
            }
            if (boonAdditionalAccessories > 0)
            {
                demonheartAccessory = true;
                if (boonAdditionalAccessories > 1)
                {
                    masterModeAccessory = true;
                }
                else
                {
                    masterModeAccessory = false;
                }
            }
            else
            {
                masterModeAccessory = false;
                demonheartAccessory = false;
            }
            SkillTreeBoons.Instance.Logger.Debug(availableNodes);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncTree();
            }
        }

        public override void PostUpdateBuffs()
        {
            Player.maxMinions += boonAdditionalMinions;
        }
        public override void ResetEffects()
        {
            
            Player.statLifeMax2 += boonMaxLife;
            Player.statLifeMax2 = (int)(Player.statLifeMax2 * boonMaxLifePercent);
            Player.statManaMax += boonMaxMana;
            Player.statManaMax = (int)(Player.statManaMax * boonMaxManaPercent);
            Player.manaRegenBonus += (int)boonManaRegen;
            Player.pickSpeed = Player.pickSpeed * boonToolTime;

            
            
            Player.GetDamage<MeleeDamageClass>() += boonMeleeDamage - 1;
            Player.GetDamage<RangedDamageClass>() += boonRangedDamage - 1;
            Player.GetDamage<MagicDamageClass>() += boonMagicDamage - 1;
            Player.GetDamage<SummonDamageClass>() += boonMinionDamage - 1;
            if (ModLoader.HasMod("CalamityMod"))
            {
                Player.GetDamage(rogueDamageClass) += boonRogueDamage - 1;     
            }
            Player.GetDamage<GenericDamageClass>() += boonDamage - 1;


        }
        public override void UpdateLifeRegen()
        {
            Player.lifeRegen += (int)boonLifeRegen;
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {

        }
        
        

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
        }
        public override void ModifyWeaponKnockback(Item item, ref StatModifier knockback)
        {
        }

        public void SyncTree(int toClient = -1, int ignoreClient = -1)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)PacketMessageType.SyncTree);
            packet.Write((byte)Player.whoAmI);
            packet.Write(allocatedNodes.Count);
            for (int i = 0; i < allocatedNodes.Count; i++)
            {
                packet.Write(allocatedNodes[i]);
            }
            packet.Send(toClient, ignoreClient);
        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            SyncTree(toWho, fromWho);
        }


        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            Player.respawnTimer = (int)(respawnBase * boonRespawnSpeed);
            bool bossAlive = false;
            if (Main.netMode != NetmodeID.SinglePlayer && !pvp)
            {
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == 13 || Main.npc[k].type == 14 || Main.npc[k].type == 15) && Math.Abs(Player.Center.X - Main.npc[k].Center.X) + Math.Abs(Player.Center.Y - Main.npc[k].Center.Y) < 4000f)
                    {
                        bossAlive = true;
                        break;
                    }
                }
            }
            Player.respawnTimer = Player.respawnTimer * (bossAlive ? 2 : 1);
        }
        public override void OnRespawn(Player player)
        {
            this.needsRespawnHeal = true;
        }
        public override void PreUpdateBuffs()
        {
            if (this.needsRespawnHeal)
            {
                Player.statLife += boonHealthRespawn;
                if (Player.statLife >= Player.statLifeMax2)
                {
                    Player.statLife = Player.statLifeMax2;
                }
                this.needsRespawnHeal = false;
            }
        }
    }
}
