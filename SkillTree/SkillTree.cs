using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SkillTreeBoons.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SkillTreeBoons.SkillTree
{
    public class SkillTree
    {

        public static void Load()
        {
            Node.reset();
            Group.reset();
            Circle.reset();
            //Stream s = SkillTreeBoons.Instance.GetFileStream("test.json");
            Stream s = new FileStream("F:/Users/Vivian/Documents/My games/Terraria/tModLoader/ModSources/SkillTreeBoons/test.json", FileMode.Open, FileAccess.Read);
            JsonSerializer serializer = new JsonSerializer();
            StreamReader sr = new StreamReader(s);
            JsonTextReader reader = new JsonTextReader(sr);
            try
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("Nodes"))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                Node o = serializer.Deserialize<Node>(reader);

                                o.RegisterNode();
                            }
                            if (reader.TokenType == JsonToken.EndArray)
                            {
                                break;
                            }
                        }
                    }
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("Groups"))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                Group o = serializer.Deserialize<Group>(reader);
                                o.Register();
                            }
                            if (reader.TokenType == JsonToken.EndArray)
                            {
                                break;
                            }
                        }
                    }
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("Circles"))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                Circle o = serializer.Deserialize<Circle>(reader);
                                o.Register();
                            }
                            if (reader.TokenType == JsonToken.EndArray)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SkillTreeBoons.Instance.Logger.Error(e);
            }
            s.Close();
            parseCircles();
            parseNodes();
        }
        public static void parseCircles()
        {
            Dictionary<int, Circle> cicles = Circle.GetCircles();
            foreach (int i in cicles.Keys)
            {
                parseCircle(i);
            }
        }
        public static void parseCircle(int target)
        {
            Dictionary<int, Circle> circles = Circle.GetCircles();
            Circle circle = circles[target];
            if (circle.circleid >= 0)
            {
                parseCircle(circle.circleid);
                Circle targetcircle = circles[circle.circleid];
                float value = ((float)Math.PI * 2f) / targetcircle.maxMembers;
                float value2 = circle.circleindex * value + (targetcircle.circoffset * (float)Math.PI);
                circle.circoffset = (float)(value2 / Math.PI) + circle.circoffset - 1;
                SkillTreeBoons.Instance.Logger.Info(circle.circoffset);
                float x = targetcircle.position.X + (float)Math.Cos(value2) * targetcircle.radius + circle.position.X;
                float y = targetcircle.position.Y + (float)Math.Sin(value2) * targetcircle.radius + circle.position.Y;
                circle.position.X = x;
                circle.position.Y = y;
                circle.circleid = -1;
            }
        }

        public static void parseNodes()
        {
            Dictionary<int, Node> nodes = Node.GetNodes();
            foreach (int i in nodes.Keys)
            {
                parseNode(i);
            }
        }

        public static void parseNode(int target)
        {
            Dictionary<int, Node> nodes = Node.GetNodes();
            Dictionary<int, Circle> circles = Circle.GetCircles();
            Node node = nodes[target];
            if(node.circleid >= 0)
            {
                Circle targetcircle = circles[node.circleid];
                float value = ((float)Math.PI * 2f) / targetcircle.maxMembers;
                float value2 = node.circleindex * value + (targetcircle.circoffset * (float)Math.PI);
                float x = targetcircle.position.X + (float)Math.Cos(value2) * targetcircle.radius + node.x;
                float y = targetcircle.position.Y + (float)Math.Sin(value2) * targetcircle.radius + node.y;
                node.x = x;
                node.y = y;
                node.circleid = -1;
            }
        }
    }
}
