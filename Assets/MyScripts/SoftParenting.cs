using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftParenting : MonoBehaviour
{
    public Transform parent;  // object to parent to
    private Vector3 relativePosition;
    private Quaternion relativeRotation;

    public void Awake()
    {
        // relative position and rotation to the parent
        relativePosition = parent.InverseTransformPoint(transform.position);
        relativeRotation = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }

    void Update()
    {
        // change position and rotation 
        transform.position = parent.TransformPoint(relativePosition);
        transform.rotation = parent.rotation * relativeRotation;
    }

    public void UpdateRelativePosition(Vector3 newPosition)
    {
        // chamnge position to new position
        relativePosition = parent.InverseTransformPoint(newPosition);
    }

}
