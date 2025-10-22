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


    //maybe rename to current movement if another class will be doing the changing?
    [SerializeField] public IMovementModeHandler movement = new CubeMovement();

    private Transform lookDirection;

    private EffectTable effectTable;

    public InputState inputs;

    [SerializeField] private Vector3 vel;

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
            
        currentVelocity = movement.Move(this, inputs, lookDirection, vel, deltaTime, effectTable);
        //Debug.Log(Motor.GroundingStatus.FoundAnyGround);
        //grounded = Motor.GroundingStatus.FoundAnyGround;

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
        vel = Motor.Velocity;
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
        //throw new System.NotImplementedException();
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        //throw new System.NotImplementedException();
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
        return vel;
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
