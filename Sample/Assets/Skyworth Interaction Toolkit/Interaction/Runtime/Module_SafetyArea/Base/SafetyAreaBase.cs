using System.Collections.Generic;
using UnityEngine;

public class SafetyAreaBase : MonoBehaviour
{
    protected float originHeight;
    protected MeshFilter meshFilter;
    protected List<Vector4> positionList;
    protected float perimeter;
    protected ExistSafetyAreaStateMachine existSafetyAreaStateMachine;

    public MeshRenderer meshRenderer;
    public GameObject outOfSafetyArea;
    public GameObject nomapUI;
    public GameObject slamLostUI;
    public float distance;
    public float alpha;

    protected float duration = 1.0f;

    protected List<ColorSection> colorSections = new List<ColorSection>()
    {
        new ColorSection(){ startColor = new Color(0.129f, 0.235f, 0.886f), endColor = new Color(0.118f, 0.365f, 0.051f) },
        new ColorSection(){ startColor = new Color(0.890f, 0.102f, 0.090f), endColor = new Color(0f, 0.482f, 0.012f) },
        new ColorSection(){ startColor = new Color(0.263f, 0.145f, 0.176f), endColor = new Color(0.639f, 0.098f, 0.3012f) }
    };

    protected Color nearColorStart = new Color(0.8902f, 0.102f, 0.09f, 1f);
    protected Color nearColorEnd = new Color(0.8902f, 0.5843f, 0.102f, 1f);

    public void Init()
    {
        if (existSafetyAreaStateMachine == null)
        {
            existSafetyAreaStateMachine = new ExistSafetyAreaStateMachine();
            existSafetyAreaStateMachine.InitStateMachine(this);
        }
        ChangeState(ExistSafetyAreaEnum.Normal);
    }

    /// <summary>
    /// 设置原始高度用于计算高度delta
    /// </summary>
    /// <param name="height"></param>
    public void SetOriginHeight(float height)
    {
        this.originHeight = height;
    }

    /// <summary>
    ///  获取原始高度
    /// </summary>
    /// <returns></returns>
    public float GetOriginHeight()
    {
        return this.originHeight;
    }

    /// <summary>
    /// 设置周长
    /// </summary>
    /// <param name="perimeter"></param>
    public void SetPerimeter(float perimeter)
    {
        this.perimeter = perimeter;
    }

    /// <summary>
    /// 获取周长
    /// </summary>
    /// <returns></returns>
    public float GetPerimter()
    {
        return perimeter;
    }

    /// <summary>
    /// 重新设置高度
    /// </summary>
    /// <param name="height"></param>
    public void ResetPlaneHeight(float height)
    {
        float heightdelta = height - originHeight;
        this.transform.position = new Vector3(this.transform.position.x, heightdelta, this.transform.position.z);
    }

    public void SetMaterial(Material mat)
    {
        meshRenderer.sharedMaterial = mat;
    }

    public virtual void Update()
    {
        if (outOfSafetyArea == null)
        {
            GameObject outOfSafetyAreaResource = Resources.Load<GameObject>("OutofSafetyArea");
            outOfSafetyArea = GameObject.Instantiate(outOfSafetyAreaResource, this.transform);
        }

        if (nomapUI == null)
        {
            GameObject nomapUIResource = Resources.Load<GameObject>("NoMapUI");
            nomapUI = GameObject.Instantiate(nomapUIResource, this.transform);
        }

        if (slamLostUI == null)
        {
            GameObject slamLostResource = Resources.Load<GameObject>("SlamLostUI");
            slamLostUI = GameObject.Instantiate(slamLostResource, this.transform);
        }

        if (meshRenderer == null)
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
        }

        if (meshFilter == null)
        {
            meshFilter = this.GetComponent<MeshFilter>();
        }

        //if (!SafetyAreaManager.Instance.isSettingSafetyArea)
        //{
        //    meshRenderer.enabled = API_GSXR_Slam.GSXR_Get_OfflineMapRelocState() == 1;
        //}
        //else
        //{
        //    meshRenderer.enabled = true;
        //}
    }

    public virtual void ChangeState(ExistSafetyAreaEnum existSafetyAreaEnum)
    {
        if (existSafetyAreaStateMachine != null)
        {
            existSafetyAreaStateMachine.ChangeState(existSafetyAreaEnum);
        }
    }

    protected List<Vector4> GetInteractionObjectPosition()
    {
        if (positionList == null)
        {
            positionList = new List<Vector4>();
        }
        positionList.Clear();

        //if (Module_InputSystem.instance == null) return positionList;

        Vector3 headTransformPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(headTransformPosition);
        positionList.Add(new Vector4(viewportPosition.x, viewportPosition.y, viewportPosition.z, 1f));
        foreach (var part in SvrTrackDevices.trackDevices)
        {
            if (part.transform.gameObject.activeSelf)
            {
                GameObject KSPart = part.transform.gameObject;
                Vector3 partPosition = KSPart.transform.position;// + KSPart.transform.forward * 0.5f;
                Vector3 viewportPartPosition = Camera.main.WorldToViewportPoint(partPosition);
                positionList.Add(new Vector4(viewportPartPosition.x, viewportPartPosition.y, viewportPartPosition.z, 1f));
            }
        }

        if (positionList.Count < 3)
        {
            for (int i = positionList.Count; i < 3; i++)
            {
                positionList.Add(new Vector4(viewportPosition.x, viewportPosition.y, viewportPosition.z, 1f));
            }
        }

        return positionList;
    }

    private void OnDestroy()
    {
        existSafetyAreaStateMachine.ExitCurrentState();
    }

}
