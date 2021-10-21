using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraPanZoom : MonoBehaviour
{
    public RectTransform panel;
    public Controller controller;

    bool drag = false;
    Vector2 dragStart;
    Vector2 origPos;
    int draggerID;

    private const float mul = 4.5f;
    private const float z = -10;

    private const float minZoom = 2.5f, maxZoom = 25.0f, zoomMul = 1.4f;
    bool zoomin = false;
    float prevdist;

    private float divide;
    
    private void Pan(Vector2 change)
    {
        Vector3 val = origPos - change * mul * Camera.main.orthographicSize / divide;
        val.z = z;
        transform.position = val;
    }

    private void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        // Clamp to avoid movement at limits
        float size = Camera.main.orthographicSize;
        amount = Mathf.Clamp(amount, size - maxZoom, size - minZoom);

        float multiplier = 1.0f / Camera.main.orthographicSize * amount;
        transform.position += (zoomTowards - transform.position) * multiplier;
        Camera.main.orthographicSize -= amount;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
    }

    private void SetZoom(float zoom)
    {
        Camera.main.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    private bool ValidTouchStart(Vector2 pos)
    {
        return !RectTransformUtility.RectangleContainsScreenPoint(panel, pos);
    }

    int prevTouchCount = -1;
    private void Update()
    {
        if (!controller.ScreenScrollable()) return;

        divide = Mathf.Max(Screen.width, Screen.height);

        // TODO: Remove in release in case some devices don't trigger this
        if (Application.isMobilePlatform)
        {
            // Start drag
            if (prevTouchCount == 0 && Input.touchCount == 1)
            {
                Vector2 pos = Input.GetTouch(0).position;
                if (ValidTouchStart(pos))
                {
                    drag = true;
                    dragStart = pos;
                    origPos = transform.position;

                    draggerID = Input.GetTouch(0).fingerId;
                }
            }

            // Continue drag
            if (drag)
            {
                if (Input.touchCount == 1)
                {
                    Touch t = Input.GetTouch(0);
                    if (t.fingerId == draggerID)
                    {
                        Pan(t.position - dragStart);
                    }
                    else
                    {
                        // We let go and this is a different finger
                        drag = false;
                    }
                }
                else
                {
                    drag = false;
                }
            }

            prevTouchCount = Input.touchCount;

            // ZOOM
            if (Input.touchCount == 2)
            {
                Touch a = Input.GetTouch(0);
                Touch b = Input.GetTouch(1);

                // Pixel -> relative coordinates
                Vector2 p1 = Input.GetTouch(0).position / divide;
                Vector2 p2 = Input.GetTouch(1).position / divide;

                //p1.x /= Screen.width; p2.x /= Screen.width;
                //p1.y /= Screen.height; p2.y /= Screen.height;

                float dist = 40.0f * Vector2.Distance(p1, p2);

                if (zoomin)
                {
                    Vector3 focus = Camera.main.ScreenToWorldPoint((a.position + b.position) / 2);
                    //SetZoom(origZoom * startDist / dist);

                    ZoomOrthoCamera(focus, zoomMul * (dist - prevdist));
                }
                else
                {
                    zoomin = true;
                }

                prevdist = dist;
            }
            else
            {
                zoomin = false;
            }

        }

        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = Input.mousePosition;
                if (ValidTouchStart(pos))
                {
                    drag = true;
                    dragStart = pos;
                    origPos = transform.position;
                }
            }

            if (drag)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 p = Input.mousePosition;
                    Pan(p - dragStart);
                }
                else
                {
                    drag = false;
                }
            }


            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), 5 * zoomMul * Input.GetAxis("Mouse ScrollWheel"));
            //SetZoom(Camera.main.orthographicSize - 5 * );
        }

    }
}
