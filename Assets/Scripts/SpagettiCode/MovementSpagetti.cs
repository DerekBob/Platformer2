using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MovementSpagetti : MonoBehaviour
{
    public Animator anim;
    public CharacterController controller;
    public Transform Root;

    public Transform Camera;
    public Transform CameraPivot;

    public Transform Head;
    public Transform LookAt;
    public Transform HeadDriver;

    public Transform collarbone;
    public Transform Neck;
    public Transform NeckDriver;

    public Transform HipPos;
    public RigBuilder rigBuild;
    public TwoBoneIKConstraint RightFootMover;
    public TwoBoneIKConstraint LeftFootMover;

    public Transform RightFoot;
    public Transform RightFootTarget;
    public Transform LeftFoot;
    public Transform LeftFootTarget;

    private Vector2 pivotRotate;
    public float cameraSpeed = 15;

    public Vector2 inputVector = new Vector2();

    private Vector3 desiredDirection;

    private float verticalVelocity = 0f;

    //States
    private string currentAnim;
    public string attackLAnim;

    public bool moving;

    public bool holdingWeapon = true;
        public bool attacking;
            public bool followUp; //only if capable of following up an attack;

    private int attackStage = 0; // 0 = nothing, 1 = swing, 2 = recovery, 3 = downward
    private bool queuedFollowUp = false;

    private float followUpTimer = 0f;
    private float followUpWindow = 1.0f; // seconds allowed to chain

    //private bool hasSnappedFoot = false;



    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        rigBuild.Build();
    }

    void Update()
    {
        Look();
        WantedDirection();

        if (!controller.isGrounded)
            verticalVelocity += -9.81f * Time.deltaTime;
        else
            verticalVelocity = 0f;

        Vector3 move = desiredDirection * 2f;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        moving = inputVector != Vector2.zero;

        if (moving)
        {
            if (currentAnim != "Test Walk")
            {
                anim.CrossFade("Test Walk", 0.1f, 0);
                currentAnim = "Test Walk";
            }
        }
        else
        {
            if (currentAnim != "Mannequin Idle")
            {
                anim.CrossFade("Mannequin Idle", 0.2f, 0);
                currentAnim = "Mannequin Idle";
            }
        }

        Attack();

        HoldWeapon();

    }

    private void LateUpdate()
    {
        if (inputVector != Vector2.zero)
        {
            Root.rotation = Quaternion.Slerp(Root.rotation, Quaternion.LookRotation(desiredDirection), Time.deltaTime * 10f);
        }


        //head logic goes here because I am too lazy.
        RaycastHit hit;

        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, 30))
        {
            LookAt.position = hit.point;
        }
        else
        {
            LookAt.position = Camera.transform.position + Camera.transform.forward * 30f;
        }

        //Look at this jank, we need the head to move first to make the neck move correctly,
        //but since the neck moves... then it moves the head to a wrong spot,
        //so we put the head back to the correct position
        HeadMove();
        NeckMove();
        Head.rotation = HeadDriver.rotation;

        //footstuff

        Vector3 rayOrigin = RightFoot.position + Vector3.up * 0.5f;
        Vector3 rayDir = Vector3.down;
        float rayLength = 1f;

        Debug.DrawRay(rayOrigin, rayDir * rayLength, Color.red);

        //if (baseState.normalizedTime >= 1f && !anim.IsInTransition(0) && !hasSnappedFoot)
        //{

        bool hasHitFloor = Physics.Raycast(rayOrigin, rayDir, out RaycastHit rightHit, rayLength);
        
        if(hasHitFloor)
        {
            //put target on floor
            RightFootTarget.transform.position = rightHit.point;
        }
        else
        {
            //just put it at the end of the ray for debugging purposes
            RightFootTarget.transform.position = rayOrigin - (rayDir * rayLength);
        }

        if (hasHitFloor && RightFoot.position.y <= RightFootTarget.position.y)
        {
            RightFootTarget.position = rightHit.point;

            RightFootMover.data.targetPositionWeight = 1f;
            RightFootMover.data.targetRotationWeight = 1f;

            //hasSnappedFoot = true;
        }
        else
        {
            RightFootMover.data.targetPositionWeight = 0f;
            RightFootMover.data.targetRotationWeight = 0f;
            //hasSnappedFoot = false;
        }

        Vector3 lrayOrigin = LeftFoot.position + Vector3.up * 0.5f;
        Vector3 lrayDir = Vector3.down;

        Debug.DrawRay(lrayOrigin, rayDir * rayLength, Color.red);


        bool lhasHitFloor = Physics.Raycast(lrayOrigin, rayDir, out RaycastHit leftHit, rayLength);

        if (lhasHitFloor)
        {
            //put target on floor
            LeftFootTarget.transform.position = leftHit.point;
        }
        else
        {
            //just put it at the end of the ray for debugging purposes
            LeftFootTarget.transform.position = lrayOrigin - (lrayDir * rayLength);
        }

        if (lhasHitFloor && LeftFoot.position.y <= LeftFootTarget.position.y)
        {
            LeftFootTarget.position = leftHit.point;

            LeftFootMover.data.targetPositionWeight = 1f;
            LeftFootMover.data.targetRotationWeight = 1f;

            //hasSnappedFoot = true;
        }
        else
        {
            LeftFootMover.data.targetPositionWeight = 0f;
            LeftFootMover.data.targetRotationWeight = 0f;
            //hasSnappedFoot = false;
        }


    }

    //3rd person mouse look basic
    private void Look()
    {
        pivotRotate.y += Input.GetAxis("Mouse X");
        pivotRotate.x += Input.GetAxis("Mouse Y");

        pivotRotate.x = Mathf.Clamp(pivotRotate.x, -3, 3);

        if (pivotRotate.y >= 24 || pivotRotate.y <= -24)
            pivotRotate.y = 0;

        CameraPivot.eulerAngles = (Vector2)pivotRotate * cameraSpeed;

    }

    //Gives the direction the player wants to go to.
    private void WantedDirection()
    {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        //getting direction
        desiredDirection = CameraPivot.forward * inputVector.y + CameraPivot.right * inputVector.x;
        desiredDirection.y = 0;
        desiredDirection.Normalize();
        Debug.DrawLine(Root.position, Root.position + desiredDirection * 3, Color.green);

    }

    private void Attack()
    {
        AnimatorStateInfo state1 = anim.GetCurrentAnimatorStateInfo(1);

        //I know I can simplify this without hard coding but this is pure spagetti, pls no critizizzim

        if (Input.GetMouseButtonDown(0))
        {

            // If not attacking, start attack chain
            if (!anim.IsInTransition(1) && (state1.IsName("Empty") || (attackStage == 0 && state1.normalizedTime >= 1f)))
            {
                anim.CrossFade("Very Basic One Handed Swing", 0.1f, 1);
                anim.SetLayerWeight(1, 1f);
                attackLAnim = "Very Basic One Handed Swing";
                attackStage = 1;
            }
            // If already attacking, queue follow-up
            else if (attackStage > 0 && !queuedFollowUp)
            {
                queuedFollowUp = true;
            }
        }

        if (attackStage > 0)
        {
            followUpTimer += Time.deltaTime;

            if (followUpTimer > followUpWindow && !queuedFollowUp)
            {
                // No follow up input in time, abort chain
                anim.CrossFade("Empty", 1f, 1);
                attackStage = 0;
                queuedFollowUp = false;
                followUpTimer = 0f;
            }
        }

        if (!anim.IsInTransition(1) && attackStage == 1 && state1.IsName("Very Basic One Handed Swing") && state1.normalizedTime >= 0.6f && queuedFollowUp)
        {
            anim.CrossFade("VeryBasic OneHanded Recovery Swing", 0.1f, 1);
            attackStage = 2;
            queuedFollowUp = false;
            followUpTimer = 0f;
        }


        if (!anim.IsInTransition(1) && attackStage == 2 && state1.IsName("VeryBasic OneHanded Recovery Swing") && state1.normalizedTime >= 0.6f && queuedFollowUp)
        {
            anim.CrossFade("Very Basic Downwards Swing", 0.3f, 1);
            attackStage = 3;
            queuedFollowUp = false;
            followUpTimer = 0f;
        }

        // Final stage ends
        if (!anim.IsInTransition(1) && attackStage == 3 && state1.IsName("Very Basic Downwards Swing") && state1.normalizedTime >= 1f)
        {
            anim.CrossFade("Empty", 1f, 1);

            attackStage = 0;
            queuedFollowUp = false;
            followUpTimer = 0f;
        }
    }

    private void HoldWeapon()
    {
        AnimatorStateInfo state2 = anim.GetCurrentAnimatorStateInfo(2);
        if (holdingWeapon)
        {
            if (!state2.IsName("Maniquin Grab Left"))
            {
                anim.CrossFade("Maniquin Grab Left", 0, 2);
            }

        }
        else
        {
            if (!state2.IsName("Empty"))
            {
                anim.CrossFade("Empty", 0, 2);
            }
        }
    }

    private void NeckMove()
    {
        //// Get the target rotation based on body forward and head's up
        Quaternion targetNeckRotation = Quaternion.LookRotation(collarbone.forward, Head.up);

        //// Smoothly rotate the neck driver toward that orientation
        float neckRotationSpeed = 120f; // adjust for stiffness
        NeckDriver.rotation = Quaternion.RotateTowards(NeckDriver.rotation, targetNeckRotation, neckRotationSpeed * Time.deltaTime);

        // Apply rotation to the neck
        Neck.rotation = NeckDriver.rotation;

    }

    private void HeadMove()
    {
        float rotationSpeed = 360f; // Degrees per second
        //HeadDriver.rotation = Quaternion.RotateTowards(HeadDriver.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //HeadDriver.rotation = Mathf.Clamp(HeadDriver.rotation.y, -15, 15);

        //Head.rotation = HeadDriver.rotation;

        Vector3 forward = Head.forward;
        Vector3 toTarget = (LookAt.position - Head.position).normalized;

        float maxAngle = 60f; // degrees

        float angle = Vector3.Angle(forward, toTarget);

        if (angle < maxAngle)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            HeadDriver.rotation = Quaternion.RotateTowards(HeadDriver.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Head.rotation = HeadDriver.rotation;
    }
}
