using System.Collections;
using System.Collections.Generic;
using Unity.XR.SDK;
using UnityEngine;
using UnityEngine.XR;

public class UISample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GvrControllerInput.GvrPointerEnable = true;
        }
    }

    public void Recenter()
    {
        StartCoroutine(RecenterIE());
    }

    public void RecenterXYZ()
    {
        StartCoroutine(RecenterXYZIE());
    }
    public void DisableController()
    {
        GvrControllerInput.GvrPointerEnable = false;
    }
    private IEnumerator RecenterIE()
    {
        yield return new WaitForSeconds(3);
        var inputsystem = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(inputsystem);
        for (int i = 0; i < inputsystem.Count; i++)
        {
            inputsystem[i].TryRecenter();
        }
    }
    private IEnumerator RecenterXYZIE()
    {
        yield return new WaitForSeconds(3);
        SkyworthPerformance.RecentXYZ();
    }
    Quaternion Multiply(Quaternion lhs, Quaternion rhs)
    {
        Quaternion result;
        result.x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y;
        result.y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z;
        result.z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x;
        result.w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z;
        return result;
    }
    [ContextMenu("EditorRecenter")]
    public void EditorRecenter()
    {
        //transform.rotation = Quaternion.Inverse(transform.rotation) * transform.rotation;

        Quaternion to = transform.rotation;
        Quaternion adjust = Quaternion.identity;
        adjust.x = -to.x;
        adjust.y = -to.y;
        adjust.z = -to.z;
        adjust.w = to.w;
        transform.rotation = Multiply(adjust, to);
    }
}
