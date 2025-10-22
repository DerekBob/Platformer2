using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerInputs : MonoBehaviour
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
    private Ilook cameraScript;
    private ControlState controlState;
    //private MovementController movementController;
    private CharacterMover mover;

    private Vector2 inputDirectionLeft; //left stick or wasd

    void Start()
    {
        //QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        //Application.targetFrameRate = 60;
        cameraScript = this.GetComponent<Ilook>();
        controlState = this.GetComponent<ControlState>();
        //movementController = this.GetComponent<MovementController>();
        mover = this.GetComponent<CharacterMover>();


        if (cameraScript == null) Debug.LogWarning("Missing Camera component.");
        if (controlState == null) Debug.LogWarning("Missing ControlState component.");
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
        //this is a little sketch not gonna lie, should the camera script do it on its own instead of this?
        if (!controlState.IsCameraControlDisabled())
        {
            mover.SetLookDirection(cameraScript.LookDirection());
        }
    }

    public Vector2 GetInput()
    {
        return inputDirectionLeft;
    }
}
