using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public bool attackAlly = false;

    public Vector2 direction;

    public float lifetime = 0;
    public float speed = 0;
    public int damage = 0;

    // Leave blank for no animation
    public string playanim = "";

    private float lifeOn;

    void Start()
    {
        isDead = false;

        if (playanim != "")
        {
            GetComponent<SpriteManager>().Play(playanim);
        }

        if (lifetime==0 || speed==0 || damage==0 || direction.magnitude == 0)
        {
            Debug.LogError("Projectile: Something hasn't been set (" + lifetime + ", " + speed + ", " + damage + ", " + direction + ")");
        }

        lifeOn = lifetime;

        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI);
    }

    void Update()
    {
        lifeOn -= Time.deltaTime;
        if (lifeOn <= 0)
        {
            Die();
        }
    }

    private bool isDead;
    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prevents hitting multiple enemies that are close together
        if (isDead) return;

        if (collision.gameObject.tag == "Ally" && attackAlly)
        {
            collision.gameObject.GetComponent<Attackable>().TakeDamage(damage, transform.position);
            Die();
        }
        if (collision.gameObject.tag == "Enemy" && !attackAlly)
        {
            collision.gameObject.GetComponent<Attackable>().TakeDamage(damage, transform.position);
            Die();
        }
    }
}
