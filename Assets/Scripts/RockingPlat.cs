using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockingPlat : MonoBehaviour, KinematicCharacterController.IMoverController
{
    public PhysicsMover mover;
    public float amplitude = 2f;
    public float frequency = 1f;
    public float rotationAmplitude = 15f;
    public float rotationFrequency = 1f;

    private Vector3 startPos;
    private Quaternion startRot;
    private float time;

    void Start()
    {
        mover.MoverController = this;
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void IMoverController.UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        time += deltaTime;

        float offsetX = Mathf.Sin(time * frequency * Mathf.PI * 2f) * amplitude;
        goalPosition = startPos + new Vector3(offsetX, 0f, 0f);

        float zRot = Mathf.Sin(time * rotationFrequency * Mathf.PI * 2f) * rotationAmplitude;
        goalRotation = startRot * Quaternion.Euler(0f, 0f, zRot);
    }
}
