using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyAreaFollower : FollowerBase
{

    bool isInit = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        isInit = false;
    }

    protected override void Follow()
    {

        if (isInit == false)
        {
            transform.position = CalculateWindowPosition(Camera.main.transform);
            transform.rotation = CalculateWindowRotation(Camera.main.transform);
            isInit = true;
        }

        transform.position = Vector3.Lerp(transform.position, CalculateWindowPosition(Camera.main.transform), WindowFollowSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, CalculateWindowRotation(Camera.main.transform), WindowFollowSpeed * Time.deltaTime);
    }

    protected override Vector3 CalculateWindowPosition(Transform cameraTransform)
    {

        Vector3 position = cameraTransform.position + (new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z) * WindowDistance);
        Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
        Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
            case TextAnchor.UpperCenter: position += verticalOffset; break;
            case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
            case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
            case TextAnchor.MiddleRight: position += horizontalOffset; break;

            case TextAnchor.MiddleCenter: position += horizontalOffset + verticalOffset; break;

            case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
            case TextAnchor.LowerCenter: position -= verticalOffset; break;
            case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
        }

        return position;
    }

    protected virtual Quaternion CalculateWindowRotation(Transform cameraTransform)
    {
        Quaternion rotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);

        switch (windowAnchor)
        {

            case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
            case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
            case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
            case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
            case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
            case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
            case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
            case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
        }

        return rotation;
    }
}
