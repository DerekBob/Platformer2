using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterMover))]
[RequireComponent(typeof(ActionAnimator))]
[RequireComponent(typeof(LookManager))]
[RequireComponent(typeof(EffectTable))]
public class MovementAnimator : MonoBehaviour
{
    private CharacterMover mover;
    private ActionAnimator actionAnim;
    private LookManager looking;
    private EffectTable table;

    public string currentAnim = null;

    public bool jump;

    private AnimatorStateInfo baseState;
    void Start()
    {
        mover = this.GetComponent<CharacterMover>();
        actionAnim = this.GetComponent<ActionAnimator>();
        looking = this.GetComponent<LookManager>();
        table = this.GetComponent<EffectTable>();

        //mover.OnAir += JumpAction;
        mover.OnGround += LandAction;
        mover.OnJump += JumpAction;
        //baseState = anim.GetAnim().GetCurrentAnimatorStateInfo(0);

        
    }

    void LateUpdate()
    {
        baseState = actionAnim.GetAnim().GetCurrentAnimatorStateInfo(0);

        //if grounded obviously //do change logic when attacking to always be looking camera direction maybe? //Also, maybe dont hardcode the string in here, and just pass it down
        if (mover.GetMovementMode().GetName() == "Ground")
        {

            if (mover.GetVelocity() != Vector3.zero && mover.IsGrounded())
            {
                Vector3 flatten = looking.LookDirection().forward;
                flatten.y = 0;
                actionAnim.ChangeRootDirection(flatten, 360, Time.deltaTime);
            }
            
            //YANDERE DEV CORE
            //table can change... and using a gigantic if else thing fucking sucks. I WANT TO USE CASE SWITCHs. I know I can make a tree but I am too lazy too code one.
            if(currentAnim == table.GetCharacterAnimations().idle)
            {
                //transition case 1
                if (jump)
                {
                    TransitionTo(table.GetCharacterAnimations().jumping, 0.2f, 0);
                    return;
                }

                //transition case 2
                if (mover.GetVelocity() != Vector3.zero && mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().walking, 0.2f, 0);
                    return;
                }

                //transition case 3
                if (!mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().falling, 0.2f, 0);
                    return;
                }

                return;
            }
            else if (currentAnim == table.GetCharacterAnimations().walking)
            {
                //transition case 1
                if (jump)
                {
                    TransitionTo(table.GetCharacterAnimations().jumping, 0.2f, 0);
                    return;
                }

                //transition case 2
                if (!mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().falling, 0.2f, 0);
                    return;
                }

                //transition case 3
                if (mover.GetVelocity() == Vector3.zero && mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().idle, 0.2f, 0);
                    return;
                }

                
                return;
            }
            else if(currentAnim == table.GetCharacterAnimations().jumping)
            {
                //transition case 1
                if (mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().landing, 0.2f, 0);
                    return;
                }

                //transition case 2
                //else if finish this one
                if (!mover.IsGrounded() && baseState.normalizedTime >= 1)
                {
                    TransitionTo(table.GetCharacterAnimations().falling, 0.2f, 0);
                    return;
                }
                return;
            }
            else if (currentAnim == table.GetCharacterAnimations().landing)
            {
                if (mover.IsGrounded() && mover.GetVelocity() != Vector3.zero)
                {
                    TransitionTo(table.GetCharacterAnimations().walking, 0.2f, 0);
                    return;
                }

                if (mover.IsGrounded() && mover.GetVelocity() == Vector3.zero)
                {
                    TransitionTo(table.GetCharacterAnimations().idle, 0.4f, 0);
                    return;
                }

                if (!mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().falling, 0.2f, 0);
                    return;
                }
                return;
            }
            else if (currentAnim == table.GetCharacterAnimations().falling)
            {
                if (mover.IsGrounded())
                {
                    TransitionTo(table.GetCharacterAnimations().landing, 0.2f, 0);
                    return;
                }
                return;
            }
            else
            {
                currentAnim = table.GetCharacterAnimations().idle;
            }

            //if all else fails (which it can as we can hot swap what animations we want to use) then set to idle... will last one frame tops)
            
        }

    }

    private void OnDisable()
    {
        mover.OnGround -= LandAction;
        mover.OnJump -= JumpAction;
    }

    public void JumpAction()
    {
        jump = true;
    }

    public void LandAction()
    {
        jump = false;
    }

    private void TransitionTo(string target, float duration, int layer = 0)
    {
        if (currentAnim == target)
            return;

        // Optional safety: don't reissue during an ongoing transition.
        var anim = actionAnim.GetAnim();
        if (anim != null && anim.IsInTransition(layer))
            return;
            
        actionAnim.CrossFade(target, duration, layer);
        currentAnim = target;
    }
}