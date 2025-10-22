using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//set up the camera procedurally instead of drag and drop
public class LookManager : MonoBehaviour, Ilook
{
    //Either drag and drop the pivot and camera, or just plop it in through scripts.
    [SerializeField] private Transform pivot;
    private float yaw;
    private float pitch;

    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 75f;

    [SerializeField] private Transform theCamera;

    public void RotateCamera(float mouseX, float mouseY)
    {
        yaw += mouseX;
        pitch -= mouseY;

        //in case the player is a whale and plays for millions of hours
        yaw = Mathf.Repeat(yaw, 360f);

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public Transform LookDirection()
    {
        return theCamera;
    }

    public Vector3 LookDirectionWorld()
    {
        return theCamera.TransformDirection(Vector3.forward);
    }
}
