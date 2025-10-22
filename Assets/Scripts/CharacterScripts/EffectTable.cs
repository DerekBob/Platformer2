using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will contain all character attributes and values. Even modifiers (effects) to the character
//think of it as a character profile that must be made on runtime
public class EffectTable : MonoBehaviour
{

    //maybe remove this and have a class hand it down? Or perhaps the effect table contains everything as it can change your name or even animations
    public BaseCharacterProfile baseCharacterProfile;

    //this one is the modified one, you modify this one
    private MovementAttributes currentAttributes;
    private CharacterAnimation currentAnimations;

    // Start is called before the first frame update... and set up all structs and attributes here
    void Start()
    {
        if (baseCharacterProfile == null)
        {
            baseCharacterProfile = new BaseCharacterProfile();
            Debug.LogWarning("baseCharacterProfile is null, filling it in with a default, and its speed is mostly zero so he stuck out there, somewhere");
        }

        //for now just set it to base attributes
        currentAttributes = baseCharacterProfile.defaultAttributes;
        currentAnimations = baseCharacterProfile.characterAnimations;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //we want modified Current attributes
    public MovementAttributes GetCurrentAttributes()
    {
        return currentAttributes;
    }

    //incase we want all default unmodified values
    public BaseCharacterProfile GetBaseCharacterProfile()
    {
        return baseCharacterProfile;
    }

    public CharacterAnimation GetCharacterAnimations()
    {
        return currentAnimations;
    }
}

//Uhhh... this should be a class, things will be changing. what will even decide what world is he in? for now lets just make a static class
public static class WorldAttributes
{
    public static float gravity = 20f;
    public static float moonGravity = 0;

    public static Vector3 windDirection = Vector3.zero;
    public static float windStrength = 0;

    public static float airDensity = 0;

}
