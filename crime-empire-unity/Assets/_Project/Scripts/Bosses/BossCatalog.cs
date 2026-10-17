using System.Collections.Generic;
using UnityEngine;

namespace CrimeEmpire.Bosses
{
    [CreateAssetMenu(menuName = "Crime Empire/Boss Catalog", fileName = "BossCatalog")]
    public sealed class BossCatalog : ScriptableObject
    {
        [SerializeField] List<BossDefinition> bosses = new();

        public IReadOnlyList<BossDefinition> All => bosses;

        public BossDefinition Find(string id) =>
            string.IsNullOrEmpty(id) ? null : bosses.Find(boss => boss != null && boss.Id == id);
    }
}
