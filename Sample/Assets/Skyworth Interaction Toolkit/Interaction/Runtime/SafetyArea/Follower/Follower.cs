using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : FollowerBase
{
    [Header("Following Settings")]
    public bool InstantFollowing = false;//是否无延迟跟随
    public bool LinearFollowing;//是否线性移动

    public Vector2 border_x; //x轴边界
    public Vector2 border_y; //y轴边界
    private Transform _slamHead;
    Vector2 initViewPoint;//相机画面的中点，坐标为（0.5f,0.5f）
    Vector3 viewPoint = Vector3.zero;// 面板中心坐标相对于相机画面的坐标。
    private Vector3 velocity = Vector3.zero;
    bool isFollower = false;

    Vector3 originPos = Vector3.zero;
    Vector3 desPos = Vector3.zero;
    float journeyLength = 0f;
    protected override void OnEnable()
    {
        base.OnEnable();

    }

    private void Start()
    {
        InitTrans();
    }

    protected override void LateUpdate()
    {
        if (StopFollower == false)
        {
            if (InstantFollowing)
            {
                InstantFollow();
            }
            else
            {
                if (IsFollower())
                {
                    if (LinearFollowing)
                    {
                        LinearFollow();
                    }
                    else
                    {
                        Follow();
                    }
                }
            }
        }
    }

    void InitTrans()
    {
        //if (API_GSXR_Slam.SlamManager?.head != null || GSXRManager.Instance.leftCamera != null)
        //{

            _slamHead = Camera.main.transform;
            transform.position = CalculateWindowPosition(Camera.main.transform);
            transform.rotation = CalculateWindowRotation(Camera.main.transform);
            initViewPoint = Camera.main.WorldToViewportPoint(_slamHead.position + (_slamHead.forward * WindowDistance));//相机画面中点坐标
        //}

    }


    protected bool IsFollower()
    {
        //if (API_GSXR_Slam.SlamManager?.head == null || GSXRManager.Instance.leftCamera == null)
        //{
        //    return false;
        //}
        viewPoint = Camera.main.WorldToViewportPoint(transform.position);// 面板在相机画面上的坐标

        if (viewPoint.x > initViewPoint.x + border_x.x + CalculateWindowOffsetX() || viewPoint.x < initViewPoint.x + border_x.y + CalculateWindowOffsetX() || viewPoint.y < initViewPoint.y + border_y.y + CalculateWindowOffsetY() || viewPoint.y > initViewPoint.y + border_y.x + CalculateWindowOffsetY())//边界判断
        {
            isFollower = true;
        }
        else if (Mathf.Abs(viewPoint.x - initViewPoint.x - CalculateWindowOffsetX()) < 0.03f && Mathf.Abs(viewPoint.y - initViewPoint.y - 2 * CalculateWindowOffsetY()) < 0.06f)
        {
            isFollower = false;
        }
        return isFollower;
    }

    protected override void Follow()
    {
        transform.position = Vector3.Lerp(transform.position, CalculateWindowPosition(Camera.main.transform), WindowFollowSpeed * Time.deltaTime);
        //transform.position = Vector3.SmoothDamp(transform.position, CalculateWindowPosition(API_GSXR_Slam.SlamManager.head), ref velocity, WindowFollowSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, CalculateWindowRotation(Camera.main.transform), WindowFollowSpeed * Time.deltaTime);
    }

    //Following camera without latency
    protected void InstantFollow()
    {
        transform.position = CalculateWindowPosition(Camera.main.transform);
        transform.rotation = CalculateWindowRotation(Camera.main.transform);
    }

    //For Linear Following, turn down the WindowFollowSpeed to around 0.5 at distance = 1 for best experience
    protected void LinearFollow()
    {
        originPos = transform.position;
        desPos = CalculateWindowPosition(Camera.main.transform);
        journeyLength = Vector3.Distance(originPos, desPos);
        transform.position = Vector3.Lerp(originPos, desPos, (Time.fixedDeltaTime) / journeyLength * WindowFollowSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, CalculateWindowRotation(Camera.main.transform), (Time.fixedDeltaTime) / journeyLength * WindowFollowSpeed);
    }

}
