using System;
using System.Collections.Generic;
using CrimeEmpire.WorldMap;
using NUnit.Framework;

namespace CrimeEmpire.Tests
{
    public sealed class WorldMapTests
    {
        static WorldMapState CreateMap() => new(new List<District>
        {
            new() { id = "docks", ownerId = "player", garrison = 100, x = 0, y = 0, neighbors = { "downtown" } },
            new() { id = "downtown", ownerId = "triads", garrison = 100, x = 1, y = 0, neighbors = { "docks", "uptown" } },
            new() { id = "uptown", ownerId = "triads", garrison = 300, x = 2, y = 0, neighbors = { "downtown" } },
        });

        [Test]
        public void CanAttack_OnlyNeighboursOfOwnedDistricts()
        {
            var map = CreateMap();

            Assert.IsTrue(map.CanAttack("player", "docks", "downtown"));
            Assert.IsFalse(map.CanAttack("player", "docks", "uptown"));
            Assert.IsFalse(map.CanAttack("player", "downtown", "uptown"));
        }

        [Test]
        public void March_ResolvesOnlyAfterArrival()
        {
            var map = CreateMap();
            map.SendMarch("player", "docks", "downtown", 500, 0, 1000);

            Assert.IsEmpty(map.Tick(999, new Random(1)));
            var reports = map.Tick(1000, new Random(1));

            Assert.AreEqual(1, reports.Count);
            Assert.IsTrue(reports[0].AttackerWon);
            Assert.AreEqual("player", map.Get("downtown").ownerId);
            Assert.IsEmpty(map.Marches);
        }

        [Test]
        public void SecondMarchOnSameTarget_IsRejected()
        {
            var map = CreateMap();

            Assert.IsNotNull(map.SendMarch("player", "docks", "downtown", 200, 0, 1000));
            Assert.IsNull(map.SendMarch("player", "docks", "downtown", 200, 0, 1000));
        }

        [Test]
        public void Resolve_StrongerSideWins_WithSquareLawLosses()
        {
            var rng = new Random(42);
            for (int i = 0; i < 100; i++)
            {
                var (won, survivors) = WorldMapState.Resolve(200, 100, rng);
                Assert.IsTrue(won);
                Assert.That(survivors, Is.InRange(120, 200)); // sqrt(170²-100²)=137 .. sqrt(230²-100²)=207, clamped to 200
            }

            var (lost, held) = WorldMapState.Resolve(50, 300, new Random(7));
            Assert.IsFalse(lost);
            Assert.Greater(held, 290);
        }

        [Test]
        public void Ai_PicksWeakestBeatableNeighbour()
        {
            var map = CreateMap();
            map.Get("docks").garrison = 80;

            Assert.IsTrue(AIGangBrain.TryPickAttack(map, "triads", 120, 1.3f, out var from, out var target));
            Assert.AreEqual("downtown", from);
            Assert.AreEqual("docks", target);

            Assert.IsFalse(AIGangBrain.TryPickAttack(map, "triads", 50, 1.3f, out _, out _));
        }
    }
}
