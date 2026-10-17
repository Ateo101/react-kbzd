namespace CrimeEmpire.Economy
{
    public enum CurrencyType
    {
        Coins,      // soft currency: businesses pay it, zones and upgrades cost it
        DirtyCash,  // raids and street jobs; laundered into CleanCash
        Contraband, // business input, capped by warehouse size
        CleanCash,  // laundered money: bosses, world map, alliances
        Diamonds    // premium: IAP and events
    }
}
