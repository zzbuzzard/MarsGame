using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Living : Attackable
{
    protected Attack[] attacks;
    protected float range1, range2; // Only for determining movement - don't approach targets if they're already within range
    private float speed; // For default movement in Enemy and Unit

    private bool isFlipped;


    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    ////////////////////////////////   Getters/Setters   ////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    public float GetDefaultSpeed()
    {
        return speed;
    }

    protected virtual void Initialise(int myHP, float radius, float speed)
    {
        base.Initialise(myHP, radius);    
        this.speed = speed;

        RefreshRange();
        StartRetargetRoutine();
    }


    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    ////////////////////////////////       Abstract      ////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    protected abstract void Retarget();


    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    ////////////////////////////////      Utilities      ////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    protected void TryAllAttacks(Attackable attackTarget)
    {
        if (attackTarget == null || attacks == null) return;

        foreach (Attack atk in attacks)
        {
            atk.TryAttack(attackTarget, transform.position);
        }
    }

    // Get min and max range
    protected void RefreshRange()
    {
        if (attacks == null)
        {
            range1 = range2 = 0;
        }
        else
        {
            // Get max attack range
            float mx = 0;
            float mn = attacks[0].GetRange();
            foreach (Attack atk in attacks)
            {
                float p = atk.GetRange();
                if (p > mx) mx = p;
                if (p < mn) mn = p;
            }

            range1 = mn;
            range2 = mx;
        }
    }

    // If animate=true, then attempt to play animation "walk"
    protected void MoveTowards(Vector2 goal, float speed, bool animate=false, bool flipsX=true)
    {
        if (animate) spriteManager.Play("walk");

        Vector2 disp = (goal - (Vector2)transform.position);
        float dist = Mathf.Max(disp.magnitude - GetRadius(), 0);
        disp = disp.normalized;
        transform.position += (Vector3)disp * speed * Time.deltaTime;

        if (flipsX && dist > 2.0f)
        {
            if (disp.x < -0.75f) spriteRenderer.flipX = true;
            if (disp.x > 0.75f) spriteRenderer.flipX = false;
            // Otherwise, leave unchanged - looks more natural.
        }
    }



    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    ////////////////////////////////  Attackable methods ////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    
    // Can be overriden for different effects
    protected override void ShowDamage(int amount, Vector2 attackLocation)
    {
        Vector2 offset = attackLocation - (Vector2)transform.position;
        Vector3 pos = transform.position + GetRadius() * 0.65f * (Vector3)offset / offset.magnitude;
        pos.z = -5.0f; // Front layer

        // Spawn blood
        Instantiate(gameState.redblood, pos, Quaternion.identity);
    }



    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    ////////////////////////////////   Unity functions   ////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    private void StartRetargetRoutine()
    {
        StartCoroutine(RetargetRoutine());
    }

    private IEnumerator RetargetRoutine()
    {
        const float retargetWait = 0.4f;
        yield return new WaitForEndOfFrame(); // Prevents Retarget running when gameState hasn't been set yet
        while (true)
        {
            Retarget();
            yield return new WaitForSeconds(retargetWait);
        }
    }
}
