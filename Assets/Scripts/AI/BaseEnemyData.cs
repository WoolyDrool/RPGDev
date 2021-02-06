using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem.Core.Items;

namespace RPGSystem.AI
{
    [CreateAssetMenu(fileName = "New BaseEnemyData", menuName = "AI/Enemy Data")]
    public class BaseEnemyData : ScriptableObject
    {
        [Header("Enemy Info")]
        [Space(5)]
        public string enemyName;
        public double enemyHealth;

        [Header("Combat Info")]
        [Space(5)]
        public double attackDamage;
        public float attackSpeed;
        public int level;
        public int stamina;

        public List<ItemData> dropPool = new List<ItemData>();
    }
}
