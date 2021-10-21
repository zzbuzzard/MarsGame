using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Living
{
    // Hmm.. should this be Unit?
    protected Attackable target;
    private int coinWorth;

    protected void Initialise(int myHP, int coinWorth, float radius, float speed)
    {
        base.Initialise(myHP, radius, speed);
        this.coinWorth = coinWorth;
    }

    // Override: green blood
    protected override void ShowDamage(int amount, Vector2 attackLocation)
    {
        Vector2 offset = attackLocation - (Vector2)transform.position;
        Vector3 pos = transform.position + GetRadius() * 0.65f * (Vector3)offset / offset.magnitude;
        pos.z = -5.0f; // Front layer

        // Spawn blood
        Instantiate(gameState.greenblood, pos, Quaternion.identity);
    }

    protected override void Die()
    {
        // TODO: Enemy death stuff

        //gameState.currencyManager.Earn(coinWorth);

        base.Die();
    }

    protected bool UnitInAnyRange()
    {
        if (target == null) return false;
        return Attackable.ADistance(this, target) <= range2;
    }

    protected bool UnitInAllRange()
    {
        if (target == null) return false;
        return Attackable.ADistance(this, target) <= range1;
    }

    protected void TargetCastle()
    {
        Castle c = gameState.GetCastle();
        if (c == null) return;
        target = c;
    }

    protected void TargetClosestUnitBuilding()
    {
        Unit a = gameState.GetClosestUnit(GetPos());
        Building b = gameState.GetClosestBuilding(GetPos());
        if (a == null)
        {
            target = b;
            return;
        }
        if (b == null)
        {
            target = a;
            return;
        }
        float ad = Vector2.Distance(a.GetPos(), GetPos());
        float bd = Vector2.Distance(b.GetPos(), GetPos());
        target = ad < bd ? (Attackable)a : (Attackable)b;
    }

    protected override void Retarget()
    {
        TargetClosestUnitBuilding();
    }


    // By default:
    //  Try all attacks on current unit.
    //  Move towards current unit if it exists and is not in range
    protected override void Update()
    {
        base.Update();

        // Move towards if we can hit em with more attacks
        if (target != null && !UnitInAllRange())
            MoveTowards(target.GetPos(), GetDefaultSpeed(), true);

        // Anims
        if (!spriteManager.IsAnimated())
        {
            spriteManager.Play("attack idle");
        }
        if (target == null)
        {
            spriteManager.Play("attack idle");
        }

        // Attack
        TryAllAttacks(target);
    }
}
