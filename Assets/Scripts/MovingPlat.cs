using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlat : MonoBehaviour, KinematicCharacterController.IMoverController
{
    public PhysicsMover mover;
    // Start is called before the first frame update
    void Start()
    {
        mover.MoverController = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IMoverController.UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalPosition = this.transform.position + new Vector3(0,0,3)*Time.deltaTime;
        goalRotation = this.transform.rotation;
    }
}
