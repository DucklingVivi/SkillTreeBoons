using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillTreeBoons.SkillTree
{
    public class Node
    {
        public static List<int> startingNodes = new List<int>();
        private static Dictionary<int, Node> _nodes = new Dictionary<int, Node>();
        public string name = "";
        public string tooltip = "";
        public string texture = "SkillTreeBoons/UI/Slot_Back_Circle";
        public int id = -1;
        public bool square = false;
        public string shape = "circle";
        public int circleid = -1;
        public int circleindex = 0;
        public bool notable = false;
        public List<int> connections = new List<int>();
        public float x = 0; // GRID
        public float y = 0; // GRID
        public float size = 70f;
        public float toolSpeed = 1f;
        public bool startingNode = false;
        public bool adrenaline = false;
        public bool wingslot = false;
        public bool rage = false;
        public float respawnSpeed = 1f;
        public int respawnHealth = 0;
        public int additionalAccessories = 0;
        public int unlocksGroup = -1;
        public int rarity = 0;
        public int additionalMinions = 0;
        public int additionalSentries = 0;
        public int maxLife = 0;
        public int maxMana = 0;
        public float maxLifePercent = 1f;
        public float maxManaPercent = 1f;
        public float lifeRegen = 0f;
        public float manaRegen = 0f;
        public bool uncapMana = false;
        public float damage = 1f;
        public float crit = 0f;
        public float meleeDamage = 1f;
        public float rangedDamage = 1f;
        public float magicDamage = 1f;
        public float minionDamage = 1f;
        public float rogueDamage = 1f;
        
        public Node()
        {

        }
        public static Node CreateNode()
        {
            return new Node();
        }
        public void RegisterNode()
        {
            _nodes.Add(id, this);
            if (startingNode)
            {
                startingNodes.Add(id);
            }
        }
        public static void reset()
        {
            _nodes.Clear();
            startingNodes.Clear();
        }

        public static Dictionary<int, Node> GetNodes()
        {
            return new Dictionary<int, Node>(_nodes);
        }
        
    }
}
