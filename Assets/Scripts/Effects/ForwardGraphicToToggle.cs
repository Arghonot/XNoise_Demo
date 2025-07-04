using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class ForwardGraphicToToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Toggle targetToggle;

        public void OnPointerEnter(PointerEventData eventData)
        {
            ForwardEvent<IPointerEnterHandler>(ExecuteEvents.pointerEnterHandler, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ForwardEvent<IPointerExitHandler>(ExecuteEvents.pointerExitHandler, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ForwardEvent<IPointerClickHandler>(ExecuteEvents.pointerClickHandler, eventData);
        }

        private void ForwardEvent<T>(ExecuteEvents.EventFunction<T> handler, PointerEventData eventData)
            where T : IEventSystemHandler
        {
            if (targetToggle != null)
            {
                ExecuteEvents.Execute(targetToggle.gameObject, eventData, handler);
            }
        }
    }
}