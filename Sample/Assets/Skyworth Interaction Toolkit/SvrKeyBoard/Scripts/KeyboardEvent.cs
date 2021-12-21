/*
 * @Author: xieminghui
 * @Date: 2021-01-05 11:35:57
 * @Description: Description
 * @LastEditors: xieminghui
 * @LastEditTime: 2021-01-05 11:38:43
 * @Copyright: Copyright 2021 Skyworth VR. All rights reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Svr.Keyboard
{
    public class KeyboardEvent : MonoBehaviour, IGvrPointerHoverHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<PointerEventData> OnGvrPointerHover;
        public event Action<PointerEventData> OnPointerDown;
        public event Action<PointerEventData> OnPointerUp;
        void IGvrPointerHoverHandler.OnGvrPointerHover(PointerEventData eventData)
        {
            if (OnGvrPointerHover != null) OnGvrPointerHover(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
            if (OnPointerDown != null) OnPointerDown(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp");
            if (OnPointerUp != null) OnPointerUp(eventData);
        }

        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
