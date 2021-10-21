using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Rules:
// Walk overrides attack, attackidle
// Attackidle overrides [no anim]
// Attack overrides attackidle
// None overrides all

[System.Serializable] // Makes it editable in the editor
public enum AnimType
{
    NONE,
    WALK,
    ATTACK,
    ATTACK_IDLE,
}

[System.Serializable]
public class AnimList
{
    // This is how the animation will be referenced in code
    public string name;

    // Time per frame
    public float switchTime;

    // If set to false, plays once and freezes on last frame (until handled by something else)
    public bool loop = true;

    public AnimType animType = AnimType.NONE;

    // obv
    public Sprite[] sprites;
}

public class SpriteManager : MonoBehaviour
{
    // Public fields
    public Sprite baseSprite;
    public AnimList[] animations;

    private IDictionary<string, int> nameToHash; // Animation name -> ID in list
    private SpriteRenderer rend;

    private int ind;
    private AnimList currentAnim;
    private int currentAnimIndex;

    private float thisTime;
    private bool animated = false;

    void Awake()
    {
        InitialiseDict();

        rend = GetComponent<SpriteRenderer>();
        rend.sprite = baseSprite;
        Stop();
    }

    private void InitialiseDict()
    {
        nameToHash = new Dictionary<string, int>();
        int c = 0;
        foreach (AnimList anim in animations)
        {
            nameToHash[anim.name] = c++;
        }
    }

    public bool IsAnimated()
    {
        return animated;
    }

    private bool PlayAnimType(AnimType newType)
    {
        if (currentAnim == null) return true;
        if (currentAnim.animType == AnimType.NONE) return true;
        if (newType == AnimType.NONE) return true;
        
        AnimType c = currentAnim.animType;

        if (newType == AnimType.ATTACK)
        {
            if (c == AnimType.ATTACK_IDLE) return true;
            if (c == AnimType.WALK) return true;
        }
        if (newType == AnimType.ATTACK_IDLE)
        {
            if (c == AnimType.WALK) return true;
        }
        if (newType == AnimType.WALK)
        {
            if (c == AnimType.ATTACK) return true;
            if (c == AnimType.ATTACK_IDLE) return true;
        }
        return false;
    }

    // Restart if current: If true then when anim is already playing it will interrupt it and restart
    //  Otherwise it will do nothing
    public void Play(string anim, bool restartIfCurrent=false)
    {
        if (!nameToHash.ContainsKey(anim))
        {
            Debug.LogWarning("ANIMATION \"" + anim + "\" NOT FOUND, WYD? ");
            return;
        }

        int index = nameToHash[anim];
        AnimList myList = animations[index];

        // Stop if it's the same one again [unless told to restart]
        if (index == currentAnimIndex && !restartIfCurrent) return;

        // Stop if this animation doesn't happen here (e.g. attackidle trying to overtake attack)
        if (!PlayAnimType(myList.animType)) return;

        animated = true;
        currentAnim = animations[index];
        currentAnimIndex = index;

        // -1 so the next one is 0
        ind = -1;
        thisTime = 0;

    }

    public void Stop()
    {
        animated = false;
        currentAnim = null;
        currentAnimIndex = -1;

        thisTime = 0;
        ind = -1;
    }

    // Time between animation start and the start of the final frame.
    public float TimeToLastFrame(string anim)
    {
        if (!nameToHash.ContainsKey(anim))
        {
            Debug.LogWarning("TimeToLastFrame called on unknown animation, " + anim);
            return 0;
        }

        AnimList a = animations[nameToHash[anim]];

        return a.switchTime * (a.sprites.Length - 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (animated)
        {
            thisTime -= Time.deltaTime;
            if (thisTime <= 0)
            {
                ind++;

                if (ind >= currentAnim.sprites.Length)
                {
                    // Reset animation
                    if (currentAnim.loop)
                    {
                        ind = 0;
                    }
                    else
                    // End animation
                    {
                        Stop();
                        return;
                    }
                }

                thisTime += currentAnim.switchTime;
                rend.sprite = currentAnim.sprites[ind];
            }
        }
    }
}
