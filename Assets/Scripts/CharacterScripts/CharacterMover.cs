using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMover : MonoBehaviour, KinematicCharacterController.ICharacterController
{
    private KinematicCharacterMotor Motor;

    public event Action OnGround;
    public event Action OnAir;
    public event Action OnJump;

    [SerializeField] private bool grounded;

    [SerializeField] private bool crouched;

    [SerializeField] public bool dashed;


    //maybe rename to current movement if another class will be doing the changing?
    [SerializeField] public IMovementModeHandler movement = new CubeMovement();
    [SerializeField] public IMovementModeHandler defaultMove = new CubeMovement();
    [SerializeField] public IMovementModeHandler dashing = new DashMovement();

    private Transform lookDirection;

    private EffectTable effectTable;

    public InputState inputs;

    [SerializeField] private Vector3 lastCalculatedVelocity;

    //private bool landed = false;

    void Start()
    {
        Motor = this.GetComponent<KinematicCharacterMotor>();
        effectTable = this.GetComponent<EffectTable>();
        Motor.CharacterController = this;

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        //throw new System.NotImplementedException();
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        //to stop some errors
        if (lookDirection == null)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        if (inputs.wantsToRun && !dashed)
        {
            //Debug.Log("here");
            movement = dashing;
        }
        else if(!dashed)
        {
            //Debug.Log("here231");
            dashed = false;
            movement = defaultMove;

        }

        currentVelocity = movement.Move(this, inputs, lookDirection, lastCalculatedVelocity, deltaTime, effectTable);

        

        

        //var attrs = effectTable.GetCurrentAttributes();

        //Vector3 up = Motor.CharacterUp;
        //Vector3 right = Vector3.right;
        ////var motor = mover.GetMotor();

        ////maybe you want to make a rotating level? then use the commented thing (NO DO NOT UNCOMMENT THIS TO IMPLEMENT IT! IT MUST STAY COMMENTED UNLESS EXPLICITLY SAID SO)
        ////Vector3 right = lookDirection
        ////    ? Vector3.ProjectOnPlane(lookDirection.right, up).normalized
        ////    : Vector3.right;

        //// Reduce WASD to left/right only: -1, 0, or +1 along 'right'
        //Vector3 inputHoriz = Vector3.ProjectOnPlane(inputs.movementInput, up);

        //if (Motor.GroundingStatus.IsStableOnGround)
        //{
        //    Vector3 velocity = inputHoriz * attrs.speed;

        //    if (inputs.wantsToJump)
        //    {
        //        Motor.ForceUnground();
        //        InvokeJump();

        //        float impulse = MovementMath.JumpToCertainHeight(WorldAttributes.gravity, attrs.jumpHeight);
        //        velocity.y = 0;
        //        velocity += up * impulse - Vector3.Project(velocity, up);
        //    }

        //    currentVelocity = velocity;
        //    //return;
        //}
        //else
        //{
        //    Vector3 velocity = lastCalculatedVelocity;

        //    // Current horizontal scalar along the left/right axis
        //    float v = Vector3.Dot(velocity, right);

        //    // Collapse input to left/right only
        //    float dot = Vector3.Dot(inputHoriz, right);
        //    int dir = 0;
        //    if (dot > 0.0001f) dir = 1;
        //    else if (dot < -0.0001f) dir = -1;

        //    if (dir != 0)
        //    {
        //        float add = dir * attrs.speed;

        //        if (Mathf.Sign(v) == dir && !Mathf.Approximately(v, 0f))
        //        {
        //            // Same direction: don't let input make it faster than current
        //            float mag = Mathf.Max(Mathf.Abs(v), Mathf.Abs(add));
        //            v = mag * dir;
        //        }
        //        else
        //        {
        //            // Opposite direction: subtract to brake
        //            v = v + add;
        //        }
        //    }

        //    // Rebuild velocity from horizontal scalar + vertical
        //    velocity = right * v;

        //    velocity.y = lastCalculatedVelocity.y;

        //    velocity.y -= WorldAttributes.gravity * deltaTime;

        //    //trying to prevent airclimbing sloped walls
        //    if (Motor.GroundingStatus.FoundAnyGround)
        //    {
        //        if ((Motor.GroundingStatus.GroundNormal.x > 0 && velocity.x <= 0) || (Motor.GroundingStatus.GroundNormal.x < 0 && velocity.x >= 0))
        //        {
        //            velocity.x = lastCalculatedVelocity.x;
        //        }

        //        if (velocity.y > 0)
        //        {
        //            velocity.y = 0;
        //        }
        //    }
        //    currentVelocity = velocity;
        //}

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        //lastPos = this.transform.position;
    }

    //also sends an event
    public void PostGroundingUpdate(float deltaTime)
    {
        bool isCurrentlyGrounded = Motor.GroundingStatus.IsStableOnGround;

        if (!grounded && isCurrentlyGrounded)
        {
            OnGround?.Invoke();

        }
        else if (grounded && !isCurrentlyGrounded)
        {
            OnAir?.Invoke();
        }


        grounded = isCurrentlyGrounded; //not so accurate, best if usedin velocity

    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        lastCalculatedVelocity = Motor.Velocity;

    }



    public void SetCrouch(bool crouch)
    {
        crouched = crouch;
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        //throw new System.NotImplementedException();
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
       
        //if(!Motor.GroundingStatus.IsStableOnGround)
        //{
        //    Debug.Log("STOP IGNORING THE GROUND");
        //    Motor.ForceGround();
        //}
        
        //throw new System.NotImplementedException();
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        //throw new System.NotImplementedException();

        //hitStabilityReport.
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        //throw new System.NotImplementedException();
        //vel = Vector3.zero;
    }

    //force unground
    public void UnGround()
    {
        Motor.ForceUnground();
    }

    public KinematicCharacterMotor GetMotor()
    {
        return Motor;
    }

    public Vector3 GetCharacterUp()
    {
        return Motor.CharacterUp;
    }

    public Vector3 GetVelocity()
    {
        return lastCalculatedVelocity;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    public void SetInputs(InputState setInputs)
    {
        inputs = setInputs;
    }

    public void SetLookDirection(Transform looking)
    {
        lookDirection = looking;
    }

    public void InvokeJump()
    {
        OnJump?.Invoke();
    }

    public IMovementModeHandler GetMovementMode()
    {
        return movement;
    }
}
public struct InputState
{
    public bool wantsToJump;
    public bool wantsToRun;
    public bool wantsToSneak;
    public bool wantsToCrouch;
    public bool wantsToAttack;

    public Vector2 movementInput;
}
