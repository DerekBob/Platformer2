using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Variables depend on Game.
public class MovementIntent : MonoBehaviour
{
    //Wont use events unless I find it a better option to use them,
    //as we want a continuous stream or just a blip of input, so the input manager will have the get key for 'wantsToRun' or use key down for 'wantsToJump'.
    //Or just let the Imovementhandler determine how to register it, as a continuous press or not, keeping it away from input manager

    //Remove Serialize Fields later, we need this for quick debugging

    //trying to speed, not necessarilly run. So faster swimming perhaps... Just log shift or run key here
    [SerializeField]private bool wantsToMoveFast;

    //same deal different name
    [SerializeField] private bool wantsToSneak;

    [SerializeField] private bool wantsToCrouch;

    [SerializeField] private bool wantsToJump;

    //maybe dont add this one
    [SerializeField] private bool wantsToAttack;

    //[SerializeField] private MovementInput inputIntent = new MovementInput();

    [SerializeField] private ControlState controlState;

    //void Awake()
    //{
    //    if (controlState == null)
    //    {
    //        controlState = GetComponent<ControlState>();
    //    }
        

    //}

    ////Input Manager will be calling these
    //public void SetWantsToMoveFast(bool keyPress)
    //{
    //    inputIntent.moveFast = keyPress;
    //}

    //public void SetWantsToSneak(bool keyPress)
    //{
    //    inputIntent.sneak = keyPress;
    //}

    //public void SetWantsToCrouch(bool keyPress)
    //{
    //    inputIntent.crouch = keyPress;
    //}

    //public void SetWantsToJump(bool keyPress)
    //{
    //    inputIntent.jump = keyPress;
    //}

    //public void SetWantsToAttack(bool keyPress)
    //{
    //    inputIntent.attack = keyPress;
    //}

    //public void GetInputState(ref MovementInput input)
    //{
    //    input.moveFast = wantsToMoveFast;
    //    input.sneak = wantsToSneak;
    //    input.crouch = wantsToCrouch;
    //    input.jump = wantsToJump;
    //    input.attack = wantsToAttack;
    //}


    ////private ControlState controlState;
    //void OnEnable()
    //{
    //    controlState.OnMovementLocked += ClearAll;
    //}

    //void OnDisable()
    //{
    //    controlState.OnMovementLocked -= ClearAll;  
    //}

    ////if Movement gets locked Call Clear All
    //public void ClearAll()
    //{
    //    inputIntent.moveFast = false;
    //    inputIntent.sneak = false;
    //    inputIntent.crouch = false;
    //    inputIntent.jump = false;
    //    inputIntent.attack = false;
    //}

}


