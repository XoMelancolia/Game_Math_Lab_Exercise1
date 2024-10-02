using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // check collision with concrete
        if (other.CompareTag("Concrete")) // checkingg concrete tag
        {
            // concrete soft parenting 
            SoftParenting softParenting = other.GetComponent<SoftParenting>();
            if (softParenting != null)
            {
                // activate soft parenting
                softParenting.enabled = true;

                // attach concrete to hook
                softParenting.parent = transform; // parenting from hook
                softParenting.Awake(); // set concretes relative position and rotation
            }
        }
    }

}

