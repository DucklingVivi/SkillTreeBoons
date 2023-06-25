
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SkillTreeBoons.SkillTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace SkillTreeBoons.UI
{
    public class BoonsWindowUI : UIState
    {




        public static KeyboardState curKeyboard;
        public static KeyboardState oldKeyboard;
        public static List<Keys> justPressedKeys;

        public static MouseState curMouse;
        public static MouseState oldMouse;
        public static BoonsWindowElement boonsWindowElement;


        public override void OnInitialize()
        {
            OverflowHidden = true;
            boonsWindowElement = new BoonsWindowElement();
            boonsWindowElement.Top.Set(0f, .05f);
            boonsWindowElement.Left.Set(0f, .05f);
            boonsWindowElement.Width.Set(0f, .9f);
            boonsWindowElement.Height.Set(0f, .9f);
            Append(boonsWindowElement);

        }

        public override void Update(GameTime gameTime)
        {
            Main.blockKey = Keys.None.ToString();

            oldMouse = curMouse;
            curMouse = Mouse.GetState();
            //log mouse scroll

            base.Update(gameTime);
        }
        public void ShowTree()
        {
            boonsWindowElement.showTree();
        }

    }

}

