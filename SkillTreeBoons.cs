using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using SkillTreeBoons.UI;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;



namespace SkillTreeBoons
{
	public class SkillTreeBoons : Mod
	{
        public static SkillTreeBoons Instance;
        public static ModKeybind boonsKeybind { get; private set; }
        public static ModKeybind boonsResetKeybind { get; private set; }
        public static ModKeybind boonsResetTreeKeybind { get; private set; }

        public static CalamityLoader Calamity = new CalamityLoader();
        public static TModLoaderLoader TModLoader = new TModLoaderLoader();
        public static WingSlotLoader WingSlot = new WingSlotLoader();
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte msgType = reader.ReadByte();
            if(msgType == (byte)PacketMessageType.SyncTree)
            {
                byte player = reader.ReadByte();
                SkillTreeBoonsPlayer modPlayer = Main.player[player].GetModPlayer<SkillTreeBoonsPlayer>();
                int count = reader.ReadInt32();
                List<int> list = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(reader.ReadInt32());
                }
                modPlayer.allocatedNodes = new List<int>(list);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = GetPacket();
                    packet.Write((byte)PacketMessageType.SyncTree);
                    packet.Write(player);
                    packet.Write(count);
                    for (int i = 0; i < count; i++)
                    {
                        packet.Write(list[i]);
                    }
                    packet.Send(-1, whoAmI);
                }
            }

        }

        public override void Load()
        {
            
            Instance = this;
            TModLoader.Init();
            if (ModLoader.HasMod("CalamityMod")) Calamity.Init();
            if (ModLoader.HasMod("WingSlotExtra")) WingSlot.Init();
         




            TModLoader.Load();
            if (ModLoader.HasMod("CalamityMod")) Calamity.Load();
            if (ModLoader.HasMod("WingSlotExtra")) WingSlot.Load();

            boonsKeybind = KeybindLoader.RegisterKeybind(this, "Open Boons Window", Microsoft.Xna.Framework.Input.Keys.P);
            boonsResetKeybind = KeybindLoader.RegisterKeybind(this, "ResetBoons", Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets);
            boonsResetTreeKeybind = KeybindLoader.RegisterKeybind(this, "ResetTreeDebug", Microsoft.Xna.Framework.Input.Keys.L);
            SkillTree.SkillTree.Load();

        }
        public override void PostSetupContent()
        {
            TModLoader.PostSetupContent();
        }
        public override void Unload()
        {


            Instance = null; 

        }
        public class SkillTreeBoonsSystem : ModSystem
        {
            public static BoonsWindowUI boonsWindowUI;
            public override void Load()
            {
                
                if (!Main.dedServ)
                {
                    boonsWindowUI = new BoonsWindowUI();
                    boonsWindowUI.Activate();
                }
            }
            public override void OnWorldLoad()
            {
                if(boonsWindowUI != null)
                {
                    boonsWindowUI.Initialize();
                   
                }
                
                base.OnWorldLoad();
            }
            public override void Unload()
            {
                boonsWindowUI = null;
            }
            public override void UpdateUI(GameTime gameTime)
            {
                if (boonsKeybind.JustPressed)
                {
                    if (Main.InGameUI.CurrentState == boonsWindowUI)
                    {
                        IngameFancyUI.Close();
                    }
                    else
                    {
                        IngameFancyUI.OpenUIState(boonsWindowUI);
                        boonsWindowUI.ShowTree();
                    }
                    
                }
                if (boonsResetKeybind.JustPressed)
                {
                    Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().allocatedNodes.Clear();
                    Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().ParseSkillTree();
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().SyncTree();
                    }
                    /*FileStream s = new FileStream("F:/Users/Vivian/Documents/My games/Terraria/tModLoader/ModSources/SkillTreeBoons/skill.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(s);
                  
                    sw.Write("");
                    JsonTextWriter writer = new JsonTextWriter(sw);
                   
                    writer.WriteStartObject();
                    writer.WritePropertyName("Nodes");
                    writer.WriteStartArray();
                    int maxnodes = 400;
                    int nodesperrow = 20;
                    int nodespercollumn = maxnodes/nodesperrow;

                    for (int i = 0; i < nodespercollumn; i++)
                    {
                        for (int j = 0; j < nodesperrow; j++)
                        {
                            int id = nodespercollumn * i + j;
                            writer.WriteStartObject();
                            writer.WritePropertyName("x");
                            writer.WriteValue(i);
                            writer.WritePropertyName("y");
                            writer.WriteValue(j);
                            writer.WritePropertyName("id");
                            writer.WriteValue(id);
                            if (i + j == 0) { 
                                writer.WritePropertyName("startingNode");
                                writer.WriteValue(true);
                            }
                            writer.WritePropertyName("connections");
                            writer.WriteStartArray();
                            int targetid = id + 1;
                            if(targetid >= maxnodes)
                            {
                                targetid = 0;
                            }
                            int targetid2 = id - 1;
                            if(targetid2 < 0)
                            {
                                targetid2 = maxnodes - 1;
                            }
                            writer.WriteValue(targetid2);
                            writer.WriteValue(targetid);
                            writer.WriteEndArray();
                            writer.WriteEndObject();

                        }
                    }
                    writer.WriteEndArray();
                    writer.WritePropertyName("Groups");
                    writer.WriteStartArray();
                    writer.WriteStartObject();
                    writer.WritePropertyName("id");
                    writer.WriteValue(0);
                    writer.WritePropertyName("name");
                    writer.WriteValue("Group 1");
                    writer.WritePropertyName("startingGroup");
                    writer.WriteValue(true);
                    writer.WritePropertyName("nodes");
                   
                    writer.WriteStartArray();
                    for (int i = 0; i < maxnodes; i++)
                    {
                        writer.WriteValue(i);
                        Instance.Logger.Info(i);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    writer.Close();
                    s.Close();*/

                }
                if (boonsResetTreeKeybind.JustPressed)
                {
                    Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().allocatedNodes.Clear();
                    Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().ParseSkillTree();
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().SyncTree();
                    }
                    SkillTree.SkillTree.Load();

                }
                
            }
           
        }
        

    }
    
}