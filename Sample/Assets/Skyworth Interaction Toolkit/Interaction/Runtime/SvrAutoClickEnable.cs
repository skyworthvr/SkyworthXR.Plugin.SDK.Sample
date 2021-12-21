using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Skyworth.Interaction
{
    public interface ISvrAutoClickEnable : IEventSystemHandler
    {
    }
    /// <summary>
    /// 挂在Button上可以触发锚点倒计时
    /// </summary>
    public class SvrAutoClickEnable : MonoBehaviour, ISvrAutoClickEnable
    {
    }
}
