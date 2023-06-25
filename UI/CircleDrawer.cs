using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SkillTreeBoons.UI
{
    public class CircleDrawer
    {

        public static BasicEffect effect;

        public static void drawCircle(Vector2 position, float size, Microsoft.Xna.Framework.Color color, int points = 40, float linethickness = 10f)
        {
            if (effect == null) effect = new BasicEffect(Main.graphics.GraphicsDevice);

            Vector2 offset = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            position -= offset;
            position.Y *= -1;

            
            object[] curveData = calcCircleVerticies(position, size, points, linethickness);
            var camPos = new Vector3(0, 0, 0.1f);
            var camLookAtVector = Vector3.Forward;
            var camUpVector = Vector3.Up;
            effect.View = Matrix.CreateLookAt(camPos, camLookAtVector, camUpVector);

            float nearClipPlane = 0.1f;
            float farClipPlane = 2f;
            //SkillTreeBoons.Instance.Logger.Debug("test");
            effect.EmissiveColor = Vector3.Zero;
            effect.DiffuseColor = color.ToVector3();
            effect.Alpha = 1f;
            effect.Projection = Matrix.CreatePerspective(Main.screenWidth, Main.screenHeight, nearClipPlane, farClipPlane);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            effect.TextureEnabled = true;
            Texture2D tex = ModContent.Request<Texture2D>("SkillTreeBoons/UI/test", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            effect.Texture = tex;
            effect.Alpha = 1f;
            effect.AmbientLightColor = color.ToVector3();
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                effect.World = Matrix.CreateScale(1f);

                //Color[] test = new Color[tex.Width * tex.Height];
                //tex.GetData<Color>(test, 0, tex.Width * tex.Height);
                /* for (int i = 0; i < test.Length; i++)
                 {
                     //SkillTreeBoons.Instance.Logger.Debug(test[i].A);
                     test[i].A = (byte)(255 * (test.Length - i) / (float)test.Length);
                 }
                 tex.SetData<Color>(test, 0, tex.Width * tex.Height); */

                //SkillTreeBoons.Instance.Logger.Debug(curveData[0]);
                Main.graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                    (VertexPositionNormalTexture[])curveData[0],
                    0,
                    ((VertexPositionNormalTexture[])curveData[0]).Length,
                    (int[])curveData[1],
                    0,
                    ((int[])curveData[1]).Length / 3);
            }

        }
        public static object[] calcCircleVerticies(Vector2 position, float size, int points, float thickness)
        {

            List<VertexPositionNormalTexture> path = new List<VertexPositionNormalTexture>();

            List<int> indices = new List<int>();
            float radius = size / 2;
            float angle = (float)(Math.PI * 2) / points;
            for (int i = 0; i <= points; i++)
            {
                Vector2 normal;
                float theta = i * angle;
                float x = (float)(position.X + radius * Math.Cos(theta));
                float y = (float)(position.Y - radius * Math.Sin(theta));
                Vector2 pos = new Vector2(x, y);
                normal = (position - pos);
                normal.Normalize();
                path.Add(new VertexPositionNormalTexture(new Vector3(pos + normal * thickness, 0), Vector3.Up, new Vector2(0f, 0f)));
                path.Add(new VertexPositionNormalTexture(new Vector3(pos - normal * thickness, 0), Vector3.Up, new Vector2(1f, 1f)));
            }

            for (int x = 0; x < points; x++)
            {
                indices.Add(2 * x + 0);
                indices.Add(2 * x + 1);
                indices.Add(2 * x + 2);

                indices.Add(2 * x + 1);
                indices.Add(2 * x + 3);
                indices.Add(2 * x + 2);
            }

            return new object[] {
                path.ToArray(),
                indices.ToArray()
            };
        }
    }
}
