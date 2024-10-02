using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMath.UI
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool isPointerDown;
        
        public bool IsHeldDown => isPointerDown;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }
    }
}