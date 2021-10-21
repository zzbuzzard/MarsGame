using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Unit
{
    public const int hp = 100;
    public const float radius = 0.8f, speed = 5.0f;

    private void Awake()
    {
        attacks = new Attack[] { new BasicAttack(5, 0.5f, 0.15f, true, this, "attack") };
        Initialise(hp, radius, speed);
    }
}
