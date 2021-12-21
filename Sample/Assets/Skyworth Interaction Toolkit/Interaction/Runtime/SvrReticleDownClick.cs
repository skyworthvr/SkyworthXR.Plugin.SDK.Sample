using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skyworth.Interaction
{
    public class SvrReticleDownClick : MonoBehaviour
    {

        private MeshRenderer m_meshRenderer;
        private MeshRenderer mSelfRenderer;

        public Material MaterialComp { get; private set; }
        /// <summary>
        /// 用于判断是否开启全局事件触发
        /// </summary>
        /// <remarks>
        /// 设置成true则只能在挂有SvrAutoClickEnable脚本的组件上使用，设置成false则能全局使用
        /// </remarks>
        public bool OnelySvrAutoClickEnable = false;
        private readonly float DEFALUTCOUNT = 1f;
        private float mEnterTime = 0;
        private float mTargetTime;
        private bool mIsContitnue;
        private float manger = 0;
        private GvrReticlePointer mGvrReticlePointer;
        //private bool EventExcuted = false;
        private int EventExcuteCount = 0;
        //private bool mFoundClick = false;
        private GameObject raycastResultObj;
        private Renderer rendererComponent;
        // Use this for initialization
        void Start()
        {
            mTargetTime = DEFALUTCOUNT;
            if (mGvrReticlePointer == null) mGvrReticlePointer = GetComponentInParent<GvrReticlePointer>();
            mGvrReticlePointer.OnPointerEnterEvent = GvrReticlePointer_OnPointerEnterEvent;
            mGvrReticlePointer.OnPointerHoverEvent = GvrReticlePointer_OnPointerHoverEvent;
            mGvrReticlePointer.OnPointerExitEvent = GvrReticlePointer_OnPointerExitEvent;
            m_meshRenderer = mGvrReticlePointer.GetComponent<MeshRenderer>();
            mSelfRenderer = GetComponent<MeshRenderer>();

            rendererComponent = GetComponent<Renderer>();
            rendererComponent.sortingOrder = m_meshRenderer.sortingOrder;

            MaterialComp = rendererComponent.material;
            CreateReticleVertices();
        }

        private void GvrReticlePointer_OnPointerHoverEvent(RaycastResult arg0)
        {
            if (!raycastResultObj) return;
            //Debug.LogFormat("kevin:{0} - {1} = {2} < {3}", Time.time, mEnterTime,(Time.time - mEnterTime), mTargetTime);
            if (Time.time - mEnterTime < mTargetTime)
            {
                manger = (360.0f / mTargetTime) * (Time.time - mEnterTime);
                MaterialComp.SetFloat("_FillAmount", manger);
            }
            else
            {
                //Debug.LogFormat("kevin:EventExcuteCount {0}", EventExcuteCount);
                MaterialComp.SetFloat("_FillAmount", 360);
                if (EventExcuteCount < 2)
                {
                    EventExcuteCount++;
                    mGvrReticlePointer.PointTriggerDown = true;
                    if (EventExcuteCount > 1)
                    {
                        mGvrReticlePointer.PointTriggerDown = false;
                        if (mIsContitnue)
                        {
                            mEnterTime = Time.time;
                            EventExcuteCount = 0;
                        }

                    }

                }

            }
        }

        private void OnEnable()
        {
            if (mGvrReticlePointer == null) mGvrReticlePointer = GetComponentInParent<GvrReticlePointer>();
            mGvrReticlePointer.GazeModeEnable = true;
        }
        private void OnDisable()
        {
            mGvrReticlePointer.GazeModeEnable = false;
        }
        private void GvrReticlePointer_OnPointerEnterEvent(RaycastResult raycastResult)
        {
            GvrPointerInputModule module = EventSystem.current.currentInputModule as GvrPointerInputModule;
            if (OnelySvrAutoClickEnable)
            {
                GameObject clickenable = module.EventExecutor.GetEventHandler<ISvrAutoClickEnable>(raycastResult.gameObject);
                if (!clickenable)
                {
                    return;
                }
            }
            raycastResultObj = null;
            mEnterTime = Time.time;
            MaterialComp.SetFloat("_FillAmount", 0);
            manger = 0;
            //EventExcuted = false;
            EventExcuteCount = 0;

            GameObject click = module.EventExecutor.GetEventHandler<IPointerClickHandler>(raycastResult.gameObject);
            if (click)
            {
                bool canOperation = false;
                Selectable selectable = click.GetComponent<Selectable>();
                if (selectable != null)
                {
                    if (selectable.interactable)
                    {
                        canOperation = true;
                    }
                }
                else
                {
                    canOperation = true;
                }
                if (canOperation)
                {
                    raycastResultObj = click;
                    UICountDown mUICountDown = click.GetComponent<UICountDown>();
                    if (mUICountDown != null)
                    {
                        mTargetTime = mUICountDown.Count;
                        mIsContitnue = mUICountDown.Continue;
                    }
                    else
                    {
                        mTargetTime = DEFALUTCOUNT;
                    }
                }
            }

        }


        private void GvrReticlePointer_OnPointerExitEvent(GameObject gameObject)
        {
            mTargetTime = DEFALUTCOUNT;
            mEnterTime = 0;
            EventExcuteCount = 0;
            mIsContitnue = false;
            //EventExcuted = false;
            manger = 0;
            MaterialComp.SetFloat("_FillAmount", 0);
            raycastResultObj = null;
            mGvrReticlePointer.PointTriggerDown = false;
        }

        private void OnRenderObject()
        {
            MaterialComp.SetFloat("_InnerDiameter", m_meshRenderer.material.GetFloat("_InnerDiameter"));
            MaterialComp.SetFloat("_OuterDiameter", m_meshRenderer.material.GetFloat("_OuterDiameter"));
            MaterialComp.SetFloat("_DistanceInMeters", m_meshRenderer.material.GetFloat("_DistanceInMeters"));
            rendererComponent.sortingOrder = m_meshRenderer.sortingOrder;
        }
        private void OnApplicationPause(bool pause)
        {
            mTargetTime = DEFALUTCOUNT;
            mEnterTime = 0;
            EventExcuteCount = 0;
            mIsContitnue = false;
            manger = 0;
            raycastResultObj = null;
        }
        // Update is called once per frame
        void Update()
        {

        }

        private void CreateReticleVertices()
        {
            Mesh mesh = new Mesh();
            gameObject.AddComponent<MeshFilter>();
            GetComponent<MeshFilter>().mesh = mesh;

            int segments_count = 20;
            int vertex_count = (segments_count + 1) * 2;

            #region Vertices

            Vector3[] vertices = new Vector3[vertex_count];
            Vector2[] UVS = new Vector2[vertex_count];
            const float kTwoPi = Mathf.PI * 2.0f;
            int vi = 0;
            for (int si = 0; si <= segments_count; ++si)
            {
                // Add two vertices for every circle segment: one at the beginning of the
                // prism, and one at the end of the prism.
                float angle = (float)si / (float)(segments_count) * kTwoPi;
                //Debug.Log(angle);
                float x = Mathf.Sin(angle);
                float y = Mathf.Cos(angle);

                vertices[vi++] = new Vector3(x, y, 0.0f); // Outer vertex.
                vertices[vi++] = new Vector3(x, y, 1.0f); // Inner vertex.
                UVS[vi - 2] = new Vector2(x, y);
                UVS[vi - 1] = new Vector2(x, y);
            }
            #endregion

            #region Triangles
            int indices_count = (segments_count + 1) * 3 * 2;
            int[] indices = new int[indices_count];

            int vert = 0;
            int idx = 0;
            for (int si = 0; si < segments_count; ++si)
            {
                indices[idx++] = vert + 1;
                indices[idx++] = vert;
                indices[idx++] = vert + 2;

                indices[idx++] = vert + 1;
                indices[idx++] = vert + 2;
                indices[idx++] = vert + 3;

                vert += 2;
            }
            #endregion

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.uv = UVS;


            mesh.RecalculateBounds();
#if !UNITY_5_5_OR_NEWER
    // Optimize() is deprecated as of Unity 5.5.0p1.
    mesh.Optimize();
#endif  // !UNITY_5_5_OR_NEWER
        }
    }
}
