using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordInHandTest : MonoBehaviour
{
    public Transform SwordGrip;
    public Transform HoldContainer;
    //and then add the hand grip animation for this specific sword

    // Start is called before the first frame update
    void Start()
    {
        SwordGrip.parent = HoldContainer;
        SwordGrip.position = HoldContainer.position;
        SwordGrip.rotation = HoldContainer.rotation;
    }

}
