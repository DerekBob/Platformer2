using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizPlat : MonoBehaviour, KinematicCharacterController.IMoverController
{
    public PhysicsMover mover;
    public float amplitude = 2f;
    public float frequency = 1f;

    private Vector3 startPos;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        startPos = this.transform.position;
        mover.MoverController = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void IMoverController.UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        time += deltaTime;
        float offsetX = Mathf.Sin(time * frequency * Mathf.PI * 2f) * amplitude;
        goalPosition = startPos + new Vector3(offsetX, 0f, 0f);
        goalRotation = transform.rotation;
    }
}
