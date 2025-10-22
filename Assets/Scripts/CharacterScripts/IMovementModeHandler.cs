using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementModeHandler
{
    public Vector3 Move(CharacterMover mover, InputState input, Transform lookDirection, Vector3 lateVelocity, float deltaTime, EffectTable table);

    public string GetName();
}

public static class MovementMath
{
    //Get Direction based off where camera is looking
    public static Vector3 GetRelativeDirectionFlat(Transform lookDirection, Vector2 inputVector)
    {
        Vector3 desiredDirection = lookDirection.forward * inputVector.y + lookDirection.right * inputVector.x;
        desiredDirection.y = 0;
        desiredDirection.Normalize();
        return desiredDirection;
    }

    public static float JumpToCertainHeight(float gravity, float height)
    {
        float impulse = Mathf.Sqrt(2 * gravity * height);
        return impulse;
    }

    //ChatGPT wrote everything else down below lol.

    /// Impulse: instantaneous change in velocity (J = m * v).
    /// If you already computed v directly, call AddVelocityDelta.
    public static void AddImpulse(ref Vector3 velocity, Vector3 impulse, float mass)
    {
        velocity += impulse / Mathf.Max(mass, 1e-6f);
    }

    /// Impulse specified directly as a velocity delta.
    public static void AddVelocityDelta(ref Vector3 velocity, Vector3 deltaV)
    {
        velocity += deltaV;
    }

    /// Force: acts over time (F = m * a). Semi-implicit Euler integration.
    public static void AddForce(ref Vector3 velocity, Vector3 force, float mass, float deltaTime)
    {
        Vector3 a = force / Mathf.Max(mass, 1e-6f);
        velocity += a * deltaTime;
    }

    /// Uniform gravity over time. 'up' is the character's up direction (e.g., motor.CharacterUp).
    /// Use gravityMagnitude = 9.81f (positive); it is applied opposite to 'up'.
    public static void IntegrateGravity(ref Vector3 velocity, float gravityMagnitude, Vector3 up, float deltaTime)
    {
        velocity += (-gravityMagnitude * up.normalized) * deltaTime;
    }

    // --------------------------------
    // Useful helpers for character use
    // --------------------------------

    /// Jump takeoff speed to reach an apex height 'h' under gravity 'g' (both positive).
    public static float JumpSpeedForHeight(float gravityMagnitude, float height)
    {
        return Mathf.Sqrt(Mathf.Max(0f, 2f * gravityMagnitude * height));
    }

    /// Apply a jump as an impulse that sets the vertical component to a desired takeoff speed,
    /// preserving existing horizontal momentum. 'up' is the up direction.
    public static void ApplyJumpImpulse(ref Vector3 velocity, float desiredTakeoffSpeed, Vector3 up)
    {
        Vector3 vUp = Vector3.Project(velocity, up);
        Vector3 vHoriz = velocity - vUp;
        Vector3 newVUp = up.normalized * desiredTakeoffSpeed;
        velocity = vHoriz + newVUp;
    }

    /// Project a vector onto a plane defined by its normal.
    public static Vector3 ProjectOnPlane(Vector3 v, Vector3 planeNormal)
    {
        return v - Vector3.Project(v, planeNormal);
    }

    /// Accelerate velocity toward a desired direction on a plane (e.g., ground),
    /// respecting a max horizontal speed. Good for “forces-from-input”.
    /// Acceleration is in m/s^2.
    public static void AccelerateOnPlane(
        ref Vector3 velocity,
        Vector3 desiredDir,    // already normalized and flattened onto plane
        float acceleration,    // m/s^2
        float maxSpeed,        // m/s (horizontal limit on the plane)
        Vector3 planeNormal,
        float deltaTime)
    {
        Vector3 vPlane = ProjectOnPlane(velocity, planeNormal);
        Vector3 add = desiredDir * (acceleration * deltaTime);
        vPlane += add;

        // Clamp horizontal (on-plane) speed
        float spd = vPlane.magnitude;
        if (spd > maxSpeed)
            vPlane = vPlane * (maxSpeed / spd);

        // Recompose with off-plane component (e.g., vertical)
        Vector3 vOff = velocity - ProjectOnPlane(velocity, planeNormal);
        velocity = vPlane + vOff;
    }

    /// Exponential drag (viscous), applied only on the plane (nice for ground friction feel).
    /// k is the damping rate (1/s). Use small values like 8–20 for snappy stop.
    public static void ApplyPlanarDrag(ref Vector3 velocity, float k, Vector3 planeNormal, float deltaTime)
    {
        Vector3 vPlane = ProjectOnPlane(velocity, planeNormal);
        Vector3 vOff = velocity - vPlane;

        float decay = Mathf.Exp(-Mathf.Max(0f, k) * deltaTime);
        vPlane *= decay;

        velocity = vPlane + vOff;
    }

    /// Simple terminal velocity clamp along a direction (e.g., downward).
    public static void ClampSpeedAlong(ref Vector3 velocity, Vector3 dir, float maxSpeed)
    {
        Vector3 along = Vector3.Project(velocity, dir);
        float s = along.magnitude;
        if (s > maxSpeed)
            velocity += (dir.normalized * maxSpeed - along);
    }

    // ----------------------------------------
    // Convenience one-liners for common tasks
    // ----------------------------------------

    /// Add a constant force in the character's up/down axis (e.g., wind tube, jump pad hold).
    public static void AddAxialForce(ref Vector3 velocity, float forceAlongUp, float mass, Vector3 up, float deltaTime)
    {
        AddForce(ref velocity, up.normalized * forceAlongUp, mass, deltaTime);
    }

    /// Horizontal input as a force over time (world-space), e.g., for air control.
    public static void AddPlanarForce(ref Vector3 velocity, Vector3 worldForce, Vector3 planeNormal, float mass, float deltaTime)
    {
        Vector3 planarForce = ProjectOnPlane(worldForce, planeNormal);
        AddForce(ref velocity, planarForce, mass, deltaTime);
    }
}
