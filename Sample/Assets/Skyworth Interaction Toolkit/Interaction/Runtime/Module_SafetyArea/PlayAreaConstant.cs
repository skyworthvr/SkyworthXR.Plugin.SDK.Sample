using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaConstant
{
    /// <summary>
    /// 笔刷大小
    /// </summary>
    public const float BRUSH_SIZE = 0.24f;
    /// <summary>
    /// 网格线之间间距
    /// </summary>
    public const float CELL_SIZE = 0.08f;
    /// <summary>
    /// 网格线个数
    /// </summary>
    public const int GRID_SIZE = 100;
    /// <summary>
    /// 默认离头部高度
    /// </summary>
    public const float DEFAULT_HEIGHT_FROM_HEAD = 0.5f;
    /// <summary>
    /// 原地区域半径
    /// </summary>
    public const float STATIONARY_AREA_RADIUS = 1.5f;
    /// <summary>
    /// 原地区域分割份数
    /// </summary>
    public const int CYLINDER_SPLIT_COUNT = 20;
    /// <summary>
    /// 低头角度
    /// </summary>
    public const float HEAD_BOW_ANGLE = 45f;

    /// <summary>
    /// 安全区域高度
    /// </summary>
    public const float SAFETY_AREA_HEIGHT = 2.5f;

    /// <summary>
    /// 游戏区域名称
    /// </summary>
    public const string PLAY_AREA_NAME = "PlayArea";
    /// <summary>
    ///  原地区域名称
    /// </summary>
    public const string STATIONARY_NAME = "StationaryArea";
    /// <summary>
    /// 本地保存文件路径+名称
    /// </summary>
    public const string SAVE_FILE_NAME = "/sdcard/SafetyArea.txt";
}
