using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillTreeBoons.SkillTree
{
    public class Group
    {
        public static List<int> startingGroups = new List<int>();
        private static Dictionary<int,Group> _groups = new Dictionary<int, Group>();
        public List<int> nodes = new List<int>();
        public string name = "";
        public string tooltip = "";
        public string texture = "SkillTreeBoons/UI/Slot_Back";
        public List<int> connections = new List<int>();
        public float x = 0; // GRID
        public float y = 0; // GRID
        public float size = 70f;
        public string color = "327DFF";
        public int id = -1;
        public bool startingGroup = false;
        public static Group getGroupByID(int id)
        {
            return _groups[id];
        }
        public static Group CreateGroup()
        {
            return new Group();
        }
        public static Dictionary<int, Group> GetGroups()
        {
            return new Dictionary<int, Group>(_groups);
        }
        public void Register()
        {
            _groups.Add(this.id, this);
            if(startingGroup)
            {
                startingGroups.Add(this.id);
            }
        }
        public static void reset()
        {
            _groups.Clear();
            startingGroups.Clear();
        }

        
        
    }
}
