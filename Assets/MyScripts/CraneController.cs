using GameMath.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    private float turnSpeed = 20.0f; // tunring speed of crane

    public HoldableButton turnLeftButton; // refrence to button for rotating left 
    public HoldableButton turnRightButton; // reference for right button

    void Update()
    {
        // checking button states
        if (turnLeftButton != null && turnLeftButton.IsHeldDown)
        {
            TurnLeft();
        }

        if (turnRightButton != null && turnRightButton.IsHeldDown)
        {
            TurnRight();
        }
    }

    // turning crane
    public void TurnLeft()
    {
        transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);

    }

    public void TurnRight()
    {
        transform.Rotate(Vector3.down * turnSpeed * Time.deltaTime);

    }

}
