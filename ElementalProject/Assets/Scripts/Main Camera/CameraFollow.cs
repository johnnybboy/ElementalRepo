using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    public Transform cameraTarget;
    public float smoothing = 1f;

    private Transform player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTarget = player;
        offset = new Vector3(0, 0, -10f);
        Follow(cameraTarget);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Follow(cameraTarget);
    }

    private void Follow(Transform targetTransform)
    {
        Vector3 targetPosition = targetTransform.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }

    //CameraFollow.instance.SetTarget(Transform t);
    public void SetTarget(Transform targetTransform)
    {
        cameraTarget = targetTransform;
    }

    //CameraFollow.instance.GetTarget();
    public Transform GetTarget()
    {
        return cameraTarget;
    }
}
