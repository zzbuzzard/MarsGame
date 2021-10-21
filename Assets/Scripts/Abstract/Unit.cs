using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attack mode:
//  Move towards nearest enemy and attack it. Maintain max distance such that I'm in range.

// Standby mode:
//  Stand still, but still attack an incoming enemy.
//  TODO: Should it break out of standby if it's dying etc? E.g. an archer refusing to move as the enemy draws closer is foolish
//  ... nah, leave it I think

// Move to mode:
//  Move towards target
//  Attack any enemies we get in range of along the way

public enum UnitMode
{
    ATTACK,
    STANDBY,
    MOVETO,
}

public abstract class Unit : Living
{
    private const float distNeededToTargetPos = 0.5f;

    private UnitMode unitMode = UnitMode.ATTACK;
    private Enemy target;
    protected Vector2 targetPos;

    private bool isSelected = false;

    // A random offset meaning we target a random part of the enemy's circumference instead of always the center
    // Prevents grouping, slightly
    // ... very slightly
    private Vector2 enemyTargetOffset;

    public bool IsSelected() { return isSelected; }
    public void SetSelected(bool s) { isSelected = s; }

    public void Attack()
    {
        unitMode = UnitMode.ATTACK;
    }
    public void Standby()
    {
        unitMode = UnitMode.STANDBY;
    }
    public void MoveTo(Vector2 pos, int unitCount)
    {
        unitMode = UnitMode.MOVETO;
        targetPos = pos;

        // Now we want to slightly change targetPos
        // So that all units land uniformly - ish inside a
        // circle of fixed *density*

        // Units/m^2
        const float density = 2.0f;

        // unitCount / area = density
        // area = unitCount / density
        // pi r^2 = unitCount / density
        float radius = Mathf.Sqrt(unitCount / (Mathf.PI * density));
        targetPos += Util.RandomPointInCircle() * radius;
    }

    protected Enemy GetTarget()
    {
        return target;
    }
    protected Vector2 GetTargetPosAdjusted()
    {
        if (target == null) return Vector2.zero;
        return target.GetPos() + enemyTargetOffset;
    }
    protected void SetTarget(Enemy target)
    {
        this.target = target;
        if (target != null)
        {
            enemyTargetOffset = Util.RandomPointOnCircle() * (target.GetRadius() + range1 + GetRadius());
        }
    }

    protected override void Initialise(int myHP, float radius, float speed)
    {
        base.Initialise(myHP, radius, speed);
    }

    protected override void Die()
    {
        // TODO: Unit death stuff

        base.Die();
    }

    public override bool TakeDamage(int damage, Vector2 attackLocation)
    {
        // Show DPS counter?
        bool died = base.TakeDamage(damage, attackLocation);
        return died;
    }

    public void SetGoal(Vector2 moveTo)
    {
        targetPos = moveTo;
    }

    protected bool EnemyInAnyRange()
    {
        if (target == null) return false;
        float mydist = Vector2.Distance(GetPos(), target.GetPos()) - GetRadius() - target.GetRadius();
        return mydist <= range2;
    }

    protected bool EnemyInAllRange()
    {
        if (target == null) return false;
        float mydist = Vector2.Distance(GetPos(), target.GetPos()) - GetRadius() - target.GetRadius();
        return mydist <= range1;
    }

    // Default behaviour:
    //  Target anyone if target is null
    //  Otherwise pick a closeish one, and target if its closer than current
    protected override void Retarget()
    {
        if (GetTarget() == null)
            SetTarget(gameState.GetClosestEnemy(GetPos(), 0.1f));
        else
        {
            Enemy en = gameState.GetClosestEnemy(GetPos(), 0.1f);

            // Only transfer if we couldnt've got them before
            if (Attackable.ADistance(this, en) < 0.9f * Attackable.ADistance(this, GetTarget()))
            {
                SetTarget(en);
            }
        }
    }

    // By default:
    //  Try all attacks on current enemy.
    //  If in ATTACK mode, move towards target
    //  If in MOVETO mode, move towards targetPos
    protected override void Update()
    {
        base.Update();

        float speed = GetDefaultSpeed();

        // Move
        switch (unitMode)
        {
            case UnitMode.MOVETO:
                MoveTowards(targetPos, speed, true);
                if (Vector2.Distance(targetPos, GetPos()) <= distNeededToTargetPos)
                {
                    unitMode = UnitMode.STANDBY;
                }
                break;
            case UnitMode.ATTACK:
                if (target != null && !EnemyInAllRange())
                    MoveTowards(GetTargetPosAdjusted(), speed, true);
                break;
        }

        // Some anims

        // If in combat and not animated, play attack idle
        if (!spriteManager.IsAnimated())
        {
            spriteManager.Play("attack idle");
        }
        // If no target, attack idle
        if (target == null)
        {
            spriteManager.Play("attack idle");
        }

        // Attack
        TryAllAttacks(target);
    }
}
