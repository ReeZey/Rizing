using UnityEngine;
using UnityEngine.EventSystems;

namespace Rizing.UI {
    public class Draggable : MonoBehaviour, IDragHandler {
        [SerializeField]
        private RectTransform _rectTransform;
    
        public void OnDrag(PointerEventData eventData) {
            _rectTransform.position += new Vector3(eventData.delta.x,eventData.delta.y,0);
        }
    }
}
