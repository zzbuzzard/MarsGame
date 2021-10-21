using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Speed note: Will be quite slow
// Should check finger locations at fixed time intervals, or at fixed DISTANCE intervals. (use map with fingerID)

// Speed note 2: If it's a bottle-neck, store a list of selected 
//   then loop over this when choosing those to call Attack() Standby() MoveTo() on
public class UnitSelector
{
    // 0 = not selecting
    // 1 = waiting for touch up
    // 2 = waiting for touch down (select clicked)
    // 3 = selecting while dragging
    private int state = 0;
    private const float dragCircleRadius = 0.5f;

    private GameState gState;

    public UnitSelector(GameState myGState)
    {
        gState = myGState;
    }

    public bool ScreenScrollable()
    {
        return state == 0;
    }

    public void SelectClicked()
    {
        state = 1;
    }

    public void MoveToPanelPressed()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                MoveUnits(Camera.main.ScreenToWorldPoint(Input.touches[0].position));
                gState.controller.SetPanel(0);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                MoveUnits(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                gState.controller.SetPanel(0);
            }
        }
    }

    private void MoveUnits(Vector2 worldPos)
    {
        List<Unit> list = new List<Unit>();
        foreach (Unit u in gState.units)
        {
            if (u.IsSelected())
            {
                list.Add(u);
            }
        }

        foreach (Unit u in list)
        {
            u.MoveTo(worldPos, list.Count);
        }
    }

    // 0 = Attack
    // 1 = Standby
    // 2 = Moveto
    public void CommandPressed(int ID)
    {
        // Set the panel where you click to move
        if (ID == 2)
        {
            gState.controller.SetPanel(1);
            return;
        }

        foreach (Unit u in gState.units)
        {
            if (u.IsSelected())
            {
                switch (ID)
                {
                    case 0:
                        u.Attack();
                        break;
                    case 1:
                        u.Standby();
                        break;
                }
            }
        }
    }

    private void CreateSelectionCircle(GameObject obj)
    {
        float xsize = obj.GetComponent<Lifebar>().lifebarScale.x;
        float yoff = obj.GetComponent<Lifebar>().lifebarYOffset;

        GameObject g = Object.Instantiate(gState.selectCircle, obj.transform);
        g.transform.localPosition = new Vector2(0, yoff * 0.6f);
        g.transform.localScale *= xsize;
    }

    public void CheckForClicks()
    {
        if (Application.isMobilePlatform) CheckForClicksMobile();
        else CheckForClicksComputer();
    }

    private bool IsAlly(GameObject obj)
    {
        if (obj.tag != "Ally") return false;
        return obj.GetComponent<Unit>() != null;
    }

    private void CheckForClicksMobile()
    {
        if (state == 0) return;
        if (Input.touchCount == 0)
        {
            // Wait is ready
            if (state == 1)
            {
                state = 2;
                return;
            }

            // Continue waiting
            if (state == 2) return;

            // Drag ends
            if (state == 3)
            {
                gState.controller.SelectDragEnd();
                state = 0;
                return;
            }
        }
        else
        {
            if (state == 2) state = 3;

            foreach (Touch t in Input.touches)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(t.position);
                foreach (Collider2D collider in Physics2D.OverlapCircleAll(pos, dragCircleRadius))
                {
                    if (IsAlly(collider.gameObject))
                    {
                        Unit unit = collider.gameObject.GetComponent<Unit>();
                        if (!unit.IsSelected())
                        {
                            unit.SetSelected(true);
                            CreateSelectionCircle(collider.gameObject);
                        }
                    }
                }
            }
        }


    }

    private void CheckForClicksComputer()
    {
        if (state == 0) return;
        if (!Input.GetMouseButton(0))
        {
            // Wait finished
            if (state == 1)
            {
                state = 2;
                return;
            }

            // Continue waiting
            if (state == 2) return;

            // Drag ends
            if (state == 3)
            {
                state = 0;
                gState.controller.SelectDragEnd();
                return;
            }
        }
        else
        {
            if (state == 2) state = 3;

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(pos, dragCircleRadius))
            {
                if (IsAlly(collider.gameObject))
                {
                    Unit unit = collider.gameObject.GetComponent<Unit>();
                    if (!unit.IsSelected())
                    {
                        unit.SetSelected(true);
                        CreateSelectionCircle(collider.gameObject);
                    }
                }
            }
        }


    }

    public void DeselectAll()
    {
        foreach (Unit u in gState.units)
        {
            if (u.IsSelected())
            {
                // Destroy the circle thing
                Object.Destroy(u.gameObject.transform.GetChild(0).gameObject);
                u.SetSelected(false);
            }
        }
    }
}
