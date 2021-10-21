using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateThenDie : MonoBehaviour
{
    private SpriteManager anim;
    public string animationName;

    void Start()
    {
        anim = GetComponent<SpriteManager>();
        anim.Play(animationName);
    }

    void Update()
    {
        if (!anim.IsAnimated())
        {
            Destroy(gameObject);
        }
    }
}
