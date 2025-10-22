using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : IMovementModeHandler
{

    private float currentSpeed = 3f;
    public string GetName()
    {
        return "Ground";
    }

    private Vector3 direction = new Vector3();
    public Vector3 Move(CharacterMover mover, InputState input, Transform lookDirection, Vector3 lateVelocity, float deltaTime, EffectTable table)
    {
        //if falling
        if(!mover.IsGrounded())
        {
            direction = lateVelocity;

            direction.y -= WorldAttributes.gravity * deltaTime;

            return direction;
        }

        float wantedSpeed = table.GetCurrentAttributes().speed;
        if (input.wantsToRun)
        {
            wantedSpeed = table.GetCurrentAttributes().runSpeed;
        }
        else if(input.wantsToSneak)
        {
            wantedSpeed = table.GetCurrentAttributes().sneakSpeed;
        }

        if(input.wantsToCrouch)
        {
            wantedSpeed = table.GetCurrentAttributes().crouchSpeed;
            mover.SetCrouch(true); //maybe dont set the crouch in controller here?
        }
        else
        {
            mover.SetCrouch(false);
        }

        if(input.wantsToCrouch && input.wantsToRun)
        {
            wantedSpeed = table.GetCurrentAttributes().runSpeed - table.GetCurrentAttributes().crouchSpeed;
        }

        if (input.wantsToCrouch && input.wantsToSneak)
        {
            wantedSpeed = table.GetCurrentAttributes().crouchSpeed - table.GetCurrentAttributes().runSpeed;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, wantedSpeed, table.GetCurrentAttributes().acceleration * deltaTime);

        // Desired horizontal direction at the smoothed speed
        direction = MovementMath.GetRelativeDirectionFlat(lookDirection, input.movementInput) * currentSpeed;

        if (input.wantsToJump)
        {
            mover.UnGround();
            mover.InvokeJump();
            
            direction += mover.GetCharacterUp() * table.GetCurrentAttributes().jumpImpulse - Vector3.Project(direction, mover.GetCharacterUp());
            //Debug.Log(direction);
        }

        
        return direction;
    }
}
