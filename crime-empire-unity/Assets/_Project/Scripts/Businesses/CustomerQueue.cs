using System.Collections.Generic;
using UnityEngine;

namespace CrimeEmpire.Businesses
{
    /// <summary>Line of customers in front of a counter. Slot 0 is at the counter.</summary>
    public sealed class CustomerQueue : MonoBehaviour
    {
        [SerializeField] Transform[] slots;

        readonly List<Customer> _line = new();

        public bool HasFreeSlot => _line.Count < slots.Length;
        public Customer Head => _line.Count > 0 ? _line[0] : null;
        public int Count => _line.Count;

        public void Enqueue(Customer customer)
        {
            _line.Add(customer);
            customer.MoveTo(slots[_line.Count - 1].position);
        }

        public void Dequeue(Customer customer)
        {
            if (!_line.Remove(customer)) return;
            for (int i = 0; i < _line.Count; i++)
                _line[i].MoveTo(slots[i].position);
        }

        void OnDrawGizmos()
        {
            if (slots == null) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;
                Gizmos.DrawWireSphere(slots[i].position, i == 0 ? 0.4f : 0.25f);
                if (i > 0 && slots[i - 1] != null) Gizmos.DrawLine(slots[i - 1].position, slots[i].position);
            }
        }
    }
}
