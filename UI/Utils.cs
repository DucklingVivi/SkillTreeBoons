using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillTreeBoons.UI
{
    public static class Utils
    {
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 pos1, Vector2 pos2, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)pos1.X, (int)pos1.Y, (int)(pos2 - pos1).Length() + width, width);
            Vector2 v = Vector2.Normalize(pos1 - pos2);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (pos1.Y > pos2.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
