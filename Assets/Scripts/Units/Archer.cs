using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public const int hp = 40;
    public const float radius = 0.8f, speed = 6.0f;

    public GameObject arrow;

    private void Awake()
    {
        attacks = new Attack[] { new ProjectileAttack(arrow, 0.7f, 7.5f, true, this, "attack") };
        Initialise(hp, radius, speed);
    }
}
