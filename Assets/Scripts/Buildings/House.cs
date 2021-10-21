using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    // For showing where units can be placed
    public GameObject circlePrefab;
    private float placeRange = 6.0f;

    private float incomePerSec = 2.0f;
    private float uncashedIncome = 0;

    const int hp = 1000;
    const float radius = 1.5f;

    private GameObject circleObj;

    private void Awake()
    {
        base.Initialise(hp, radius);

        circleObj = Instantiate(circlePrefab, transform.position, Quaternion.identity);
        circleObj.transform.localScale = new Vector2(2*placeRange - 1, 2*placeRange - 1);
        ShowCircle(false);
    }

    protected override void Update()
    {
        base.Update();

        uncashedIncome += incomePerSec * Time.deltaTime;
        if (uncashedIncome >= 1)
        {
            int get = (int)uncashedIncome;
            uncashedIncome -= get;
            gameState.currencyManager.Earn(get);
        }
    }

    public override bool CanPlace(Vector2 point)
    {
        return Vector2.Distance(GetPos(), point) <= placeRange;
    }

    public override void ShowCircle(bool enabled)
    {
        circleObj.SetActive(enabled);
    }

}
