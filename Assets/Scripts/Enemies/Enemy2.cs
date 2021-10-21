using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    public const int _hp = 65, _coinWorth = 35;
    public const float _radius = 0.8f, _speed = 1.4f;
    public GameObject shoot;

    private void Awake()
    {
        attacks = new Attack[] { new ProjectileAttack(shoot, 1.2f, 6.0f, true, this, "attack") };
        Initialise(_hp, _coinWorth, _radius, _speed);
    }
}
