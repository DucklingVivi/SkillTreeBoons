using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkillTreeBoons.SkillTree;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace SkillTreeBoons.UI
{
    public class BoonsGroupElement : UIElement
    {
        public List<connection> cons = new List<connection>();
        public Dictionary<int, Vector2> nodePos = new Dictionary<int, Vector2>();
        public float scale = 1f;
        public float xoffset = 0f;
        public float yoffset = 0f;

        public int selectedGroup { set { LoadNodes(value); } }
        public override void OnInitialize()
        {
            SetPadding(0f);
        }

        public void LoadNodes(int id)
        {
            RemoveAllChildren();
            cons.Clear();
            Group targetGroup = new Group();
            if (id >= 0)
                targetGroup = Group.getGroupByID(id);

            Dictionary<int, Node> nodeDict = Node.GetNodes();
            foreach (int nodeID in targetGroup.nodes)
            {
                Node node = nodeDict[nodeID];
                foreach (int j in node.connections)
                {
                    connection test = new connection(nodeID, j );
                    connection test2 = new connection(j, nodeID);
                    bool flag = true;
                    foreach (connection con in cons)
                    {
              
                        if (con.Equals(test) || con.Equals(test2))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    { 
                        connection connect = new connection(nodeID, j);
                        connect.offset = Main.rand.NextFloat(0,2f);
                        cons.Add(connect);
                    }
                }
                UINode uINode = new UINode();
                uINode.parent = this;
                uINode.id = nodeID;
                uINode.x = node.x;
                uINode.y = node.y;
                uINode.name = node.name;
                uINode.tooltip = node.tooltip;
                uINode.texture = node.texture;
                uINode.size = node.size;
                uINode.node = node;
                Append(uINode);

            }

            Recalculate();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (PlayerInput.GetPressedKeys().Contains((Keys)Enum.Parse(typeof(Keys), Main.cInv, true)))
            {
                Main.blockKey = ((Keys)Enum.Parse(typeof(Keys), Main.cInv, true)).ToString();
                BoonsWindowUI.boonsWindowElement.showTree();
            }

        }

        public struct connection
        {
            public int[] connect = { -1, -1 };
            public float offset;
            public connection(int i, int j)
            {
                connect[0] = i;
                connect[1] = j;
                offset = 0;
            }
            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                if(obj is connection)
                {
                    connection con = (connection)obj;
                    if (con.connect[0] == connect[0] && con.connect[1] == connect[1])
                        return true;
                }
                return false;
              
            }
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            DrawConnections(spriteBatch);
            base.DrawChildren(spriteBatch);
        }


        private void DrawConnections(SpriteBatch spriteBatch)
        {
            Dictionary<int, Node> nodes = Node.GetNodes();
            List<int> allocatedNodes = Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().allocatedNodes;
            foreach (connection con in cons)
            {
                Vector2 currentpos = new Vector2(xoffset, yoffset);
                float nodesize1 = nodes[con.connect[0]].size * Main.UIScale * scale / 2;
                float nodesize2 = nodes[con.connect[1]].size * Main.UIScale * scale / 2;
                Vector2 start = nodePos[con.connect[0]];
                Vector2 finish = nodePos[con.connect[1]];
                Vector2 slope = finish - start;
                float length = slope.Length();

                Vector2 pos1 = start + slope * ((length - nodesize2) / length);
                Vector2 pos2 = finish - slope * ((length - nodesize1) / length);
                //SkillTreeBoons.Instance.Logger.Debug(start + " " + finish);

                float spread = 10f * Main.UIScale * scale;
                if (allocatedNodes.Contains(con.connect[0]) && allocatedNodes.Contains(con.connect[1]))
                {
                    ConnectorLightning.Draw(spriteBatch, pos1, pos2, Color.White, 0f, 0f, spread); 
                }
                else if(allocatedNodes.Contains(con.connect[0]) || allocatedNodes.Contains(con.connect[1]))
                {
                   
                    Color c =  Color.Lerp(Color.Gray, Color.Yellow * 0.5f, (float)Math.Abs(Math.Sin(Main.timeForVisualEffects / 40f)));
                    float scalar = 1.5f * length / 150f;
                    ConnectorLightning.Draw(spriteBatch, pos1, pos2, c, spread* scalar, con.offset, spread);
                }
                else
                {
                    ConnectorLightning.Draw(spriteBatch, pos1, pos2, Color.DarkGray * 0.5f, 0f, 0f, spread);
                }
            }
            
        }

        public override void Recalculate()
        {

            base.Recalculate();
            nodePos.Clear();
            foreach (UIElement child in Children)
            {
                if (child is UINode)
                {
                    UINode nodechild = child as UINode;
                    nodePos.Add(nodechild.id, child.GetDimensions().Center());
                }
            }
        }
        
        
    }
}
