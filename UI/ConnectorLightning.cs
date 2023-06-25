using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SkillTreeBoons.UI
{
    public class ConnectorLightning
    {

        public static BasicEffect effect;
        
        public ConnectorLightning()
        {

        }

        public void Initalize()
        {
            
        }
       
        public static void Draw(SpriteBatch spriteBatch, Vector2 pos1, Vector2 pos2, Color color, float scale = 30f, float offset2 = 0f, float linethickness = 10f)
        {
            if (effect == null) effect = new BasicEffect(Main.graphics.GraphicsDevice);
            /*Vector2 slope = pos2 - pos1;
            float distance = slope.Length();
            Vector2 specialnorm = Vector2.Normalize(new Vector2(slope.Y, -slope.X));
            Vector2 prevPoint = pos1;
            for (int i = 1; i < positions.Count; i++)
            {
                float scale = (distance * Jaggedness) * (positions[i] - positions[i - 1]);

                float envelope = positions[i] > 0.8f || positions[i] < 0.2f ? scale * (1 - positions[i]) : spread;
                float olddisplacement = displacements[i];
                displacements[i] -= (displacements[i] - displacements[i - 1]) * (1 - scale);
                displacements[i] *= envelope;
                Vector2 point = pos1 + (positions[i] * slope) + (displacements[i] * specialnorm);
                displacements[i] = olddisplacement;
                DrawLine(spriteBatch, point, prevPoint, Color.Black);
                prevPoint = point;
            }*/
            Vector2 offset = new Vector2(Main.screenWidth /2, Main.screenHeight/2);
            pos1 -= offset;
            pos2 -= offset;
            pos2.Y *= -1;
            pos1.Y *= -1;
           
            Vector2[] points3d = computePoints3D(pos1, pos2, scale, offset2);
            int curvepoints = scale > 0 ? 20 : 1;
            object[] curveData = computeCurve3D(curvepoints, points3d, linethickness);

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

        
        public static Vector2[] computePoints3D(Vector2 start, Vector2 end, float scale, float offset)
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(start);

            Vector2 slope = end - start;
            float distance = slope.Length();
            Vector2 specialnorm = Vector2.Normalize(new Vector2(slope.Y, -slope.X));

            Vector2 point1 = start + (0.33f * slope) + ((float)Math.Sin(Main.timeForVisualEffects / 40f + offset * Math.PI) * specialnorm * scale);
            Vector2 point2 = start + (0.66f * slope) -((float)Math.Sin(Main.timeForVisualEffects / 40f + offset * Math.PI) * specialnorm * scale);


            points.Add(point1);
            points.Add(point2);

            points.Add(end);
            return points.ToArray();
        }
        public static object[] computeCurve3D(int steps, Vector2[] points3D,float linethickness)
        {

            List<VertexPositionNormalTexture> path = new List<VertexPositionNormalTexture>();

            List<int> indices = new List<int>();

            List<Vector2> curvePoints = new List<Vector2>();
            for (float x = 0; x < 1; x += 1 / (float)steps)
            {
                curvePoints.Add(getBezierPointRecursive(x, points3D));
            }
            curvePoints.Add(getBezierPointRecursive(1, points3D));

            for (int x = 0; x < curvePoints.Count; x++)
            {
                Vector2 normal;

                if (x == 0)
                {
                    //First point, Take normal from first line segment
                    normal = getNormalizedVector(getLineNormal(curvePoints[x + 1] - curvePoints[x]));
                }
                else if (x + 1 == curvePoints.Count)
                {
                    //Last point, take normal from last line segment
                    normal = getNormalizedVector(getLineNormal(curvePoints[x] - curvePoints[x - 1]));
                }
                else
                {
                    //Middle point, interpolate normals from adjacent line segments
                    normal = getNormalizedVertexNormal(getLineNormal(curvePoints[x] - curvePoints[x - 1]), getLineNormal(curvePoints[x + 1] - curvePoints[x]));
                }

                path.Add(new VertexPositionNormalTexture(new Vector3(curvePoints[x] + normal * linethickness, 0), Vector3.Up, new Vector2(0f,0f)));
                path.Add(new VertexPositionNormalTexture(new Vector3(curvePoints[x] + normal * -linethickness, 0), Vector3.Up, new Vector2(1f,1f)));
            }

            for (int x = 0; x < curvePoints.Count - 1; x++)
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

        //Recursive algorithm for getting the bezier curve points 
        private static Vector2 getBezierPointRecursive(float timeStep, Vector2[] ps)
        {

            if (ps.Length > 2)
            {
                List<Vector2> newPoints = new List<Vector2>();
                for (int x = 0; x < ps.Length - 1; x++)
                {
                    newPoints.Add(interpolatedPoint(ps[x], ps[x + 1], timeStep));
                }
                return getBezierPointRecursive(timeStep, newPoints.ToArray());
            }
            else
            {
                return interpolatedPoint(ps[0], ps[1], timeStep);
            }
        }

        //Gets the interpolated Vector2 based on t
        private static Vector2 interpolatedPoint(Vector2 p1, Vector2 p2, float t)
        {
            return Vector2.Multiply(p2 - p1, t) + p1;
        }

        //Gets the normalized normal of a vertex, given two adjacent normals (2D)
        private static Vector2 getNormalizedVertexNormal(Vector2 v1, Vector2 v2) //v1 and v2 are normals
        {
            return getNormalizedVector(v1 + v2);
        }

        //Normalizes the given Vector2
        private static Vector2 getNormalizedVector(Vector2 v)
        {
            Vector2 temp = new Vector2(v.X, v.Y);
            v.Normalize();
            return v;
        }

        //Gets the normal of a given Vector2
        private static Vector2 getLineNormal(Vector2 v)
        {
            Vector2 normal = new Vector2(v.Y, -v.X);
            return normal;
        }

    }
}
