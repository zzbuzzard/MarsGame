using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifebarObjectScript : MonoBehaviour
{
    public Gradient lifebarGradient;
    public GameObject back, bar;

    public GameObject assignedTo;
    private SpriteRenderer barSprite;

    void Start()
    {
        barSprite = bar.GetComponent<SpriteRenderer>();
        ShowHP(1.0f);
    }

    public void ShowHP(float p)
    {
        if (p < 0) p = 0;
        if (p > 1) p = 1;

        bar.transform.localScale = new Vector2(p, 1.0f);
        bar.transform.localPosition = new Vector3(-(1.0f - p) / 2.0f, 0, -0.1f); // slightly in front of back

        Color col = lifebarGradient.Evaluate(p);
        barSprite.color = col;
    }

    public void SetSortingOrder(int o)
    {
        back.GetComponent<SpriteRenderer>().sortingOrder = o;
        bar.GetComponent<SpriteRenderer>().sortingOrder = o + 1;
    }

    private void Update()
    {
        if (assignedTo == null) Destroy(gameObject);
    }
}
