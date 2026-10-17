using UnityEngine;
using UnityEngine.EventSystems;

namespace CrimeEmpire.Player
{
    /// <summary>
    /// Floating joystick: appears where the finger touches. Put it on a transparent, raycastable
    /// full-screen Image; <see cref="background"/> must be its direct child, <see cref="handle"/> a child of background.
    /// </summary>
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] RectTransform background;
        [SerializeField] RectTransform handle;
        [SerializeField] float radius = 90f;
        [SerializeField, Range(0f, 0.5f)] float deadZone = 0.1f;
        [SerializeField] bool floating = true;
        [SerializeField] bool hideWhenIdle = true;

        public Vector2 Value { get; private set; }

        RectTransform _area;
        Vector3 _restPosition;

        void Awake()
        {
            _area = (RectTransform)transform;
            _restPosition = background.localPosition;
            if (hideWhenIdle) background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (floating &&
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_area, eventData.position, eventData.pressEventCamera, out var local))
                background.localPosition = local;

            background.gameObject.SetActive(true);
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out var local))
                return;

            var value = Vector2.ClampMagnitude(local / radius, 1f);
            handle.anchoredPosition = value * radius;
            Value = value.magnitude < deadZone ? Vector2.zero : value;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            if (floating) background.localPosition = _restPosition;
            if (hideWhenIdle) background.gameObject.SetActive(false);
        }

        void OnDisable() => Value = Vector2.zero;
    }
}
