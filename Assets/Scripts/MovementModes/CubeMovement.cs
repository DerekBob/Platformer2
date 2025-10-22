using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : IMovementModeHandler
{
    public string GetName()
    {
        return "CubeMovement";
    }

    private int wallTouchCount = 0;
    private bool wasTouchingWallLastFrame = false;
    public Vector3 Move(CharacterMover mover, InputState input, Transform lookDirection, Vector3 lateVelocity, float deltaTime, EffectTable table)
    {
        
        var attrs = table.GetCurrentAttributes();

        Vector3 up = mover.GetCharacterUp();
        Vector3 right = Vector3.right;
        var motor = mover.GetMotor();

        //maybe you want to make a rotating level? then use the commented thing (NO DO NOT UNCOMMENT THIS TO IMPLEMENT IT! IT MUST STAY COMMENTED UNLESS EXPLICITLY SAID SO)
        //Vector3 right = lookDirection
        //    ? Vector3.ProjectOnPlane(lookDirection.right, up).normalized
        //    : Vector3.right;

        // Reduce WASD to left/right only: -1, 0, or +1 along 'right'
        Vector3 inputHoriz = Vector3.ProjectOnPlane(input.movementInput, up);

        if (motor.GroundingStatus.IsStableOnGround)
        {
            wallTouchCount = 0;
            wasTouchingWallLastFrame = false;

            Vector3 velocity = inputHoriz * attrs.speed;

            if (input.wantsToJump)
            {
                
                mover.UnGround();
                mover.InvokeJump();

                float impulse = MovementMath.JumpToCertainHeight(WorldAttributes.gravity, attrs.jumpHeight);
                velocity.y = 0;
                velocity += mover.GetCharacterUp() * impulse - Vector3.Project(velocity, up);
            }
            return velocity;
        }
        else
        {
            Vector3 velocity = lateVelocity;

            // Current horizontal scalar along the left/right axis
            float v = Vector3.Dot(velocity, right);

            // Collapse input to left/right only
            float dot = Vector3.Dot(inputHoriz, right);
            int dir = 0;
            if (dot > 0.0001f) dir = 1;
            else if (dot < -0.0001f) dir = -1;

            if (dir != 0)
            {
                float add = dir * attrs.speed;

                if (Mathf.Sign(v) == dir && !Mathf.Approximately(v, 0f))
                {
                    // Same direction: don't let input make it faster than current
                    float mag = Mathf.Max(Mathf.Abs(v), Mathf.Abs(add));
                    v = mag * dir;
                }
                else
                {
                    // Opposite direction: subtract to brake
                    v = v + add;
                }
            }

            // Rebuild velocity from horizontal scalar + vertical
            velocity = right * v;

            velocity.y = lateVelocity.y;

            velocity.y -= WorldAttributes.gravity * deltaTime;

            //trying to prevent airclimbing sloped walls
            //if(motor.GroundingStatus.FoundAnyGround)
            //{
            //    if(!touchedSlopedWall)
            //    {
            //        touchedSlopedWallSecondTime = false;
            //    }
            //    else
            //    {
            //        touchedSlopedWallSecondTime = true;
            //    }

            //    touchedSlopedWall = true;

            //    //Debug.Log("Normal: "+ motor.GroundingStatus.GroundNormal);
            //    //Debug.Log("Vel: " + velocity);
            //    //Debug.Log("Late Vel: " + lateVelocity);
            //    if ((motor.GroundingStatus.GroundNormal.x > 0 && velocity.x < 0) || (motor.GroundingStatus.GroundNormal.x < 0 && velocity.x > 0))
            //    {
            //        velocity.x = lateVelocity.x;
            //    }

            //    if(velocity.y > 0 && !touchedSlopedWallSecondTime)
            //    {
            //        velocity.y = 0;
            //    }
            //}

            // treat any contacted but unstable ground here as "wall"
            bool isTouchingWallThisFrame = motor.GroundingStatus.FoundAnyGround;

            // count distinct touches (edge-triggered)
            if (isTouchingWallThisFrame && !wasTouchingWallLastFrame)
            {
                wallTouchCount++;
            }
            wasTouchingWallLastFrame = isTouchingWallThisFrame;

            if (isTouchingWallThisFrame)
            {
                if ((motor.GroundingStatus.GroundNormal.x > 0 && velocity.x < 0) ||
                    (motor.GroundingStatus.GroundNormal.x < 0 && velocity.x > 0))
                {
                    velocity.x = lateVelocity.x;
                }

                // kill Y only on the second distinct touch
                if (velocity.y > 0f && wallTouchCount >= 2)
                {
                    velocity.y = 0f;
                }
            }


            return velocity;

        }



        //return Vector3.zero;
    }
}