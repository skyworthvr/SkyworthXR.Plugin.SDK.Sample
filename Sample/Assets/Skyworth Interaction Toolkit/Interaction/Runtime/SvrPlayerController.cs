using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SvrPlayerController : MonoBehaviour
{
    public float MoveSpeed = 1;

    private Vector3 m_startPosition;
    private Quaternion m_startRotation;
    // Start is called before the first frame update
    void Start()
    {
        m_startPosition = transform.position;
        m_startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        UpdateTransformEditor();
#else
        UpdateTransformAndroid();
#endif

    }
    private void UpdateTransformEditor() 
    {
        Vector3 forward = Camera.main.transform.forward;
        
        forward.y = 0;
        Debug.Log(forward);
        Vector3 right = Camera.main.transform.right; 
        right.y = 0;
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += forward * Time.deltaTime * MoveSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position -= forward * Time.deltaTime * MoveSpeed;
        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += right * Time.deltaTime * MoveSpeed;
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position -= right * Time.deltaTime * MoveSpeed;
    }
    private void UpdateTransformAndroid() 
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        InputDevice RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        RightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool clickvalue);
        if (RightControllerDevice.isValid && clickvalue && RightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 touchvalue))
        {
            transform.position += touchvalue.y * forward * Time.deltaTime * MoveSpeed;
            transform.position += touchvalue.x * right * Time.deltaTime * MoveSpeed;
        }
        
    }
}
