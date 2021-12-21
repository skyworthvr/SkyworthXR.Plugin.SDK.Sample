using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

public class SvrPlayerEditor
{
    [MenuItem("Skyworth Interaction Tools/XR Gameobject/Player", false, 10)]
    static void GeneratePlayer()
    {
        GameObject player = ObjectFactory.CreateGameObject("Player");
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.transform.localScale = Vector3.one;

        Camera mainCamera = Camera.main;
        bool isHaveGvrReticlePointer = false;
        bool isHaveTrackedPoseDriver = false;
        bool isHaveGvrPointerPhysicsRaycaster = false;
        if (mainCamera == null)
        {
            GameObject camera = new GameObject("MainCamera", typeof(Camera), typeof(FlareLayer), typeof(AudioListener), typeof(TrackedPoseDriver),typeof(GvrPointerPhysicsRaycaster));
            camera.transform.tag = "MainCamera";
            camera.transform.parent = player.transform;
            camera.transform.position = Vector3.zero;
            camera.transform.rotation = Quaternion.identity;
            camera.transform.localScale = Vector3.one;
            mainCamera = camera.GetComponent<Camera>();
            isHaveGvrPointerPhysicsRaycaster = true;
            isHaveTrackedPoseDriver = true;
        }
        else
        {
            mainCamera.transform.parent = player.transform;
            mainCamera.transform.position = Vector3.zero;
            mainCamera.transform.rotation = Quaternion.identity;
            mainCamera.transform.localScale = Vector3.one;
            isHaveGvrReticlePointer = mainCamera.GetComponentInChildren<GvrReticlePointer>() != null;
            isHaveTrackedPoseDriver = mainCamera.GetComponent<TrackedPoseDriver>() != null;
            isHaveGvrPointerPhysicsRaycaster = mainCamera.GetComponent<GvrPointerPhysicsRaycaster>() != null;
        }
        if (!isHaveGvrReticlePointer)
        {
            GameObject GvrReticlePointer = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/GvrReticlePointer"), mainCamera.transform, false);
            GvrReticlePointer.name = "GvrReticlePointer";
        }
        if (!isHaveTrackedPoseDriver)
        {
            mainCamera.gameObject.AddComponent<TrackedPoseDriver>();
        }
        if (!isHaveGvrPointerPhysicsRaycaster)
        {
            mainCamera.gameObject.AddComponent<GvrPointerPhysicsRaycaster>();
        }
        GameObject eventsystem = new GameObject("GvrEventSystem", typeof(EventSystem), typeof(GvrPointerInputModule));
        eventsystem.transform.parent = player.transform;
        eventsystem.transform.position = Vector3.zero;
        eventsystem.transform.rotation = Quaternion.identity;
        eventsystem.transform.localScale = Vector3.one;

        GameObject GvrControllerMain = new GameObject("GvrControllerMain", typeof(GvrControllerInput));
        GvrControllerMain.transform.parent = player.transform;
        GvrControllerMain.transform.position = Vector3.zero;
        GvrControllerMain.transform.rotation = Quaternion.identity;
        GvrControllerMain.transform.localScale = Vector3.one;

        GameObject GvrEditorEmulator = new GameObject("GvrEditorEmulator", typeof(GvrEditorEmulator));
        GvrEditorEmulator.transform.parent = player.transform;
        GvrEditorEmulator.transform.position = Vector3.zero;
        GvrEditorEmulator.transform.rotation = Quaternion.identity;
        GvrEditorEmulator.transform.localScale = Vector3.one;

        GameObject LeftHand = new GameObject("LeftHand Controllerr", typeof(TrackedPoseDriver));
        LeftHand.transform.parent = player.transform;
        LeftHand.transform.position = Vector3.zero;
        LeftHand.transform.rotation = Quaternion.identity;
        LeftHand.transform.localScale = Vector3.one;
        var LeftDriver = LeftHand.GetComponent<TrackedPoseDriver>();
        LeftDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);

        GameObject NoloLeftController = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/NoloLeftController"), LeftHand.transform, false);
        NoloLeftController.name = "NoloLeftController";


        GameObject RightHand = new GameObject("RightHand Controller", typeof(TrackedPoseDriver));
        RightHand.transform.parent = player.transform;
        RightHand.transform.position = Vector3.zero;
        RightHand.transform.rotation = Quaternion.identity;
        RightHand.transform.localScale = Vector3.one;
        var RightDriver = RightHand.GetComponent<TrackedPoseDriver>();
        RightDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
        GameObject NoloRightController = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/NoloRightController"), RightHand.transform, false);
        NoloRightController.name = "NoloRightController";
        GameObject SvrControllerPointer = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/SvrControllerPointer"), RightHand.transform, false);
        SvrControllerPointer.name = "SvrControllerPointer";
        
        Selection.activeGameObject = player;
    }

    [MenuItem("Skyworth Interaction Tools/XR Gameobject/UI Canvas", false, 10)]
    static void GenerateCanvas()
    {
        GameObject canvas = ObjectFactory.CreateGameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GvrPointerGraphicRaycaster));
        canvas.transform.position = new Vector3(0, 0, 2);
        canvas.transform.rotation = Quaternion.identity;
        canvas.transform.localScale = Vector3.one * 0.002f;

        Selection.activeGameObject = canvas.gameObject;
    }
}
