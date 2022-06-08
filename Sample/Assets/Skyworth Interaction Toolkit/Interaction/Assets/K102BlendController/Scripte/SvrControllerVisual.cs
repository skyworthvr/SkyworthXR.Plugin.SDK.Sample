using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.Skyworth
{
    public enum BatteryLevel
    {
        CriticalLow = 0,
        Low = 1,
        Medium = 2,
        AlmostFull = 3,
        Full = 4,
        Count = 5
    };
    public class SvrControllerVisual : MonoBehaviour
    {

        public XRNode m_node;
        private Animator s_ControllerAnimatior;
        private Material s_Material;
        public Texture2D[] baterryImages;
        // Start is called before the first frame update
        void Start()
        {
            s_ControllerAnimatior = GetComponentInChildren<Animator>();
            s_Material = GetComponentInChildren<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            InputDevice ControllerDevice = InputDevices.GetDeviceAtXRNode(m_node);
            if (!ControllerDevice.isValid) return;

            if (ControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool PrimaryButtonvalue))
            {
                s_ControllerAnimatior.SetFloat("primaryButton", PrimaryButtonvalue ? 1 : 0);
            }
            else
            {
                s_ControllerAnimatior.SetFloat("primaryButton", 0);
            }

            if (ControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonvalue))
            {
                s_ControllerAnimatior.SetFloat("secondaryButton", secondaryButtonvalue ? 1 : 0);
            }
            else
            {
                s_ControllerAnimatior.SetFloat("secondaryButton", 0);
            }

            if (ControllerDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonvalue))
            {
                s_ControllerAnimatior.SetFloat("menuButton", menuButtonvalue ? 1 : 0);
            }
            else
            {
                s_ControllerAnimatior.SetFloat("menuButtonvalue", 0);
            }

            if (ControllerDevice.TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
            {
                s_ControllerAnimatior.SetFloat("grip", gripvalue);
            }
            else
            {
                s_ControllerAnimatior.SetFloat("grip", 0);
            }
            if (ControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggervalue))
            {
                s_ControllerAnimatior.SetFloat("trigger", triggervalue);
            }
            else
            {
                s_ControllerAnimatior.SetFloat("trigger", 0);
            }

            if (ControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisvalue))
            {
                s_ControllerAnimatior.SetFloat("primary2DAxisX", primary2DAxisvalue.x);
                s_ControllerAnimatior.SetFloat("primary2DAxisY", primary2DAxisvalue.y);
            }
            else
            {
                s_ControllerAnimatior.SetFloat("primary2DAxisX", 0);
                s_ControllerAnimatior.SetFloat("primary2DAxisY", 0);
            }

            if (ControllerDevice.TryGetFeatureValue(CommonUsages.batteryLevel, out float batteryvalue))
            {
                
                if (batteryvalue >= 0.8f)
                {
                    s_Material.SetTexture("_TextureSample1", baterryImages[(int)BatteryLevel.Full]);
                }
                else if (batteryvalue > 0.6f)
                {
                    s_Material.SetTexture("_TextureSample1", baterryImages[(int)BatteryLevel.AlmostFull]);
                }
                else if (batteryvalue >= 0.4)
                {
                    s_Material.SetTexture("_TextureSample1", baterryImages[(int)BatteryLevel.Medium]);
                }
                else if (batteryvalue >= 0.2)
                {
                    s_Material.SetTexture("_TextureSample1", baterryImages[(int)BatteryLevel.Low]);
                }
                else
                {
                    s_Material.SetTexture("_TextureSample1", baterryImages[(int)BatteryLevel.CriticalLow]);
                }
            }
            else
            {
                s_Material.SetTexture("_TextureSample1", baterryImages[(int)BatteryLevel.Full]);
            }
        }
    }
}
