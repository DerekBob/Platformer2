using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickCali : MonoBehaviour
{

    public PlayerInputManager player;

    public RectTransform LeftDot;

    // Update is called once per frame
    void Update()
    {
        LeftDot.anchoredPosition = player.GetInput() * 50;
    }
}
