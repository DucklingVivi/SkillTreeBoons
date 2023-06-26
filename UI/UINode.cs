using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using SkillTreeBoons.SkillTree;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SkillTreeBoons.UI
{
    public class UINode : UIElement
    {

        public static Asset<Texture2D> backTex = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> frontTex = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> selectTex = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> backTexCircle = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> frontTexCircle = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> selectTexCircle = Asset<Texture2D>.Empty;
        public Asset<Texture2D> imageTex = Asset<Texture2D>.Empty;
        public int id = -1;
        public Node node;
        public float x = 0;
        public float y = 0;
        public float size = 70f;
        public BoonsGroupElement parent;
        public string name;
        public string tooltip;
        public string texture;
        public static BasicEffect effect;
        public override void OnInitialize()
        {
            if (!backTexCircle.IsLoaded) backTexCircle = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Back_Circle");
            if (!frontTexCircle.IsLoaded) frontTexCircle = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Front_Circle");
            if (!selectTexCircle.IsLoaded) selectTexCircle = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Selection_Circle");
            if (!backTex.IsLoaded) backTex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Back");
            if (!frontTex.IsLoaded) frontTex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Front");
            if (!selectTex.IsLoaded) selectTex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Selection");

            if (!imageTex.IsLoaded) imageTex = ModContent.Request<Texture2D>(texture);
            
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            if (effect == null) effect = new BasicEffect(Main.graphics.GraphicsDevice);
            CalculatedStyle style = GetDimensions();
            List<int> allocatedNodes = Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().allocatedNodes;
            List<int> availableNodes = Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().availableNodes;
            Color color = Color.White;
            Dictionary<int, Node> nodeData = Node.GetNodes();
            if (!allocatedNodes.Contains(id))
            {
                color = Color.DarkGray * 0.5f;
            }
            Texture2D backtex;
            if (node.square)
            {
                backtex = backTex.Value;
            }
            else
            {
                backtex = backTexCircle.Value;
            }
            



            spriteBatch.Draw(backtex, style.ToRectangle(), backtex.Frame(), color);
            spriteBatch.Draw(imageTex.Value, style.ToRectangle(), color);
            
            Color c = color;
            if (availableNodes.Contains(id))
            {
                c = Color.Lerp(Color.Gray, Color.Yellow * 0.5f, (float)Math.Abs(Math.Sin(Main.timeForVisualEffects / 40f)));            }
            if (base.IsMouseHovering)
            {
                c = Color.Yellow;
                string tooltipstr = "";
                foreach (string str in generateTooltip(nodeData[id]))
                {
                    tooltipstr += str + "\n";
                }
                string[] lines = tooltip.Split(new string[]
                {
                    "\r\n", "\n", "\r"
                }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str2 in lines)
                {
                    tooltipstr += "[c/808080:" + str2 + "]\n";
                }
                
                Main.instance.MouseText(name + " " + id, tooltipstr, nodeData[id].rarity);
            }
            if (node.square)
            {
                SquareDrawer.drawSquare(style.Center(), size * Main.UIScale * parent.scale, c, 0f, 15f * Main.UIScale * parent.scale);
            }
            else
            {
                CircleDrawer.drawCircle(style.Center(), size * Main.UIScale * parent.scale, c, 25, 10f * Main.UIScale * parent.scale);
            }
            


        }

        
        public static List<string> generateTooltip(Node node)
        {
            List<string> wipstr = new List<string>();
            if (node.additionalAccessories > 0)
            {
                bool flag = node.additionalAccessories > 1;
                wipstr.Add("Adds " + node.additionalAccessories + " additional accessory slot" + (flag ? "s" : ""));
            }
            if (node.adrenaline)
            {
                wipstr.Add("Enables Calamity's Adrenaline Mechanic");
            }
            if (node.rage)
            {
                wipstr.Add("Enables Calamity's Rage Mechanic");
            }
            if (node.respawnHealth > 0)
            {
                wipstr.Add("You respawn with " + node.respawnHealth + " " + (node.respawnHealth > 0 ? "more" : "less") + " health after you die");
            }
            if (node.respawnSpeed != 1)
            {
                wipstr.Add("You respawn " + (1 - node.respawnSpeed).ToString("P0") + " " + (node.respawnSpeed < 1f ? "faster" : "slower"));
            }
            if (node.toolSpeed != 1f)
            {
                wipstr.Add("You mine " + (1 - node.toolSpeed).ToString("P0") + " " + (node.toolSpeed < 1f ? "faster" : "slower"));
            }
            if (node.unlocksGroup != -1) {
                Group group = Group.getGroupByID(node.unlocksGroup);
                wipstr.Add("Unlocks the [c/" + group.color + ":" + group.name + "] tree.");
            }
            if (node.wingslot) {
                wipstr.Add("Enables the Wing Slot");
            }
            if (node.additionalMinions != 0)
            {
                wipstr.Add((node.additionalMinions > 0 ? "Increases": "Decreases")+" your max number of minions by " + node.additionalMinions);
            }
            if(node.additionalSentries != 0)
            {
                wipstr.Add((node.additionalSentries > 0 ? "Increases" : "Decreases") + " your max number of sentries by " + node.additionalSentries);
            }
            if(node.maxLife != 0)
            {
                wipstr.Add((node.maxLife > 0 ? "Increases" : "Decreases") + " your max life by " + node.maxLife);
            }
            if(node.maxMana != 0)
            {
                wipstr.Add((node.maxMana > 0 ? "Increases" : "Decreases") + " your max mana by " + node.maxMana);
            }
            if (node.maxLifePercent != 1f)
            {
                wipstr.Add((node.maxLifePercent > 1f ? "Increases" : "Decreases") + " your max life by " + node.maxLifePercent.ToString("P0"));
            }
            if (node.maxManaPercent != 1f)
            {
                wipstr.Add((node.maxManaPercent > 1f ? "Increases" : "Decreases") + " your max mana by " + node.maxManaPercent.ToString("P0"));
            }
            if (node.meleeDamage != 1f)
            {
                wipstr.Add((node.meleeDamage > 1f ? "Increases" : "Decreases") + " your melee damage by " + (node.meleeDamage - 1).ToString("P0"));
            }
            if (node.rangedDamage != 1f)
            {
                wipstr.Add((node.rangedDamage > 1f ? "Increases" : "Decreases") + " your ranged damage by " + (node.rangedDamage - 1).ToString("P0"));
            }
            if (node.magicDamage != 1f)
            {
                wipstr.Add((node.magicDamage > 1f ? "Increases" : "Decreases") + " your magic damage by " + (node.magicDamage - 1).ToString("P0"));
            }
            if (node.minionDamage != 1f)
            {
                wipstr.Add((node.minionDamage > 1f ? "Increases" : "Decreases") + " your minion damage by " + (node.minionDamage-1).ToString("P0"));
            }
            if (node.rogueDamage != 1f)
            {
                wipstr.Add((node.rogueDamage > 1f ? "Increases" : "Decreases") + " your rogue damage by " + (node.rogueDamage - 1).ToString("P0"));
            }
            if (node.damage != 1f)
            {
                wipstr.Add((node.damage > 1f ? "Increases" : "Decreases") + " all damage by " + (node.damage - 1).ToString("P0"));
            }
            return wipstr;
        }
        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);
            Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().AllocateNode(id);
        }
        public override bool ContainsPoint(Vector2 point)
        {
            if(node.shape == "circle")
            {
                float s = (size * Main.UIScale * parent.scale) / 2;
                if(point.Distance(GetOuterDimensions().Center()) <  s)
                {
                    return true;
                }
                return false;
            }
            return base.ContainsPoint(point);
        }
        public override void Recalculate()
        {

            float baseSize = 100f * Main.UIScale * parent.scale;
            float squareSize = size * Main.UIScale * parent.scale;
            
            CalculatedStyle parentStyle = Parent.GetInnerDimensions();
            
           
            float offsetx = parentStyle.Width > parentStyle.Height ? (parentStyle.Width - parentStyle.Height) / 2 : 0f;
            float offsety = parentStyle.Height > parentStyle.Width ? (parentStyle.Height - parentStyle.Width) / 2 : 0f;
            offsetx += parent.xoffset;
            offsety += parent.yoffset;

            Height.Set(squareSize, 0f);
            Width.Set(squareSize, 0f);
            Left.Set(offsetx + baseSize * x - squareSize / 2, 0f);
            Top.Set(offsety + baseSize * y - squareSize / 2, 0f);
            base.Recalculate();
        }
    }
}
