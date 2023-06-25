using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SkillTreeBoons.UI
{
    public class BoonsWindowElement : UIPanel
    {
        public static BoonsTreeElement boonsTreeElement;
        public static BoonsGroupElement boonsGroupElement;
        public static BoonsWindowElement Instance;
        private UIElement targetElement;
        public float scale = 1f;
        public float xoffset = 0f;
        public float yoffset = 0f;
        public Vector2 oldMousepos = Vector2.Zero;
        public bool dragging = false;
        public BoonsWindowElement()
        {
            
            Instance = this;
        }
        public override void OnInitialize()
        {
            BackgroundColor = Color.Black;
            OverflowHidden = true;
            boonsGroupElement = new BoonsGroupElement();
            boonsGroupElement.Width.Set(0f, 1f);
            boonsGroupElement.Height.Set(0f, 1f);
            boonsGroupElement.Left.Set(0f, 0f);
            boonsGroupElement.Top.Set(0f, 0f);

            boonsTreeElement = new BoonsTreeElement();
            boonsTreeElement.Width.Set(0f, 1f);
            boonsTreeElement.Height.Set(0f, 1f);
            boonsTreeElement.Left.Set(0f, 0f);
            boonsTreeElement.Top.Set(0f, 0f);
            Append(boonsTreeElement);
        }

        public override void Update(GameTime gameTime)
        {
            
            if (targetElement != null)
            {
                CalculatedStyle style = GetDimensions();
                RemoveAllChildren();
                Append(targetElement);
                targetElement = null;
                Activate();
                scale = 1f;
                xoffset = style.Width/4f;
                yoffset = style.Height/4f;
                
                Recalculate();
            }
            if (dragging)
            {

                float xdif = BoonsWindowUI.curMouse.X - BoonsWindowUI.oldMouse.X;
                float ydif = BoonsWindowUI.curMouse.Y - BoonsWindowUI.oldMouse.Y;
                xoffset += xdif;
                yoffset += ydif;
                boonsGroupElement.xoffset = xoffset;
                boonsGroupElement.yoffset = yoffset;
                boonsTreeElement.xoffset = xoffset;
                boonsTreeElement.yoffset = yoffset;
                Recalculate();
            }
            

            base.Update(gameTime);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            CalculatedStyle style = GetDimensions();
            float scaler = 1.2f;
            float oldscale = scale;
            if(evt.ScrollWheelValue > 0)
            {
                scale *= scaler;
            }
            else
            {
                scale /= scaler;
            }
            if(scale < 0.1f)
            {
                scale *= scaler;
            }
            if (scale > 4f)
            {
                scale /= scaler;
            }
            float scaledif = scale - oldscale;



            float zoomPointX = (evt.MousePosition.X - style.X) + xoffset - style.Width;
            float zoomPointY = (evt.MousePosition.Y - style.Y) + yoffset - style.Height;

            float newxoffset = -(zoomPointX * scaledif);
            float newyoffset = -(zoomPointY * scaledif);

           

            xoffset += newxoffset;
            yoffset += newyoffset;
            
            
            Recalculate();
        }
        public override void MouseDown(UIMouseEvent evt)
        {
            if(!(evt.Target is UINode || evt.Target is UIGroup))
            {
                dragging = true;
            }

        }
        public override void MouseUp(UIMouseEvent evt)
        {
            dragging = false;
        }
        public void showTree()
        {
            targetElement = boonsTreeElement;
        }
        public void showGroup(int id)
        {
            boonsGroupElement.selectedGroup = id;
            targetElement = boonsGroupElement;
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {

            base.DrawChildren(spriteBatch);
        }
        public override void Recalculate()
        {

            //TODO KEEP ON SCREEN LATER
            if (boonsGroupElement != null && boonsTreeElement != null)
            {
                boonsGroupElement.scale = scale;
                boonsTreeElement.scale = scale;
                boonsGroupElement.xoffset = xoffset;
                boonsGroupElement.yoffset = yoffset;
                boonsTreeElement.xoffset = xoffset;
                boonsTreeElement.yoffset = yoffset;
            }
            

            base.Recalculate();
        }
    }
}
