using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skyworth.Interaction.SafetyArea
{
    public class StationaryAreaMono : SafetyAreaBase
    {
        private bool isFreeze = true;
        private StationaryAreaStep stationaryAreaStep;
        private GroundHeightStep groundHeightStep;
        private float radius;

        public override void Update()
        {
            base.Update();

            if (!isFreeze)
            {
                Vector3 headPosition = Camera.main.transform.position;
                stationaryAreaStep.SetCircleCenter(headPosition);
                Vector2 circleCenter = stationaryAreaStep.GetCircleCenter();
                this.gameObject.transform.position = new Vector3(circleCenter.x, 0f, circleCenter.y);
            }

            List<Vector4> positionList = GetInteractionObjectPosition();
            //Debug.Log(positionList.Count);
            if (positionList.Count != 0)
            {
                meshRenderer.sharedMaterial.SetInt("positionCount", positionList.Count);
                meshRenderer.sharedMaterial.SetVectorArray("interactionPosition", positionList);
            }

            distance = GetDistance(Camera.main.transform.position);
            alpha = Mathf.Max(0, (distance - radius * 0.5f) * SafetyAreaManager.Instance.AlphaParam);
            meshRenderer.sharedMaterial.SetFloat("alpha", alpha);

            //var lerp = Mathf.PingPong(Time.time, duration) / duration;
            Color colorStart = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].startColor;
            Color colorEnd = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].endColor;
            meshRenderer.sharedMaterial.SetColor("_Color1", colorStart);
            meshRenderer.sharedMaterial.SetColor("_Color2", colorEnd);

            var lerp = Mathf.PingPong(Time.time, duration) / duration;
            meshRenderer.sharedMaterial.SetColor("settingsColor", Color.Lerp(nearColorStart, nearColorEnd, lerp));
            meshRenderer.sharedMaterial.SetVector("_MainTex_ST", new Vector4((float)perimeter / PlayAreaConstant.SAFETY_AREA_HEIGHT * 7f, 7f, 0f, -0.05f));

            if (outOfSafetyArea == null)
            {
                GameObject outOfSafetyAreaResource = Resources.Load<GameObject>("OutofSafetyArea");
                outOfSafetyArea = GameObject.Instantiate(outOfSafetyAreaResource, this.transform);
            }
            meshRenderer.sharedMaterial.SetInt("bowHead", 1);
            if (SafetyAreaManager.Instance.ShowAreaWhenBowHead)
            {
                if (Vector3.Angle(Camera.main.transform.forward, Vector3.down) < PlayAreaConstant.HEAD_BOW_ANGLE)
                {
                    meshRenderer.sharedMaterial.SetInt("bowHead", 1);
                }
                else
                {
                    meshRenderer.sharedMaterial.SetInt("bowHead", 0);
                }
            }
            else
            {
                meshRenderer.sharedMaterial.SetInt("bowHead", 0);
            }

            //if (!SafetyAreaManager.Instance.isSettingSafetyArea && !SafetyAreaManager.Instance.isDisableSafetyArea)
            //{
            //    int currentRelocState = API_GSXR_Slam.GSXR_Get_OfflineMapRelocState();

            //    if (currentRelocState == 0)
            //    {
            //        nomapUI.SetActive(true);
            //        meshRenderer.enabled = false;
            //    }
            //    else
            //    {
            //        if (distance > 5f)
            //        {
            //            slamLostUI.gameObject.SetActive(true);
            //            meshRenderer.enabled = false;
            //        }
            //        else
            //        {
            //            slamLostUI.gameObject.SetActive(false);
            //            meshRenderer.enabled = true;
            //            if (alpha >= 10f)
            //            {
            //                outOfSafetyArea.SetActive(true);
            //            }
            //            else
            //            {
            //                outOfSafetyArea.SetActive(false);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    outOfSafetyArea.SetActive(false);
            //    nomapUI.SetActive(false);
            //    slamLostUI.gameObject.SetActive(false);
            //    meshRenderer.enabled = true;
            //}

            if (existSafetyAreaStateMachine != null)
            {
                existSafetyAreaStateMachine.Breathe();
            }
        }

        public void FreezeStationaryAreaPosition()
        {
            isFreeze = true;
        }

        public void UnFreezeStationaryAreaPosition()
        {
            isFreeze = false;
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public float GetRadius()
        {
            return this.radius;
        }

        public void SetMesh(Mesh mesh, float radius)
        {
            SetRadius(radius);
            if (stationaryAreaStep == null)
            {
                stationaryAreaStep = SafetyAreaManager.Instance.GetStep<StationaryAreaStep>(SafetyAreaStepEnum.StationaryArea);
            }

            if (groundHeightStep == null)
            {
                groundHeightStep = SafetyAreaManager.Instance.GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight);
            }

            meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            //meshRenderer.material = Resources.Load<Material>("Material/SafetyEdgeMat");
            meshFilter.mesh = mesh;
            SetOriginHeight(groundHeightStep.GetPlaneHeight());
            perimeter = 2f * Mathf.PI * radius;
        }

        public float GetDistance(Vector3 position)
        {
            if (position.y >= 2.5f) return float.PositiveInfinity;
            return Vector2.Distance(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z), new Vector2(position.x, position.z));
        }
    }
}