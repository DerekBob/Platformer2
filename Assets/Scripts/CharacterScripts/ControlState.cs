using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlState : MonoBehaviour
{
    //no more movement (So scripts that move player automatically dont work)
    public event System.Action OnMovementLocked;
    public event System.Action OnMovementUnlocked;
    private ControlPriority moveHoldPriority = 0;
    private string moveOwner = "unknown";
    [SerializeField] private bool movementDisabled = false;

    //no more inputs
    public event System.Action OnInputLocked;
    public event System.Action OnInputUnlocked;
    private ControlPriority inputHoldPriority = 0;
    private string inputOwner = "unknown";
    [SerializeField] private bool isInputLocked = false;

    //CameraControl is no longer available for player, so scripts like Locking On can take over
    private ControlPriority cameraHoldPriority = 0;
    private string cameraOwner = "unknown";
    [SerializeField] private bool isCameraControlDisabled = false;

    private ControlPriority attackHoldPriority = 0;
    private string attackOwner = "unknown";
    [SerializeField] private bool isAttackDisabled = false;

    // ----------------- Movement -----------------

    public bool DisableMovement(ControlPriority priority, string owner)
    {
        if (moveHoldPriority > priority)
            return false;

        moveHoldPriority = priority;
        movementDisabled = true;
        moveOwner = owner;
        OnMovementLocked?.Invoke();
        return true;
    }

    public bool EnableMovement(ControlPriority priority)
    {
        if (moveHoldPriority > priority)
            return false;

        moveHoldPriority = 0;
        movementDisabled = false;
        moveOwner = "";
        OnMovementUnlocked?.Invoke();
        return true;
    }

    // ----------------- Input -----------------

    public bool DisableInputs(ControlPriority priority, string owner = "unknown")
    {
        if (inputHoldPriority > priority)
            return false;

        inputHoldPriority = priority;
        isInputLocked = true;
        inputOwner = owner;
        OnInputLocked?.Invoke();
        return true;
    }

    public bool EnableInputs(ControlPriority priority)
    {
        if (inputHoldPriority > priority)
            return false;

        inputHoldPriority = 0;
        isInputLocked = false;
        inputOwner = "";
        OnInputUnlocked?.Invoke();
        return true;
    }

    // ----------------- Camera -----------------

    public bool DisableCamera(ControlPriority priority, string owner = "unknown")
    {
        if (cameraHoldPriority > priority)
            return false;

        cameraHoldPriority = priority;
        isCameraControlDisabled = true;
        cameraOwner = owner;
        return true;
    }

    public bool EnableCamera(ControlPriority priority)
    {
        if (cameraHoldPriority > priority)
            return false;

        cameraHoldPriority = 0;
        isCameraControlDisabled = false;
        cameraOwner = "";
        return true;
    }

    // ----------------- Attack -----------------

    public bool DisableAttack(ControlPriority priority, string owner = "unknown")
    {
        if (attackHoldPriority > priority)
            return false;

        attackHoldPriority = priority;
        isAttackDisabled = true;
        attackOwner = owner;
        return true;
    }

    public bool EnableAttack(ControlPriority priority)
    {
        if (attackHoldPriority > priority)
            return false;

        attackHoldPriority = 0;
        isAttackDisabled = false;
        attackOwner = "";
        return true;
    }

    // ----------------- Getters -----------------

    public bool MovementDisabled()
    {
        return movementDisabled;
    }

    public bool IsInputLocked()
    {
        return isInputLocked;
    }

    public bool IsCameraControlDisabled()
    {
        return isCameraControlDisabled;
    }

    public bool IsAttackDisabled()
    {
        return isAttackDisabled;
    }

    // ----------------- Debug Info -----------------

    public string MovementOwner => moveOwner;
    public string InputOwner => inputOwner;
    public string CameraOwner => cameraOwner;
    public string AttackOwner => attackOwner;
}

public enum ControlPriority
{
    None = 0, //no one controls it
    Low = 1, //maybe for attack manager
    Medium = 2, //perhaps for status effects
    High = 3, //If you reached this one, then it is most likely trying to control the camera

    Death = 999, //Yeah, this one beats it all
    CutScene = 1000, //Perhaps cutscene plays after death.
    GameWorld = 1001, //The game world will overwrite everything
    God = 1002 //God, the debug console will be using this one. And it will be the fault of the person if their game breaks for PLAYING GOD!!!
}
