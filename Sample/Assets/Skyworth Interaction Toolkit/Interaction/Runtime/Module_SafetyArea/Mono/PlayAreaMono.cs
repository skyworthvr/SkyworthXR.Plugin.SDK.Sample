using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyworth.Interaction.SafetyArea
{
    public class PlayAreaMono : SafetyAreaBase
    {
        private Color[] colors;

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            List<Vector4> positionList = GetInteractionObjectPosition();
            //Debug.Log(positionList.Count);
            if (positionList.Count != 0)
            {
                meshRenderer.sharedMaterial.SetInt("positionCount", positionList.Count);
                meshRenderer.sharedMaterial.SetVectorArray("interactionPosition", positionList);
            }

            distance = GetDistance(Camera.main.transform.position);
            alpha = GetPlaneFillPercentage(Camera.main.transform.position);
            meshRenderer.sharedMaterial.SetFloat("alpha", alpha);

            Color colorStart = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].startColor;
            Color colorEnd = colorSections[SafetyAreaManager.Instance.OriginSafetyAreaColorIndex].endColor;
            meshRenderer.sharedMaterial.SetColor("_Color1", colorStart);
            meshRenderer.sharedMaterial.SetColor("_Color2", colorEnd);

            var lerp = Mathf.PingPong(Time.time, duration) / duration;
            meshRenderer.sharedMaterial.SetColor("settingsColor", Color.Lerp(nearColorStart, nearColorEnd, lerp));

            meshRenderer.sharedMaterial.SetVector("_MainTex_ST", new Vector4(Mathf.RoundToInt(perimeter / PlayAreaConstant.SAFETY_AREA_HEIGHT * 7f), 7f, 0f, -0.05f));

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
            //TODO StateMachine
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

            //        nomapUI.SetActive(false);                
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

        public void SetColor(Color[] colors)
        {
            this.colors = colors;
        }

        public Color[] GetColor()
        {
            return this.colors;
        }

        public void SetMesh(Mesh mesh, Color[] colors, float perimeter)
        {
            SetColor(colors);

            meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            //meshRenderer.material = Resources.Load<Material>("Material/SafetyEdgeMat");
            meshFilter.mesh = mesh;
            GroundHeightStep groundHeightStep = SafetyAreaManager.Instance.GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight);
            //originHeight = groundHeightStep.GetPlaneHeight();
            SetOriginHeight(groundHeightStep.GetPlaneHeight());
            this.perimeter = perimeter;
        }

        public float GetPlaneFillPercentage(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            List<int> effectIndices = SafetyAreaVertexHelper.CaculateEffectVerticeIndices(localPosition, 1f);
            if (effectIndices.Count == 0) return 999f;
            int fillCount = 0;
            for (int i = 0; i < effectIndices.Count; i++)
            {
                if (colors[effectIndices[i]] == Color.white)
                {
                    fillCount++;
                }
            }
            return (effectIndices.Count - fillCount) / (effectIndices.Count * 1f / SafetyAreaManager.Instance.AlphaParam);
        }

        public float GetDistance(Vector3 position)
        {
            if (position.y >= 2.5f) return float.PositiveInfinity;
            return Vector3.Distance(transform.position, position);
        }
    }
}
