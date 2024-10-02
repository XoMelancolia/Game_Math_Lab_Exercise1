using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrolleyController : MonoBehaviour
{
    public Transform trolleyNearLimit; // near limit position
    public Transform trolleyFarLimit;   // far limit position
    public Slider trolleySlider;         // reference to trolley slider
    public SoftParenting softParenting; // reference to soft parenting

    private void OnEnable()
    {
        // adding listener to trolley slider changes
        trolleySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // moving trolley when slider value changes
    private void OnSliderValueChanged(float value)
    {
        MoveTrolley(value);
    }

    private void MoveTrolley(float t)
    {
        // changed trolley position
        Vector3 newPosition = Vector3.Lerp(trolleyNearLimit.position, trolleyFarLimit.position, t);

        // chaning soft parenting relative position
        if (softParenting != null)
        {
            softParenting.UpdateRelativePosition(newPosition);
        }
    }

}