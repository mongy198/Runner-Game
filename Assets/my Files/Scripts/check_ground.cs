using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check_ground : MonoBehaviour
{
    public bool grounded = true;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            grounded = false;
        }
    }
}
