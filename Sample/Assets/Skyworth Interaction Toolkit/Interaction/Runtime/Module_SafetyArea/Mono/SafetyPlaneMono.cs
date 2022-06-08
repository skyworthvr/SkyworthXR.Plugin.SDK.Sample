using Skyworth.Interaction.SafetyArea;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SafetyPlaneMono : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool isFreeze = false;
    private bool isHover = false;
    private bool canFill = false;
    private int lastPaintIndex = -1;
    private PointerEventData currentPointerEventData;
    private PointerEventData hoverPointerEventData;
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    //private Mesh hoverMesh;
    private MeshRenderer hoverMeshRenderer;
    //private Mesh largerMesh;
    //private MeshRenderer largerMeshRenderer;
    private Color[] colors;
    //private Color[] hoverColors;
    //private Color[] largerColors;

    private GroundHeightStep groundHeightStep;
    private float animationTime;
    private bool isPlayAnimation = false;

    private Action<PointerEventData> OnPointerClickDown
    {
        get;
        set;
    }

    private Action<PointerEventData> OnPointerClickUp
    {
        get;
        set;
    }

    public void Init()
    {

        if (groundHeightStep == null)
        {
            groundHeightStep = SafetyAreaManager.Instance.GetStep<GroundHeightStep>(SafetyAreaStepEnum.GroundHeight);
        }

        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = SafetyAreaVertexHelper.GeneratePlaneMesh();
        mesh = meshFilter.mesh;
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Material/SafetyPlaneMat");
        MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        GameObject hoverObject = GameObject.Instantiate(this.gameObject);
        hoverMeshRenderer = hoverObject.GetComponent<MeshRenderer>();
        GameObject.Destroy(hoverObject.GetComponent<SafetyPlaneMono>());
        GameObject.Destroy(hoverObject.GetComponent<MeshCollider>());
        //hoverMesh = hoverObject.GetComponent<MeshFilter>().mesh;

        hoverObject.transform.SetParent(this.gameObject.transform);

        ClearAllMeshColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFreeze)
        {
            ContinueSetPlaneHeight();
            meshRenderer.sharedMaterial.SetVector("_MainTex_ST", new Vector4(30f, 30f, Camera.main.transform.position.x, Camera.main.transform.position.z));
        }
        else
        {
            if (canFill)
            {
                FillNearest(currentPointerEventData);
            }

            //ClearHoverMesh();
            if (isHover)
            {
                FillHoverNearest(hoverPointerEventData);
            }
        }

        if (isPlayAnimation)
        {
            animationTime += Time.deltaTime;
            meshRenderer.sharedMaterial.SetFloat("_Mtime", animationTime / 15f);
        }
    }

    public void FreezePlaneHeight()
    {
        isFreeze = true;
    }

    public void UnFreezePlaneHeight()
    {
        ContinueSetPlaneHeight();
        isFreeze = false;
    }

    public void ContinueSetPlaneHeight()
    {
        
        GameObject interactiveObject = GetInteractiveGameObject();
        if (interactiveObject != null)
        {
            groundHeightStep.SetPlaneHeight(interactiveObject.transform.position.y);
        }
        Transform headTransform = GetHeadTransform();
        groundHeightStep.SetHeadPosition(headTransform.position);
        this.gameObject.transform.position = new Vector3(headTransform.position.x, groundHeightStep.GetPlaneHeight(), headTransform.position.z);
        Vector3 localHeadPosition = this.transform.InverseTransformPoint(Camera.main.transform.position);
        meshRenderer.sharedMaterial.SetVector("headPosition", new Vector4(localHeadPosition.x, localHeadPosition.y, localHeadPosition.z, 1));
    }

    private void EnableFill(PointerEventData eventData)
    {
        canFill = true;
    }

    private void DisableFill(PointerEventData eventData)
    {
        canFill = false;
    }

    private GameObject GetInteractiveGameObject()
    {
        GameObject KSModel = null;
        foreach (var part in SvrTrackDevices.trackDevices)
        {
            if (part.gameObject.activeSelf)
            {
                GameObject KSPart = part.gameObject;
                if (KSModel == null || KSModel.transform.position.y > KSPart.transform.position.y)
                {
                    KSModel = KSPart;
                }
            }
        }
        return KSModel;
    }

    private Transform GetHeadTransform()
    {
        return Camera.main.transform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPointerEventData = eventData;
        OnPointerClickDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentPointerEventData = null;
        OnPointerClickUp?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverPointerEventData = eventData;
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPointerEventData = null;
        isHover = false;
    }

    public void RegistPointerDownFillEvent()
    {
        OnPointerClickDown += EnableFill;
    }

    public void UnRegistPointerDownFillEvent()
    {
        OnPointerClickDown -= EnableFill;
    }

    public void RegistPointerUpFillEvent()
    {
        OnPointerClickUp += DisableFill;
    }

    public void UnRegistPointerUpFillEvent()
    {
        OnPointerClickUp -= DisableFill;
    }

    public void RegistPointerUpEvent(Action<PointerEventData> callback)
    {
        OnPointerClickUp += callback;
    }

    public void UnRegistPointerUpEvent(Action<PointerEventData> callback)
    {
        OnPointerClickUp -= callback;
    }

    public void FillNearest(PointerEventData pointerEventData)
    {
        if (pointerEventData == null)
        {
            Debug.LogError("currentPointerEventData == null");
            return;
        }

        Vector3 raycastPosition = pointerEventData.pointerCurrentRaycast.worldPosition;
        Vector3 pointerLocalPosition = transform.InverseTransformPoint(raycastPosition);
        List<int> effectIndices = SafetyAreaVertexHelper.CaculateEffectVerticeIndices(pointerLocalPosition, PlayAreaConstant.BRUSH_SIZE);
        for (int i = 0; i < effectIndices.Count; i++)
        {
            int index = effectIndices[i];
            colors[index] = Color.white;
            lastPaintIndex = index;
        }

        mesh.colors = colors;
    }

    public void FillHoverNearest(PointerEventData pointerEventData)
    {
        if (pointerEventData == null)
        {
            //Debug.LogError("hoverPointerEventData == null");
            return;
        }

        Vector3 raycastPosition = pointerEventData.pointerCurrentRaycast.worldPosition;
        Vector3 pointerLocalPosition = transform.InverseTransformPoint(raycastPosition);
        hoverMeshRenderer.sharedMaterial.SetFloat("brushSize", PlayAreaConstant.BRUSH_SIZE);
        hoverMeshRenderer.sharedMaterial.SetVector("pointerLocalPosition", new Vector4(pointerLocalPosition.x, pointerLocalPosition.y, pointerLocalPosition.z, 1.0f));
        //List<int> effectHoverIndices = SafetyAreaVertexHelper.CaculateEffectVerticeIndices(pointerLocalPosition, PlayAreaConstant.BRUSH_SIZE);
        //for (int i = 0; i < effectHoverIndices.Count; i++)
        //{
        //    int index = effectHoverIndices[i];
        //    hoverColors[index] = new Color(1f, 1f, 1f, 1f);
        //    //lastPaintIndex = index;
        //}

        //hoverMesh.colors = hoverColors;
    }

    public void GenerateEdgeMesh(Action<Mesh, float> onGenerateMesh)
    {
        if (lastPaintIndex == -1)
        {
            Debug.LogError("lastPaintIndex == -1");
            return;
        }

        SafetyAreaEightNeighbourHelper.EightNeighbours(lastPaintIndex, (index) =>
        {
            return colors[index] == Color.white;
        }, (edgeIndices) =>
        {
            float planeHeight = groundHeightStep.GetPlaneHeight();
            float perimeter = 0f;
            Mesh edgeMesh = SafetyAreaVertexHelper.GenerateEdgeMesh(mesh, edgeIndices, planeHeight + PlayAreaConstant.SAFETY_AREA_HEIGHT, planeHeight, ref perimeter);
            onGenerateMesh?.Invoke(edgeMesh, perimeter);
        });
    }

    //填充中间
    public void FillIndices()
    {
        if (lastPaintIndex == -1)
        {
            Debug.LogError("lastPaintIndex == -1");
            return;
        }

        SafetyAreaEightNeighbourHelper.EightNeighbours(lastPaintIndex, (index) =>
        {
            return colors[index] == Color.white;
        }, (edgeIndices) =>
        {
            List<int> fillIndices = ScanLineFillHelper.ScaneLine((index) =>
            {
                return colors[index] == Color.white;
            }, edgeIndices);

            for (int i = 0; i < fillIndices.Count; i++)
            {
                colors[fillIndices[i]] = Color.white;
            }
            mesh.colors = colors;
        });
    }

    public bool CheckIndicesEnough()
    {
        int count = 0;
        for (int i = 0; i < (PlayAreaConstant.GRID_SIZE + 1); i++)
        {
            for (int j = 0; j < (PlayAreaConstant.GRID_SIZE + 1); j++)
            {
                int index = i * (PlayAreaConstant.GRID_SIZE + 1) + j;
                if (colors[index] == Color.white)
                {
                    count++;
                }
            }
        }
        Debug.Log("fillCount=" + count);
        int oneSquareMeterIndexCount = (int)Mathf.Pow(Mathf.CeilToInt(1f / PlayAreaConstant.CELL_SIZE), 2);
        Debug.Log("oneSquareMeterIndexCount:" + oneSquareMeterIndexCount);
        return count > oneSquareMeterIndexCount;
    }

    public void ClearAllMeshColor()
    {
        ClearMeshColor();
        //ClearHoverMesh();
    }

    private void ClearMeshColor()
    {
        colors = Enumerable.Repeat(Color.clear, mesh.vertexCount).ToArray();
        mesh.colors = colors;
    }

    //private void ClearHoverMesh()
    //{
    //    hoverColors = Enumerable.Repeat(Color.clear, hoverMesh.vertexCount).ToArray();
    //    hoverMesh.colors = hoverColors;
    //}

    public void SetPlaneMat(Material mat)
    {
        meshRenderer.sharedMaterial = mat;
    }

    public void SetHoverPlaneMat(Material mat)
    {
        hoverMeshRenderer.sharedMaterial = mat;
    }

    public void StartPlaneAnimation()
    {
        animationTime = 0.3f;
        meshRenderer.sharedMaterial.SetFloat("_AniUVScale", 1f);
        isPlayAnimation = true;
    }

    public void StopPlaneAnimation()
    {
        meshRenderer.sharedMaterial.SetFloat("_AniUVScale", 0f);
        isPlayAnimation = false;
    }

    public Color[] GetColorArray()
    {
        return colors;
    }
}
