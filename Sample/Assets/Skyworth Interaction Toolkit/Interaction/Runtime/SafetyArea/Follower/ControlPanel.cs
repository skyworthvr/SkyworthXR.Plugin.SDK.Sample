using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SC.XR.Unity;

public class ControlPanel : MonoBehaviour
{
    //public SCSlider3D WidthSCSlider3D;
    //public SCSlider3D HeightSCSlider3D;
    //public SCSlider3D SpeedSCSlider3D;
    //public SCSlider3D DistanceSCSlider3D;
    //public SCToggleCheckbox3D InstantFollowing;
    //public SCToggleCheckbox3D LinearFollowing;
    //public SCToggleCheckbox3D StopFollower;
    public CameraFollower canvas;
    // Start is called before the first frame update
    void Awake()
    {
        //WidthSCSlider3D?.onValueChanged.AddListener(OnSliderUpdatedWidth);
        //HeightSCSlider3D?.onValueChanged.AddListener(OnSliderUpdatedHeight);
        //SpeedSCSlider3D?.onValueChanged.AddListener(OnSliderUpdatedSpeed);
        //DistanceSCSlider3D?.onValueChanged.AddListener(OnSliderUpdatedDistance);
        //InstantFollowing.onValueChanged.AddListener(CheckboxInstantFollowing);
        //LinearFollowing.onValueChanged.AddListener(CheckboxLinearFollowing);
        //StopFollower.onValueChanged.AddListener(CheckboxStopFollower);
    }
    private void Start()
    {
        
    }
    public void OnSliderUpdatedWidth(float value)
    {
        canvas.menu_size.x = value;
    }

    public void OnSliderUpdatedHeight(float value)
    {
        canvas.menu_size.y = value;
    }
    public void OnSliderUpdatedSpeed(float value)
    {
        canvas.WindowFollowSpeed = value;
    }

    public void OnSliderUpdatedDistance(float value)
    {
        canvas.WindowDistance = value;
    }
    public void CheckboxInstantFollowing(bool isOn)
    {
        canvas.InstantFollowing = isOn;
    }
    public void CheckboxLinearFollowing(bool isOn)
    {
        canvas.LinearFollowing = isOn;
    }
    public void CheckboxStopFollower(bool isOn)
    {
        canvas.StopFollower = isOn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
