using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Attackable : MonoBehaviour
{
    public GameState gameState;
    private int HP, maxHP;
    private float radius;

    // How long ago in secs
    //protected float lastRetargetCall = 0;

    protected SpriteManager spriteManager;
    protected SpriteRenderer spriteRenderer;

    protected abstract void ShowDamage(int damage, Vector2 loc);

    protected virtual void Die()
    {
        if (gameObject != null)
        Destroy(gameObject);
        Destroy(this);
    }

    public int GetHP()
    {
        return HP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public float GetRadius()
    {
        return radius;
    }

    public Vector2 GetPos()
    {
        return transform.position;
    }

    // Sprite renderer's sorting order
    public int GetSortingOrder()
    {
        return (int)(-transform.position.y * 10);
    }
    public static int GetSortingOrder(float y)
    {
        return (int)(-y * 10);
    }

    private bool initialised = false;
    protected virtual void Initialise(int myHP, float radius)
    {
        initialised = true;

        spriteManager = GetComponent<SpriteManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        this.radius = radius;
        HP = myHP;
        maxHP = HP;
    }

    public virtual bool TakeDamage(int damage, Vector2 attackLocation)
    {
        ShowDamage(damage, attackLocation);

        HP -= damage;
        if (HP <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    // Accounts for radii
    public static float ADistance(Attackable a, Attackable b)
    {
        return Mathf.Max(0, Vector2.Distance(a.transform.position, b.transform.position) - a.radius - b.radius);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    ////////////////////////////////   Unity functions   ////////////////////////////////
    ////////////////////////////////                     ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    protected virtual void Update()
    {
        if (!initialised)
        {
            Debug.LogWarning("SOMEONE HASN'T BEEN INITIALISED.");
        }

        //lastRetargetCall += Time.deltaTime;

        spriteRenderer.sortingOrder = GetSortingOrder();

        //// Set the z to be a function of y to get that thing where further down = in front
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y * 0.01f);
    }

}
