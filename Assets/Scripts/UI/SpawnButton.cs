using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    public GameObject panel;
    public int unitID;
    GameObject childImage;

    Controller controller;

    public void OnBeginDrag(PointerEventData eventData)
    {
        controller.GetGameState().DragBegin();
    }

    public void OnDrag(PointerEventData eventData)
    {
        childImage.transform.SetParent(panel.transform);
        childImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        controller.GetGameState().DragEnd();

        childImage.transform.SetParent(transform);
        childImage.transform.localPosition = Vector3.zero;

        Vector2 dropPos = eventData.position;

        if (!RectTransformUtility.RectangleContainsScreenPoint(panel.GetComponent<RectTransform>(), dropPos))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(dropPos);
            controller.GetGameState().TryPlacing(unitID, pos);
        }
    }

    private void Start()
    {
        childImage = transform.GetChild(0).gameObject;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }
}
