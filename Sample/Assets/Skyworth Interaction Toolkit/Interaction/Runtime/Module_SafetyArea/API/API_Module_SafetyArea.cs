using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyworth.Interaction.SafetyArea
{
    public class API_Module_SafetyArea
    {
        /// <summary>
        /// 进入安全区域时的回调注册
        /// </summary>
        /// <param name="callback"></param>
        public static void RegistSafetyAreaEnterCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnEnterSafetyArea += callback;
        }

        /// <summary>
        /// 进入安全区域时的回调注销
        /// </summary>
        /// <param name="callback"></param>
        public static void UnRegistSafetyAreaEnterCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnEnterSafetyArea -= callback;
        }

        /// <summary>
        /// 退出安全区域时的回调注册
        /// </summary>
        /// <param name="callback"></param>
        public static void RegistSafetyAreaExitCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnExitSafetyArea += callback;
        }

        /// <summary>
        /// 退出安全区域时的回调注销
        /// </summary>
        /// <param name="callback"></param>
        public static void UnRegistSafetyAreaExitCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnExitSafetyArea += callback;
        }

        /// <summary>
        /// 开始设置安全区域时的回调注册
        /// </summary>
        /// <param name="callback"></param>
        public static void RegistSafetyAreaStartCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnBeginSetSafeArea += callback;
        }

        /// <summary>
        /// 开始设置安全区域时的回调注销
        /// </summary>
        /// <param name="callback"></param>
        public static void UnRegistSafetyAreaStartCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnBeginSetSafeArea -= callback;
        }

        /// <summary>
        /// 完成设置安全区域时的回调注册
        /// </summary>
        /// <param name="callback"></param>
        public static void RegistSafetyAreaFinishCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnFinishSetSafeArea += callback;
        }

        /// <summary>
        /// 完成设置安全区域时的回调注销
        /// </summary>
        /// <param name="callback"></param>
        public static void UnRegistSafetyAreaFinishCallback(Action callback)
        {
            SafetyAreaManager.Instance.OnFinishSetSafeArea += callback;
        }

        /// <summary>
        /// 开始设置安全区域
        /// </summary>
        public static void StartSetSafetyArea()
        {
            SafetyAreaManager.Instance.StartSetSafetyArea();
        }

        /// <summary>
        /// 重新设置区域高度(如果没有安全区域则会重新设置)
        /// </summary>
        public static void StartSetSafetyAreaHeight()
        {
            SafetyAreaManager.Instance.StartSetSafetyAreaHeight();
        }

        /// <summary>
        /// 获取安全区域颜色
        /// </summary>
        /// <returns></returns>
        public static int GetSafetyAreaColor()
        {
            return SafetyAreaManager.Instance.OriginSafetyAreaColorIndex;
        }

        /// <summary>
        /// 设置安全区域颜色
        /// </summary>
        /// <param name="color"></param>
        public static void SetSafetyAreaColor(int index)
        {
            SafetyAreaManager.Instance.OriginSafetyAreaColorIndex = index;
        }

        /// <summary>
        /// 获取低头是否显示安全区域
        /// </summary>
        /// <returns></returns>
        public static bool GetShowAreaWhenBowHead()
        {
            return SafetyAreaManager.Instance.ShowAreaWhenBowHead;
        }

        /// <summary>
        /// 低头时是否显示游戏区域轮廓
        /// </summary>
        /// <param name="isOpen"></param>
        public static void SetShowAreaWhenBowHead(bool isOpen)
        {
            SafetyAreaManager.Instance.ShowAreaWhenBowHead = isOpen;
        }

        /// <summary>
        /// 获取灵敏度
        /// </summary>
        /// <returns></returns>
        public static float GetSensitivity()
        {
            return SafetyAreaManager.Instance.OriginAlphaParam;
        }

        /// <summary>
        /// 设置安全区域灵敏度 范围0~1
        /// </summary>
        /// <param name="value"></param>
        public static void SetSensitivity(float value)
        {
            SafetyAreaManager.Instance.OriginAlphaParam = value;
        }

        /// <summary>
        /// 删除已有的原地区域
        /// </summary>
        public static void DestroySafetyArea()
        {
            SafetyAreaManager.Instance.DestroySafetyArea();
        }

        public static void DisableSafetyAreaDisplay(bool isDisable)
        {
            SafetyAreaManager.Instance.IsDisableSafetyArea = isDisable;
        }
    }
}