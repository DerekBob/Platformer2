using UnityEngine;

[CreateAssetMenu(fileName = "Character Profile", menuName = "Base Character Profile")]
public class BaseCharacterProfile : ScriptableObject
{
    public string Name = "Mr. Default";
    public MovementAttributes defaultAttributes;
    public CharacterAnimation characterAnimations;
}

//Once this is initialized, it is unmodifiable. But... there will be an override that adds to it and classes will take this + override.
//make a class that holds all additive values, and then we save that class. 
//The real override will be the effects, who can take away.
//So, BaseAttributes + CurrentAttribute class counter + effect modifications later
[System.Serializable]
public class MovementAttributes
{
    public float speed; //also serves as max ground speed
    public float acceleration;
    public float runSpeed;
    public float sneakSpeed;
    public float crouchSpeed;
    public float jumpImpulse;
    public float airAccel;
    public float maxAirSpeed;
    public float jumpHeight;
    public float jumpCoolDown;
}

[System.Serializable]
public class CharacterAnimation
{
    public string idle = "Mannequin Idle";
    public string walking = "Test Walk";
    public string running = "Test Walk";
    public string jumping = "TestJump";
    public string falling = "falling";
    public string landing = "Landing";
    public string crouching = "TestJump";
    //add more as time goes on
}