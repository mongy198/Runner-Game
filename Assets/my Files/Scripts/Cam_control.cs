using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_control : MonoBehaviour
{
    [SerializeField]
    Transform look_target;
    void LateUpdate()
    {
        //make camera follow player rotation smoothly
        transform.rotation = Quaternion.Lerp(transform.rotation, look_target.rotation, .2f);
    }
    private void FixedUpdate()
    {
        //follow player postion
        transform.position = Vector3.Lerp(transform.position, look_target.position, .5f);
    }
}
