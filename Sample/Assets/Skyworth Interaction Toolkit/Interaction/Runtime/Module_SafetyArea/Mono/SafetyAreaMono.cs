using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Skyworth.Interaction.SafetyArea
{
    public class SafetyAreaMono : MonoBehaviour
    {
        public Material safetyPlaneMat;
        public Material safetyPlaneConfirmedMat;
        public Material safetyPlaneHoverMat;

        public Material areaConfirmMat;
        public Material areaNormalMat;

        public GroundHeightUI groundHeightUI;
        public PlayAreaWaitingDrawUI playAreaWaitingDrawUI;
        public PlayAreaNotEnoughUI playAreaNotEnoughUI;
        public PlayAreaOKUI playAreaOKUI;
        public StationaryAreaUI stationaryAreaUI;
        public ConfirmPlayAreaUI confirmPlayAreaUI;
        public SafetyGreyCameraUI safetyGreyCameraUI;

        [HideInInspector]
        public SafetyPlaneMono safetyPlaneMono;
        private GameObject safetyPlaneObject;
        //[HideInInspector]
        //public StationaryAreaMono stationaryAreaMono;
        //private GameObject stationaryAreaObject;
        //[HideInInspector]
        //public PlayAreaMono playAreaMono;
        //private GameObject playAreaObject;

        private GroundHeightStep groundHeightStep;

        public void Init()
        {
            if (groundHeightStep == null)
            {
                groundHeightStep = SafetyAreaManager.Instance.GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight);
            }

            SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.GroundHeight);
        }

        public void Release()
        {

        }

        //创建平面
        public void CreateSafetyPlane()
        {
            if (safetyPlaneMono == null)
            {
                safetyPlaneObject = new GameObject("SafetyPlane");
                safetyPlaneMono = safetyPlaneObject.AddComponent<SafetyPlaneMono>();
                safetyPlaneMono.Init();
            }
        }

        //销毁平面
        public void DestroySafetyPlane()
        {
            if (safetyPlaneObject != null)
            {
                GameObject.Destroy(safetyPlaneObject);
            }
            safetyPlaneObject = null;
            safetyPlaneMono = null;
        }
    }
}