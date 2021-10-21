using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public const int _hp = 100, _coinWorth = 20;
    public const float _radius = 0.8f, _speed = 2.0f;

    private void Awake()
    {
        attacks = new Attack[] { new BasicAttack(10, 0.8f, 0.15f, true, this, "attack") };
        Initialise(_hp, _coinWorth, _radius, _speed);
    }
}
