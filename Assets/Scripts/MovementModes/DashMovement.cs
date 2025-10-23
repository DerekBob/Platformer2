using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMovement : IMovementModeHandler
{
    public string GetName()
    {
        return "Dashing";
    }

    Vector3 startPos;
    Vector3 finalPos;
    float currentdist; //not a straightline distance, we get the total commulative distance
    private float forceStopDist = 2; //if it absolutely exceeds its distance due to outragous speed
    public Vector3 Move(CharacterMover mover, InputState input, Transform lookDirection, Vector3 lateVelocity, float deltaTime, EffectTable table)
    {
        //We dont go by input perhaps?
        //straightline dash.

        //we calculate the distance and stop it once it exceeds the distance. Start and final get reset as if the dashed is false, then this is a new dash
        if(mover.dashed == false)
        {
            startPos = mover.transform.position;
            finalPos = mover.transform.position;
        }

        //if the dash goes its certain distance
        if (mover.dashed == true)
        {
            finalPos = mover.transform.position;
            currentdist += Vector3.Distance(startPos, finalPos);
            if (currentdist >= table.GetCurrentAttributes().DashDistance) //if it goes over the dash distance, then we stop
            {
                Vector3 dir = (finalPos - startPos).normalized; //the direction between last position and this position

                currentdist -= Vector3.Distance(startPos, finalPos); //reset back to the original distance.

                float distanceRemaining = table.GetCurrentAttributes().DashDistance - currentdist; //find the required distance to stop at

                //check for any obstruction. make capsule cast later.
                RaycastHit hit;
                if (Physics.Raycast(startPos, dir, out hit, distanceRemaining))
                {
                    mover.GetMotor().MoveCharacter(hit.point); //hit something so end up at hit point
                    Debug.Log(hit.collider.name);
                }
                else
                {
                    mover.GetMotor().MoveCharacter(startPos + dir * distanceRemaining);
                }

                mover.dashed = false;
                currentdist = 0;
                return Vector3.zero;
            }
            else
            {
                startPos = mover.transform.position;
            }
        }



        //if the dash somehow got stopped
        if (mover.dashed == true && currentdist < table.GetCurrentAttributes().DashDistance && lateVelocity == Vector3.zero)
        {
            mover.dashed = false;
            currentdist = 0;
            return Vector3.zero;
        }

        Vector3 desiredDirection = input.movementInput.normalized;
        Vector3 velocity;
        if(mover.dashed == false && desiredDirection == Vector3.zero) //if the dash happened with no input, we dont dash at all.
        {
            velocity = lateVelocity;
            mover.dashed = false;
            currentdist = 0;
        }
        else if (mover.dashed == false && desiredDirection != Vector3.zero) //if the dash is first started out, we dash the desired direction
        {
            velocity = desiredDirection * table.GetCurrentAttributes().DashSpeed;
            mover.dashed = true;
        }
        else //if the dash is ongoing, we keep the velocity.
        {
            velocity = lateVelocity.normalized * table.GetCurrentAttributes().DashSpeed;
        }

        //calculate future position here. if we overshoot
        Vector3 futurePos = mover.transform.position + velocity * deltaTime;

        Vector3 futuredir = (futurePos - startPos).normalized;

        //Debug.DrawRay(mover.transform.position, futurePos - mover.transform.position, Color.green, 10);

        RaycastHit futureHit;
        if (Vector3.Distance(startPos, futurePos) >= table.GetCurrentAttributes().DashDistance && currentdist == 0) //we check if currentdist is 0 as we only calculate the first frame.
        {
            //starts in center of our sphere.
            if (Physics.Raycast(mover.transform.position, futuredir, out futureHit, table.GetCurrentAttributes().DashDistance))
            {
                //Debug.DrawRay(mover.transform.position + new Vector3(0, 0.5f, 0), futureHit.point - (mover.transform.position + new Vector3(0, 0.5f, 0)), Color.green, 10);

                //mover.GetMotor().SetPosition(futureHit.point, false);

                mover.GetMotor().MoveCharacter(futureHit.point);

                //            mover.GetMotor().SetPosition(
                //SphereOrCapsuleCastCenterOnCollision(mover.transform.position + new Vector3(0, 0.5f, 0), futuredir, futureHit.distance)
                //- new Vector3(0, 0.5f, 0),
                //false);

            }
            else
            {
                mover.GetMotor().SetPosition(startPos + futuredir * table.GetCurrentAttributes().DashDistance, false);
            }

            mover.dashed = false;
            currentdist = 0;
            return Vector3.zero;
        }

        return velocity;
    }

    public static Vector3 SphereOrCapsuleCastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }
}
