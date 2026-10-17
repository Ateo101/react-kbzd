using System;
using CrimeEmpire.Player;
using UnityEngine;

namespace CrimeEmpire.Zones
{
    /// <summary>Reusable "is the player standing here" trigger for pads, counters, cash piles.</summary>
    [RequireComponent(typeof(Collider))]
    public sealed class PlayerTrigger : MonoBehaviour
    {
        public event Action Entered;
        public event Action Exited;

        int _inside;

        public bool IsInside => _inside > 0;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(PlayerController.Tag)) return;
            if (_inside++ == 0) Entered?.Invoke();
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(PlayerController.Tag) || _inside == 0) return;
            if (--_inside == 0) Exited?.Invoke();
        }

        // Unity sends no exit message when a trigger is disabled with the player inside.
        void OnDisable()
        {
            if (_inside == 0) return;
            _inside = 0;
            Exited?.Invoke();
        }
    }
}
