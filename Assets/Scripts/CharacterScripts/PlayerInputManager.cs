using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    //gotta find a way for the player to be able to map these manually... incase they are weird or something.
    [Header("Movement Keys")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;


    [Header("Special Keys")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sneakKey = KeyCode.C;

    private InputState inputs = new InputState();
    private LookManager cameraControls;
    private ControlState controlState;
    //private MovementController movementController;
    private CharacterMover mover;

    private float mouseX;
    private float mouseY;

    private Vector2 inputDirectionLeft; //left stick or wasd

    void Start()
    {
        //QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        //Application.targetFrameRate = 60;
        cameraControls = this.GetComponent<LookManager>();
        controlState = this.GetComponent<ControlState>();
        mover = this.GetComponent<CharacterMover>();
        

        if (cameraControls == null) Debug.LogWarning("Missing LookManager component.");
        if (controlState == null) Debug.LogWarning("Missing ControlState component.");
        if (mover == null) Debug.LogWarning("Missing CharacterMover component");
        //if (movementController == null) Debug.LogWarning("Missing MovementController component.");

    }

    private void Update()
    {
        float rawX = Input.GetAxis("Horizontal");
        float rawY = Input.GetAxis("Vertical");

        inputDirectionLeft = new Vector2(rawX, rawY);

        if (inputDirectionLeft.sqrMagnitude > 1f)
            inputDirectionLeft.Normalize();
        //The IMovementMode will handle if it will treat the jump as a key down or a continuous press. Not this
        if (!controlState.IsInputLocked())
        {
            inputs.wantsToJump = Input.GetKey(jumpKey);
            inputs.wantsToRun = Input.GetKey(sprintKey);
            inputs.wantsToCrouch = Input.GetKey(crouchKey);
            inputs.wantsToSneak = Input.GetKey(sneakKey);

            inputs.movementInput = inputDirectionLeft;

            mover.SetInputs(inputs);
        }

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //controlState.DisableCamera(ControlPriority.Low, "input");

    }

    private void LateUpdate()
    {
        if (!controlState.IsCameraControlDisabled())
        {
            mouseX = Input.GetAxis("Mouse X") * 5;
            mouseY = Input.GetAxis("Mouse Y") * 5;
            cameraControls.RotateCamera(mouseX, mouseY);

            mover.SetLookDirection(cameraControls.LookDirection());
        }
    }

    public Vector2 GetInput()
    {
        return inputDirectionLeft;
    }
}
