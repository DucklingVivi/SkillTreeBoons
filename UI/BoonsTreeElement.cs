using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillTreeBoons.SkillTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace SkillTreeBoons.UI
{
    public class BoonsTreeElement : UIElement
    {

        public List<connection> cons = new List<connection>();
        public float scale = 1f;
        public float xoffset = 0f;
        public float yoffset = 0f;

        public Dictionary<int, Vector2> groupPos = new Dictionary<int, Vector2>();
        public override void OnInitialize()
        {
            SetPadding(0f);
            MinWidth.Precent = 0f;
            MinHeight.Precent = 0f;
            MaxHeight.Precent = 5f;
            MaxWidth.Precent = 5f;

            Dictionary<int, Group> groups = Group.GetGroups();
            foreach (int i in groups.Keys)
            {

                Group group = groups[i];
                foreach (int j in group.connections)
                {
                    connection test = new connection(i, j);
                    connection test2 = new connection(j, i);
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
                        connection conn = new connection(i, j);
                        conn.offset = Main.rand.NextFloat(0f, 2f);
                        cons.Add(conn);
                    }
                }
                UIGroup uIGroup = new UIGroup();
                uIGroup.parent = this;
                uIGroup.id = i;
                uIGroup.x = group.x;
                uIGroup.y = group.y;
                uIGroup.name = group.name;
                uIGroup.tooltip = group.tooltip;
                uIGroup.texture = group.texture;
                uIGroup.size = group.size;

                Append(uIGroup);
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

                if (obj is connection)
                {
                    connection con = (connection)obj;
                    if (con.connect[0] == connect[0] && con.connect[1] == connect[1])
                        return true;
                }
                return false;

            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Parent == BoonsWindowElement.Instance)
            {
                base.Draw(spriteBatch);
            }


        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            DrawConnections(spriteBatch);
            base.DrawChildren(spriteBatch);
        }

        private void DrawConnections(SpriteBatch spriteBatch)
        {
            Dictionary<int, Group> groups = Group.GetGroups();

            foreach (connection con in cons)
            {
                Vector2 start = groupPos[con.connect[0]];
                Vector2 finish = groupPos[con.connect[1]];
                float nodesize1 = groups[con.connect[0]].size;
                float nodesize2 = groups[con.connect[1]].size;
                List<int> allocatedGroups = Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().availableGroups;
                if (allocatedGroups.Contains(con.connect[0]) && allocatedGroups.Contains(con.connect[1]))
                {
                    
                    Vector2 slope = finish - start;
                    float length = slope.Length();

                    Vector2 pos1 = start + slope * ((length - nodesize2) / length);
                    Vector2 pos2 = finish - slope * ((length - nodesize1) / length);
                    //SkillTreeBoons.Instance.Logger.Debug(start + " " + finish);

                    float spread = 10f / Main.UIScale * scale;
                    ConnectorLightning.Draw(spriteBatch, pos1, pos2, Color.White, spread, con.offset);
                }
                else if(allocatedGroups.Contains(con.connect[0]) || allocatedGroups.Contains(con.connect[1]))
                {
                    Utils.DrawLine(spriteBatch, start, finish, Color.Gray);
                }
                else
                {
                    Utils.DrawLine(spriteBatch, start, finish, Color.Gray);
                }
            }
        }

        public override void Recalculate()
        {

            base.Recalculate();
            groupPos.Clear();
            foreach (UIElement child in Children)
            {
                if (child is UIGroup)
                {
                    UIGroup groupchild = child as UIGroup;
                    groupPos.Add(groupchild.id, child.GetDimensions().Center());
                }
            }
        }

    }
}
