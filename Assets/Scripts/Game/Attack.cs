using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack
{
    readonly protected bool passiveReload;
    readonly private float reload, range;
    readonly protected string animationName;

    private float t;
    private float animateWhenT;
    private bool thisTAnimated;

    readonly protected Living attacker;

    public Attack(float reload, float range, bool passiveReload, Living attacker, string animationName="")
    {
        this.reload = reload;
        this.range = range;
        this.passiveReload = passiveReload;
        this.attacker = attacker;
        this.animationName = animationName;

        thisTAnimated = false;

        if (animationName != "")
        {
            // Time taken to animate, start it so the last frame begins just as the attack commences
            animateWhenT = attacker.GetComponent<SpriteManager>().TimeToLastFrame(animationName);
            if (animateWhenT > reload)
            {
                Debug.LogWarning("Attack has animation time greater than reload time! "  + attacker.name);
            }
        }
        else animateWhenT = 0;

        t = reload;
    }

    public float GetRange()
    {
        return range;
    }
    public float GetReload()
    {
        return reload;
    }

    protected abstract void Animate(Attackable en, Vector2 pos);
    protected abstract void Perform(Attackable en, Vector2 myPos);
    public void TryAttack(Attackable enemy, Vector2 myPos)
    {
        float realDist = Attackable.ADistance(attacker, enemy);
        if (realDist <= range)
        {
            t -= Time.deltaTime;

            // Begin animation
            if (t <= animateWhenT && !thisTAnimated)
            {
                thisTAnimated = true;
                Animate(enemy, myPos);
            }

            if (t <= 0)
            {
                thisTAnimated = false;

                t += reload;
                Perform(enemy, myPos);
            }
        }
        else
        {
            if (passiveReload)
            {
                t = Mathf.Max(0, t - Time.deltaTime);
            }
        }
    }
}

public class BasicAttack : Attack
{
    readonly private int damage;

    public BasicAttack(int damage, float reload, float range, bool passiveReload, Living attacker, string animationName="")
        : base(reload, range, passiveReload, attacker, animationName)
    {
        this.damage = damage;
    }

    protected override void Animate(Attackable en, Vector2 pos)
    {
        if (animationName != "")
        {
            attacker.GetComponent<SpriteManager>().Play(animationName);
        }
    }

    protected override void Perform(Attackable en, Vector2 myPos)
    {
        en.TakeDamage(damage, myPos);
    }
}

public class ProjectileAttack : Attack
{
    readonly private GameObject spawnPrefab;

    public ProjectileAttack(GameObject spawnPrefab, float reload, float range, bool passiveReload, Living attacker, string animationName = "")
        : base(reload, range, passiveReload, attacker, animationName)
    {
        this.spawnPrefab = spawnPrefab;
    }

    protected override void Animate(Attackable en, Vector2 pos)
    {
        if (animationName != "")
        {
            attacker.GetComponent<SpriteManager>().Play(animationName);
        }
    }

    protected override void Perform(Attackable en, Vector2 myPos)
    {
        GameObject shot = Object.Instantiate(spawnPrefab, myPos, Quaternion.identity);
        shot.GetComponent<BasicProjectile>().direction = en.GetPos() - myPos;
    }
}

public class CircleSplashAttack : Attack
{
    readonly private int damage;
    readonly private float radius;
    readonly private bool explodesOnEnemy;
    readonly private bool attackAlly;

    // explodesOnEnemy : If true then center is enemy, otherwise the center is the attack user
    public CircleSplashAttack(int damage, float radius, bool attackAlly, float reload, float range, bool passiveReload, Living attacker, string animationName = "", bool explodesOnEnemy = true)
        : base(reload, range, passiveReload, attacker, animationName)
    {
        this.damage = damage;
        this.radius = radius;
        this.explodesOnEnemy = explodesOnEnemy;
        this.attackAlly = attackAlly;
    }

    protected override void Animate(Attackable en, Vector2 pos)
    {
        if (animationName != "")
        {
            attacker.GetComponent<SpriteManager>().Play(animationName);
        }
    }

    protected override void Perform(Attackable en, Vector2 myPos)
    {
        if (explodesOnEnemy)
            en.gameState.Explode(en.GetPos(), radius, damage, attackAlly);
        else
            en.gameState.Explode(attacker.GetPos(), radius, damage, attackAlly);
    }
}