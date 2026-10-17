using System;
using System.Collections.Generic;
using CrimeEmpire.Bosses;
using CrimeEmpire.Economy;

namespace CrimeEmpire.Save
{
    /// <summary>JsonUtility-friendly snapshot. Dictionaries are stored as key/value lists, enums as names.</summary>
    [Serializable]
    public sealed class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public long lastSeenUtcTicks;
        public List<LongEntry> currencies = new();
        public List<LongEntry> caps = new();
        public List<string> unlocked = new();
        public List<LongEntry> payments = new();
        public List<IntEntry> levels = new();
        public List<IntEntry> bosses = new();
        public List<StringEntry> assignments = new();
        public List<StringEntry> districtOwners = new();

        public static SaveData Capture(Wallet wallet, ProgressState progress, BossRoster roster, DateTime utcNow)
        {
            var data = new SaveData { lastSeenUtcTicks = utcNow.Ticks };
            foreach (var pair in wallet.Amounts) data.currencies.Add(new LongEntry(pair.Key.ToString(), pair.Value));
            foreach (var pair in wallet.Caps) data.caps.Add(new LongEntry(pair.Key.ToString(), pair.Value));
            data.unlocked.AddRange(progress.Unlocked);
            foreach (var pair in progress.Payments) data.payments.Add(new LongEntry(pair.Key, pair.Value));
            foreach (var pair in progress.Levels) data.levels.Add(new IntEntry(pair.Key, pair.Value));
            foreach (var pair in progress.DistrictOwners) data.districtOwners.Add(new StringEntry(pair.Key, pair.Value));
            foreach (var pair in roster.Levels) data.bosses.Add(new IntEntry(pair.Key, pair.Value));
            foreach (var pair in roster.Assignments) data.assignments.Add(new StringEntry(pair.Key, pair.Value));
            return data;
        }

        public void ApplyTo(Wallet wallet, ProgressState progress, BossRoster roster)
        {
            // Caps first, so restored amounts are clamped by the saved warehouse size.
            foreach (var entry in caps)
                if (Enum.TryParse(entry.key, out CurrencyType type)) wallet.SetCap(type, entry.value);
            foreach (var entry in currencies)
                if (Enum.TryParse(entry.key, out CurrencyType type)) wallet.Restore(type, entry.value);

            foreach (var key in unlocked) progress.MarkUnlocked(key);
            foreach (var entry in payments) progress.SetPaid(entry.key, entry.value);
            foreach (var entry in levels) progress.SetLevel(entry.key, entry.value);
            foreach (var entry in districtOwners) progress.SetDistrictOwner(entry.key, entry.value);

            foreach (var entry in bosses) roster.Restore(entry.key, entry.value);
            foreach (var entry in assignments) roster.Assign(entry.key, entry.value);
        }
    }

    [Serializable]
    public struct LongEntry
    {
        public string key;
        public long value;
        public LongEntry(string key, long value) { this.key = key; this.value = value; }
    }

    [Serializable]
    public struct IntEntry
    {
        public string key;
        public int value;
        public IntEntry(string key, int value) { this.key = key; this.value = value; }
    }

    [Serializable]
    public struct StringEntry
    {
        public string key;
        public string value;
        public StringEntry(string key, string value) { this.key = key; this.value = value; }
    }
}
