using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    //The input on the stick/wasd
    //[SerializeField] private Vector2 inputVector;
    //Direction the player is looking at
    [SerializeField] private Transform lookDirection;

    //Direction Moved.. maybe remove this? or perhaps keep it?
    private Vector3 directionMoved = new Vector3();

    //set to the default walk mode on start... once mode gets made
    private IMovementModeHandler moveMode = new GroundMovement();

    private CharacterMover Mover;
    private Vector3 currentVelocity = new Vector3();

    private EffectTable table;

    //try
    private InputState inputs;

    // Start is called before the first frame update
    void Start()
    {
        Mover = this.GetComponent<CharacterMover>();

        table = this.GetComponent<EffectTable>();

        if (Mover == null) Debug.LogError("Movement Controller is missing CharacterMover");
        if (table == null) Debug.LogError("Effect Table is missing!");
    }

    // Update is called once per frame... and called after Mode detector... for now
    public void FixedUpdate()
    {
        if (lookDirection == null)
            return;
        currentVelocity = Mover.GetVelocity();
        //directionMoved = moveMode.Move(Mover, inputs, lookDirection, currentVelocity, table);

        //and then mover gets direction on its own.
    }

    private void Update()
    {

    }

    //private void LateUpdate()
    //{
    //    lateVelocity = controller.GetVelocity();
    //}

    public void SetInputs(InputState setInputs)
    {
        inputs = setInputs;
    }

    public void SetLookDirection(Transform look)
    {
        lookDirection = look;
    }

    public void SetMoveMode(IMovementModeHandler newMode)
    {
        moveMode = newMode;
    }

    public Vector3 GetDirectionMoved()
    {
        return directionMoved;
    }

    public string GetMode()
    {
        return moveMode.GetName();
    }
}
