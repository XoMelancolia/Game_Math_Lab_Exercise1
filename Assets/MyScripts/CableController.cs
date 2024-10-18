using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CableController : MonoBehaviour
{
    public Slider cableSlider;     // reference to ui cable slider

    public float minCableLength = 0.05f; // cable minimum scale
    public float maxCableLength = 2.49f; // cable maximum scale

    private void OnEnable()
    {
        // listener for the cable slider changes
        cableSlider.onValueChanged.AddListener(OnCableSliderValueChanged);
    }

    // scale cable when slider is moved
    private void OnCableSliderValueChanged(float value)
    {
        ScaleCable(value);
    }

    public void ScaleCable(float t)
    {
        // calculates cable length based on cable slider value
        float newCableLength = Mathf.Lerp(minCableLength, maxCableLength, t);

        // set cable lenght
        transform.localScale = new Vector3(transform.localScale.x, newCableLength, transform.localScale.z);

    }

}