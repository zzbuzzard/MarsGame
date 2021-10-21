using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Attackable
{
    protected override void Initialise(int myHP, float radius)
    {
        base.Initialise(myHP, radius);
    }
    
    protected override void Die()
    {
        ShowCircle(false);
        base.Die();
    }

    // These two are overriden by units which allow placing
    public virtual bool CanPlace(Vector2 point){return false;}
    public virtual void ShowCircle(bool enabled){}

    protected override void ShowDamage(int damage, Vector2 attackLocation)
    {
        Vector2 offset = attackLocation - (Vector2)transform.position;
        Vector3 pos = transform.position + GetRadius() * 0.65f * (Vector3)offset / offset.magnitude;
        pos.z = -5.0f; // Front layer

        // Spawn blood
        Instantiate(gameState.spark, pos, Quaternion.identity);
    }
}
