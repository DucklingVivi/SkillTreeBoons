using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SkillTreeBoons.SkillTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SkillTreeBoons.UI
{
    public class UIGroup : UIElement
    {

        public static Asset<Texture2D> backTex = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> frontTex = Asset<Texture2D>.Empty;
        public static Asset<Texture2D> selectTex = Asset<Texture2D>.Empty;
        public Asset<Texture2D> imageTex = Asset<Texture2D>.Empty;
        public BoonsTreeElement parent;
        public int id = -1;
        public float x = 0;
        public float y = 0;
        public float size = 70f;
        public string name;
        public string tooltip;
        public string texture;

        public override void OnInitialize()
        {
            if (!backTex.IsLoaded) backTex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Back");
            if (!frontTex.IsLoaded) frontTex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Front");
            if (!selectTex.IsLoaded) selectTex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/Slot_Selection");
            if (!imageTex.IsLoaded) imageTex = ModContent.Request<Texture2D>(texture);
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            List<int> allocatedGroups = Main.player[Main.myPlayer].GetModPlayer<SkillTreeBoonsPlayer>().availableGroups;
            Group group = Group.getGroupByID(id);
            CalculatedStyle style = GetDimensions();

            Color color = Color.White;
            if (!allocatedGroups.Contains(id))
            {
                color = Color.Gray;
            }
            spriteBatch.Draw(backTex.Value, style.ToRectangle(), color);
            spriteBatch.Draw(imageTex.Value, style.ToRectangle(), color);
            spriteBatch.Draw(frontTex.Value, style.ToRectangle(), color);
            if (base.IsMouseHovering)
            {
                spriteBatch.Draw(selectTex.Value, style.ToRectangle(), color);
                Main.instance.MouseText("[c/" + group.color +":"+  name + "]\n" + tooltip, 0, 0);
            }

        }

        public override void Click(UIMouseEvent evt)
        {
            
            BoonsWindowUI.boonsWindowElement.showGroup(id);
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
