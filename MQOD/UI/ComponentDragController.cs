using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MQOD
{
    public class ComponentDragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform currentTransform;
        public Action callback;
        private Vector3 currentPosition;
        private GameObject mainContent;

        private int totalChild;

        public void OnDrag(PointerEventData eventData)
        {
            currentTransform.position =
                new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

            for (int i = 0; i < totalChild; i++)
                if (i != currentTransform.GetSiblingIndex())
                {
                    Transform otherTransform = mainContent.transform.GetChild(i);
                    int distance = (int)Vector3.Distance(currentTransform.position,
                        otherTransform.position);
                    if (distance <= 10)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, currentPosition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(currentTransform.position.x,
                            otherTransformOldPosition.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPosition = currentTransform.position;
                    }
                }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            currentPosition = currentTransform.position;
            mainContent = currentTransform.parent.gameObject;
            totalChild = mainContent.transform.childCount;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentTransform.position = currentPosition;
            callback();
        }
    }
}