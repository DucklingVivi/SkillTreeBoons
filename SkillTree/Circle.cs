using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkillTreeBoons.SkillTree
{
    public class Circle
    {
        private static Dictionary<int, Circle> _circles = new Dictionary<int, Circle>();
        public Vector2 position = Vector2.Zero;
        public float radius = 0f;
        public int maxMembers = 0;
        public float circoffset = 0f;
        public int id;
        public int circleid = -1;
        public int circleindex = 0;
        public string name = "";
        public static Circle getCircleByID(int id)
        {
            return _circles[id];
        }
        public static Dictionary<int, Circle> GetCircles()
        {
            return new Dictionary<int, Circle>(_circles);
        }
        public void Register()
        {
            _circles.Add(this.id, this);
        }
        public static void reset()
        {
            _circles.Clear();
        }
    }
}
