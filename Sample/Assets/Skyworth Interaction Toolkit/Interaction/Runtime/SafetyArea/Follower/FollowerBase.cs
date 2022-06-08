using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FollowerBase : MonoBehaviour
{

    public bool StopFollower = false;

    [SerializeField, Range(0.1f, 100.0f)]
    private float windowDistance = 1.5f;
    public float WindowDistance
    {
        get { return windowDistance; }
        set { windowDistance = Mathf.Abs(value); }
    }

    [SerializeField, Range(0.0f, 100.0f), Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
    private float windowFollowSpeed = 2f;

    public float WindowFollowSpeed
    {
        get { return windowFollowSpeed; }
        set { windowFollowSpeed = Mathf.Abs(value); }
    }

    [Header("Window Settings")]
    [SerializeField, Tooltip("What part of the view port to anchor the window to.")]
    protected TextAnchor windowAnchor = TextAnchor.LowerCenter;

    public TextAnchor WindowAnchor
    {
        get { return windowAnchor; }
        set { windowAnchor = value; }
    }

    [SerializeField, Tooltip("The offset from the view port center applied based on the window anchor selection.")]
    protected Vector2 windowOffset = new Vector2(0f, 0f);
    public Vector2 WindowOffset
    {
        get { return windowOffset; }
        set { windowOffset = value; }
    }

    [SerializeField]
    private Vector2 defaultWindowRotation = new Vector2(0f, 0f);
    public Vector2 DefaultWindowRotation
    {
        get { return defaultWindowRotation; }
        set { defaultWindowRotation = value; }
    }

    protected Quaternion windowHorizontalRotation;
    protected Quaternion windowHorizontalRotationInverse;
    protected Quaternion windowVerticalRotation;
    protected Quaternion windowVerticalRotationInverse;

    protected virtual void OnEnable()
    {
        windowHorizontalRotation = Quaternion.AngleAxis(defaultWindowRotation.y, Vector3.right);
        windowHorizontalRotationInverse = Quaternion.Inverse(windowHorizontalRotation);
        windowVerticalRotation = Quaternion.AngleAxis(defaultWindowRotation.x, Vector3.up);
        windowVerticalRotationInverse = Quaternion.Inverse(windowVerticalRotation);
    }

    protected virtual Vector3 CalculateWindowPosition(Transform cameraTransform)
    {

        Vector3 position = cameraTransform.position + (cameraTransform.forward * WindowDistance);
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
        Quaternion rotation = cameraTransform.rotation;

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

    protected virtual float CalculateWindowOffsetX()
    {
        float _offsetX = 0;
        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: _offsetX += -windowOffset.x; break;
            case TextAnchor.UpperCenter: _offsetX += 0; break;
            case TextAnchor.UpperRight: _offsetX += windowOffset.x; break;
            case TextAnchor.MiddleLeft: _offsetX += -windowOffset.x; break;
            case TextAnchor.MiddleRight: _offsetX += windowOffset.x; break;

            case TextAnchor.MiddleCenter: _offsetX += 0; break;

            case TextAnchor.LowerLeft: _offsetX += -windowOffset.x; break;
            case TextAnchor.LowerCenter: _offsetX += 0; break;
            case TextAnchor.LowerRight: _offsetX += windowOffset.x; break;
        }
        return _offsetX / 2;
    }

    protected virtual float CalculateWindowOffsetY()
    {
        float _offsetY = 0;
        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: _offsetY += windowOffset.y; break;
            case TextAnchor.UpperCenter: _offsetY += windowOffset.y; break;
            case TextAnchor.UpperRight: _offsetY += windowOffset.y; break;
            case TextAnchor.MiddleLeft: _offsetY += 0; break;
            case TextAnchor.MiddleRight: _offsetY += 0; break;

            case TextAnchor.MiddleCenter: _offsetY += 0; break;

            case TextAnchor.LowerLeft: _offsetY += -windowOffset.y; break;
            case TextAnchor.LowerCenter: _offsetY += -windowOffset.y; break;
            case TextAnchor.LowerRight: _offsetY += -windowOffset.y; break;
        }
        return _offsetY;
    }



    protected virtual void LateUpdate()
    {
        if (StopFollower == false)
        {
            Follow();
        }
    }

    protected abstract void Follow();

    public virtual void StopFollow()
    {
        StopFollower = false;
    }
}
