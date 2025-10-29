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
    public Vector3 Move(CharacterMover mover, InputState input, Transform lookDirection, Vector3 lateVelocity, float deltaTime, EffectTable table)
    {
        //We dont go by input perhaps?
        //straightline dash.

        //we calculate the distance and stop it once it exceeds the distance. Start and final get reset as if the dashed is false, then this is a new dash
        if(mover.dashed == false)
        {
            startPos = mover.transform.position;
            Debug.Log("Startpos: " + startPos);
            finalPos = mover.transform.position;
        }

        //if the dash goes its certain distance
        if (mover.dashed == true)
        {
            finalPos = mover.transform.position;
            currentdist += Vector3.Distance(startPos, finalPos);

            //if (currentdist >= table.GetCurrentAttributes().DashDistance) //if it goes over the dash distance, then we stop
            //{
            //    Vector3 dir = (finalPos - startPos).normalized; //the direction between last position and this position

            //    currentdist -= Vector3.Distance(startPos, finalPos); //reset back to the original distance.
            //    ;
            //    float distanceRemaining = table.GetCurrentAttributes().DashDistance - currentdist; //find the required distance to stop at

            //    //check for any obstruction. make capsule cast later.
            //    RaycastHit hit;
            //    if (Physics.Raycast(startPos, dir, out hit, distanceRemaining, LayerMask.GetMask("Terrain")))
            //    {
            //        mover.GetMotor().MoveCharacter(hit.point); //hit something so end up at hit point
            //        Debug.Log(hit.collider.name);
            //    }
            //    else
            //    {
            //        mover.GetMotor().MoveCharacter(finalPos + dir * distanceRemaining);
            //    }

            //    mover.dashed = false;
            //    currentdist = 0;
            //    return lateVelocity;
            //}
            //else
            //{
            //    startPos = mover.transform.position;
            //}


        }



        //if the dash somehow got stopped
        if (mover.dashed == true && currentdist < table.GetCurrentAttributes().DashDistance && lateVelocity == Vector3.zero)
        {
            mover.dashed = false;
            currentdist = 0;
            //Debug.Log("helo");
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
        Vector3 futurePos = finalPos + velocity * deltaTime;

        Vector3 futuredir = (futurePos - finalPos).normalized;

        float futureDist = currentdist + Vector3.Distance(futurePos, finalPos);

        float distanceRemaining = Mathf.Abs(table.GetCurrentAttributes().DashDistance - futureDist); // Mathf.Clamp(table.GetCurrentAttributes().DashDistance - futureDist, 0, table.GetCurrentAttributes().DashDistance);

        //Debug.DrawRay(mover.transform.position, futurePos - mover.transform.position, Color.green, 10);

        Debug.Log("CurrentDist: " + currentdist);
        Debug.Log("FutureDist: " + futureDist);
        Debug.Log("distRemaining: " + distanceRemaining);

        RaycastHit futureHit;
        if (futureDist >= table.GetCurrentAttributes().DashDistance) //we check if currentdist is 0 as we only calculate the first frame.
        {
            if (Physics.Raycast(mover.transform.position, futuredir, out futureHit, distanceRemaining))
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
                mover.GetMotor().MoveCharacter(futurePos - futuredir * distanceRemaining);
                Vector3 uyeah = futurePos - futuredir * distanceRemaining;

                Debug.Log("final pos: " + uyeah);
            }


            mover.dashed = false;
            currentdist = 0;
            return Vector3.zero;
        }
        else
        {
            //currentdist += futureDist;
            
            startPos = mover.transform.position;
        }

        Debug.Log("WHAT");
        mover.dashed = false;
        currentdist = 0;
        return Vector3.zero; //unreachable technically
    }

    public static Vector3 SphereOrCapsuleCastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }
}
