namespace CrimeEmpire.WorldMap
{
    /// <summary>Offline rival gangs for the MVP. Replaced by real players when PvP goes online.</summary>
    public static class AIGangBrain
    {
        /// <summary>Picks the weakest neighbour the gang can beat with a safety margin.</summary>
        public static bool TryPickAttack(WorldMapState map, string gangId, int availablePower, float margin,
            out string fromId, out string targetId)
        {
            fromId = null;
            targetId = null;
            int weakest = int.MaxValue;

            foreach (var from in map.Districts)
            {
                if (from.ownerId != gangId) continue;
                foreach (var neighborId in from.neighbors)
                {
                    var target = map.Get(neighborId);
                    if (target == null || target.garrison >= weakest) continue;
                    if (availablePower < target.garrison * margin) continue;
                    if (!map.CanAttack(gangId, from.id, target.id)) continue;

                    weakest = target.garrison;
                    fromId = from.id;
                    targetId = target.id;
                }
            }
            return targetId != null;
        }
    }
}
