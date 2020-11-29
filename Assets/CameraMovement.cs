using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float CameraDepth = -10;

    [SerializeField]
    private float smoothTime = 0.2f;

    public Transform ObjectToFollow { get; set; } = null;

    private Vector3 smoothVelocity;

    void LateUpdate()
    {
        if (ObjectToFollow != null)
        {
            this.transform.position = Vector3.SmoothDamp(transform.position, new Vector3(ObjectToFollow.position.x, ObjectToFollow.position.y, CameraDepth), ref smoothVelocity, smoothTime);
        }
    }
}
