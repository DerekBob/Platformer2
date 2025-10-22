using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionAnimator : MonoBehaviour
{
    public Animator Anim;

    //maybe make a class that contains all essentials?

    //the one inside the capsule
    public Transform Root;

    //the bone
    public Transform MeshRoot;
    
    void Start()
    {
        if (Anim == null) Debug.LogError("Animator is missing from action animator!");
    }

    //turnspeed in degrees per second
    public void ChangeRootDirection(Vector3 dir, float turnSpeed, float deltaTime)
    {
        Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);

        Root.rotation = Quaternion.RotateTowards(Root.rotation, target, turnSpeed * deltaTime);
    }

    public void CrossFade(string animName, float duration, int layer)
    {
        Anim.CrossFade(animName, duration, layer);
    }

    public Animator GetAnim()
    {
        return Anim;
    }
}
