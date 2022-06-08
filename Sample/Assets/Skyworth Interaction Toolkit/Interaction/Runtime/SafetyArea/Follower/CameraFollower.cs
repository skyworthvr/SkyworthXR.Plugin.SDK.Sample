using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.XR.Unity
{
    public class CameraFollower : FollowerBase
    {
        [Header("Following Settings")]
        public bool InstantFollowing;//是否无延迟跟随
        public bool LinearFollowing;//是否线性移动
        public Vector2 menu_size;//面板的长宽，默认面板的position在中心。

        private Transform _slamHead;
        Vector2 initViewPoint;//相机画面的中点，坐标为（0.5f,0.5f）
        Vector3 viewPoint = Vector3.zero;// 面板坐标相对于相机画面的坐标。
        Vector3 viewPoint_right = Vector3.zero;// 面板的上下左右端相对于相机画面的坐标。
        Vector3 viewPoint_left = Vector3.zero;
        Vector3 viewPoint_top = Vector3.zero;
        Vector3 viewPoint_bot = Vector3.zero;
        bool isFollower = false;
        Vector3 originPos = Vector3.zero;
        Vector3 desPos = Vector3.zero;
        float journeyLength = 0f;
        void Start()
        {
            _slamHead = Camera.main.transform;
            initViewPoint = Camera.main.WorldToViewportPoint(_slamHead.position + (_slamHead.forward * WindowDistance));
            InstantFollow();
        }

        //Nonlinear Following by default
        protected override void Follow()
        {
           
            transform.position = Vector3.Lerp(transform.position, CalculateWindowPosition(Camera.main.transform), WindowFollowSpeed * Time.deltaTime);
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

        protected override void LateUpdate()
        {
            if ((StopFollower == false) && !InstantFollowing)
            {
                if (IsFollower() && !LinearFollowing)
                {
                    Follow();
                }
                else if (IsFollower() && LinearFollowing)
                {
                    LinearFollow();
                }
            }
            else if ((StopFollower == false) && InstantFollowing)
            {
                InstantFollow();
            }
        }

        protected bool IsFollower()
        {

            viewPoint = Camera.main.WorldToViewportPoint(transform.position);// 面板在相机画面上的坐标
            viewPoint_right = Camera.main.WorldToViewportPoint(transform.TransformPoint(new Vector3(menu_size.x / 2, 0, 0)));// 面板边界在相机画面上的坐标
            viewPoint_left = Camera.main.WorldToViewportPoint(transform.TransformPoint(new Vector3(-menu_size.x / 2, 0, 0)));
            viewPoint_top = Camera.main.WorldToViewportPoint(transform.TransformPoint(new Vector3(0, menu_size.y / 2, 0)));
            viewPoint_bot = Camera.main.WorldToViewportPoint(transform.TransformPoint(new Vector3(0, -menu_size.y / 2, 0)));

            if (viewPoint_right.x > 1 || viewPoint_left.x < 0 || viewPoint_bot.y < 0 || viewPoint_top.y > 1)//边界判断
            {
                isFollower = true;
            }
            else if (Mathf.Abs(viewPoint.x - initViewPoint.x) < 0.05f && Mathf.Abs(viewPoint.y - initViewPoint.y) < 0.05f)//停止跟随的边界判断
            {
                isFollower = false;
            }
            return isFollower;
        }

    }
}
