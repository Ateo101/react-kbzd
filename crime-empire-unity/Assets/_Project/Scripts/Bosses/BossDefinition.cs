using UnityEngine;

namespace CrimeEmpire.Bosses
{
    public enum BossRarity { Common, Rare, Epic, Legendary }

    /// <summary>Static data for a recruitable boss. One asset per boss.</summary>
    [CreateAssetMenu(menuName = "Crime Empire/Boss", fileName = "Boss_")]
    public sealed class BossDefinition : ScriptableObject
    {
        [SerializeField] string id;
        [SerializeField] string displayName;
        [SerializeField] Sprite portrait;
        [SerializeField] BossRarity rarity;
        [SerializeField, Tooltip("Needs Health + AutoShooter for raids; also shown behind the counter as a manager")]
        GameObject prefab;

        [Header("Combat")]
        [SerializeField] float baseHealth = 200f;
        [SerializeField] float baseDamage = 20f;
        [SerializeField] float fireRate = 2f;
        [SerializeField] float range = 8f;

        [Header("Business")]
        [SerializeField, Range(0f, 0.9f), Tooltip("Service time reduction when managing a business")]
        float serviceSpeedBonus = 0.25f;

        [Header("Growth")]
        [SerializeField] float statGrowthPerLevel = 0.12f;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Portrait => portrait;
        public BossRarity Rarity => rarity;
        public GameObject Prefab => prefab;
        public float FireRate => fireRate;
        public float Range => range;
        public float ServiceSpeedBonus => serviceSpeedBonus;

        public float Health(int level) => baseHealth * Scale(level);
        public float Damage(int level) => baseDamage * Scale(level);

        /// <summary>Rough strength used by the world map to turn a squad into march power.</summary>
        public int Power(int level) => Mathf.RoundToInt((Health(level) / 10f + Damage(level) * fireRate) * 2f);

        float Scale(int level) => 1f + statGrowthPerLevel * (Mathf.Max(1, level) - 1);
    }
}
