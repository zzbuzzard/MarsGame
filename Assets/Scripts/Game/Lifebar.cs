using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attackable))]
public class Lifebar : MonoBehaviour
{
    public static GameObject barprefab;

    public Vector2 lifebarScale;
    public float lifebarYOffset;

    private LifebarObjectScript lifebar;
    private Attackable attackable;

    private static void LoadResources()
    {
        barprefab = Resources.Load<GameObject>("Prefabs/lifebar");
    }

    void Start()
    {
        if (lifebar == null) LoadResources();

        Vector3 pos = transform.position;
        pos.y += lifebarYOffset;

        GameObject lifebarObj = Instantiate(barprefab, pos, Quaternion.identity);
        lifebarObj.transform.localScale = lifebarScale;

        lifebar = lifebarObj.GetComponent<LifebarObjectScript>();
        lifebar.assignedTo = gameObject;
        attackable = GetComponent<Attackable>();
    }

    void Update()
    {
        float p = attackable.GetHP() / (float)attackable.GetMaxHP();
        lifebar.ShowHP(p);

        lifebar.SetSortingOrder(attackable.GetSortingOrder());

        Vector3 pos = transform.position;
        pos.y += lifebarYOffset;

        lifebar.gameObject.transform.position = pos;
    }
}
