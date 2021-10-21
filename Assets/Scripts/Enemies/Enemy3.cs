using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : Enemy
{
    public const int _hp = 2000, _coinWorth = 100;
    public const float _radius = 1.4f, _speed = 1.2f;

    private void Awake()
    {
        attacks = new Attack[] { new CircleSplashAttack(15, 2.5f, true, 3.0f, 1.0f, false, this, "attack2", false),
                                 new BasicAttack(30, 0.8f, 0.2f, true, this, "attack1") };
        Initialise(_hp, _coinWorth, _radius, _speed);
    }
}
