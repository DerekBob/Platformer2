using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatCamera : MonoBehaviour, Ilook
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform theCamera;
    [SerializeField] private float followSpeed = 0;
    [SerializeField] private float distance = 20;
    [SerializeField] private bool lookDirectlyAt = false;
    

    void Start()
    {
    }

    void LateUpdate()
    {
        theCamera.transform.position = target.transform.position + new Vector3(0, 0, distance);
    }

    public Transform LookDirection()
    {
        return theCamera;
    }

    public Vector3 LookDirectionWorld()
    {
        return theCamera.TransformDirection(Vector3.forward);
    }

    public void SetTarget(Transform set)
    {
        target = set;
    }

}
