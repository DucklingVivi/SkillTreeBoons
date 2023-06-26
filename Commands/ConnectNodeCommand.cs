using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SkillTreeBoons.Commands
{
    public class AddTimeCommand : ModCommand
    {
        public override CommandType Type
            => CommandType.Chat;

        public override string Command
            => "connect";

        public override string Usage
            => "/connect [node id][node id]";

        public override string Description
            => "Connects two nodes and writes the result to a file";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            int id0 = int.Parse(args[0]);
            int id1 = int.Parse(args[1]);

            string json = File.ReadAllText("F:/Users/Vivian/Documents/My games/Terraria/tModLoader/ModSources/SkillTreeBoons/test.json");
            JObject jsonObj = JsonConvert.DeserializeObject(json) as JObject;
            JArray nodes = jsonObj.SelectToken("Nodes") as JArray;
            JToken node0 = null;
            JToken node1 = null;
            foreach (JToken item in nodes)
            {
                int idtoken = item.SelectToken("id").Value<int>();
                if(idtoken == id0)
                {
                    node0 = item;
                }
                if (idtoken == id1)
                {
                    node1 = item;
                }
            }
            
            if(node0 == null || node1 == null)
            {
                caller.Reply("Node not found");
                return;
            }

            JArray node0conns = node0.SelectToken("connections") as JArray;
            if(node0conns == null)
            {
                node0conns = new JArray();
                node0["connections"] = node0conns;
            }
            JArray node1conns = node1.SelectToken("connections") as JArray;
            if (node1conns == null)
            {
                node1conns = new JArray();
                node1["connections"] = node1conns;
            }
            bool flag0 = false;
            foreach (JToken item in node0conns)
            {
                if (item.Value<int>() == id1)
                {
                    flag0 = true;
                    break;
                }
            }
            bool flag1 = false;
            foreach (JToken item in node1conns)
            {
                if (item.Value<int>() == id0)
                {
                    flag1 = true;
                    break;
                }
            }
            bool flag = flag0 && flag1;
            if(!flag0)
            {
                node0conns.Add(id1);

            }
            
            if (!flag1)
            {
                node1conns.Add(id0);
            }
            if (flag) { 
                caller.Reply("Nodes already connected");
                return;
            }
            File.WriteAllText("F:/Users/Vivian/Documents/My games/Terraria/tModLoader/ModSources/SkillTreeBoons/test.json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
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
