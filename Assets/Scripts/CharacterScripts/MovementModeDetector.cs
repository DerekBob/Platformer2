using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//change this this to work with Character Mover instead
[RequireComponent(typeof(ControlState))]
public class MovementModeDetector : MonoBehaviour
{
    private CharacterMover mover;
    private ControlState controlState;
    //private MovementController movementController;

    private IMovementModeHandler ground = new GroundMovement();
    //private IMovementModeHandler falling = new FallingMovement();

    //remove this bool sometime
    //private bool grounded = true;
    private void Start()
    {
        mover = this.GetComponent<CharacterMover>();
        controlState = this.GetComponent<ControlState>();
        //movementController = this.GetComponent<MovementController>();
    }

    private void FixedUpdate()
    {
        if (!controlState.MovementDisabled())
        {
            //remove all this doohicky later
            //bool isCurrentlyGrounded = mover.IsGrounded();
            ////Debug.Log(isCurrentlyGrounded + "currently");
            ////Debug.Log(grounded);
            //if (isCurrentlyGrounded && !grounded)
            //{
            //    Vector3 please = new Vector3();
            //    mover.UpdateVelocity(ref please, Time.deltaTime);
            //    please = Vector3.zero;

            //    movementController.SetMoveMode(ground);
            //}
            //else if(!isCurrentlyGrounded && grounded)
            //{
            //    //Debug.Log("yes");
            //    movementController.SetMoveMode(falling);
            //}
            //grounded = isCurrentlyGrounded;

            
        }

    }
}
