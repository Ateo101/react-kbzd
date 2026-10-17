using CrimeEmpire.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace CrimeEmpire.UI
{
    /// <summary>Small health bar over a unit (green for the gang, red for the police).</summary>
    public sealed class WorldSpaceBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] Image fill;
        [SerializeField] Color gangColor = new(0.35f, 0.85f, 0.3f);
        [SerializeField] Color enemyColor = new(0.9f, 0.2f, 0.2f);
        [SerializeField] bool hideWhenFull = true;

        Camera _camera;

        void Awake() => _camera = Camera.main;

        void LateUpdate()
        {
            if (health == null || fill == null) return;

            fill.fillAmount = health.Normalized;
            fill.color = health.Team == Team.Gang ? gangColor : enemyColor;
            fill.enabled = !health.IsDead && !(hideWhenFull && health.Normalized >= 1f);

            if (_camera != null) transform.rotation = _camera.transform.rotation;
        }
    }
}
