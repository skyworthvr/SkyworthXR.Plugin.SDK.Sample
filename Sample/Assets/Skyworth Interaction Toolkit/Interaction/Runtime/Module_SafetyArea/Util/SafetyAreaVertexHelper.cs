using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyAreaVertexHelper
{
    private static List<int> effectIndicesList = new List<int>();
    /// <summary>
    /// When raycast hit the plane mesh, caculate the brush effected vertex index
    /// </summary>
    /// <param name="localHitPoint"></param>
    public static List<int> CaculateEffectVerticeIndices(Vector3 localHitPoint, float brushSize = PlayAreaConstant.BRUSH_SIZE)
    {
        effectIndicesList.Clear();
        if (localHitPoint.x == 0f && localHitPoint.z == 0f)
        {
            return effectIndicesList;
        }

        if (localHitPoint.x < (-PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE / 2f) || localHitPoint.x > (PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE / 2f)
            || localHitPoint.z < (-PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE / 2f) || localHitPoint.z > (PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE / 2f))
        {
            return effectIndicesList;
        }

        float x = localHitPoint.x + PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE / 2f;
        float z = localHitPoint.z + PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE / 2f;

        float rangeDown = Mathf.Clamp(z - brushSize, 0, PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE);
        float rangeUp = Mathf.Clamp(z + brushSize, 0, PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE);
        float columnDistance = PlayAreaConstant.CELL_SIZE;
        int effectColumnStart = Mathf.CeilToInt(rangeDown / columnDistance);
        int effectColumnStop = Mathf.FloorToInt(rangeUp / columnDistance);
        for (int i = effectColumnStart; i <= effectColumnStop; i++)
        {
            float zDistance = Mathf.Abs(z - i * columnDistance);
            float rowRangeSize = Mathf.Sqrt(Mathf.Pow(brushSize, 2) - Mathf.Pow(zDistance, 2) + Mathf.Epsilon);
            float rangeLeft = Mathf.Clamp(x - rowRangeSize, 0, PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE);
            float rangeRight = Mathf.Clamp(x + rowRangeSize, 0, PlayAreaConstant.GRID_SIZE * PlayAreaConstant.CELL_SIZE);
            float rowDistance = PlayAreaConstant.CELL_SIZE;
            int effectRowStart = Mathf.CeilToInt(rangeLeft / rowDistance);
            int effectRowStop = Mathf.FloorToInt(rangeRight / rowDistance) - 1;
            for (int j = effectRowStart; j <= effectRowStop; j++)
            {
                int index = i * (PlayAreaConstant.GRID_SIZE + 1) + j + 1;
                if (index <= ((PlayAreaConstant.GRID_SIZE + 1) * (PlayAreaConstant.GRID_SIZE + 1) - 1))
                {
                    effectIndicesList.Add(index);
                }
            }
        }
        return effectIndicesList;
    }

    private static Vector2 _cellSize = new Vector2(PlayAreaConstant.CELL_SIZE, PlayAreaConstant.CELL_SIZE);
    private static Vector2Int _gridSize = new Vector2Int(PlayAreaConstant.GRID_SIZE, PlayAreaConstant.GRID_SIZE);

    public static Mesh GeneratePlaneMesh()
    {
        Mesh planeMesh = new Mesh();

        //计算Plane大小
        Vector2 size;
        size.x = _cellSize.x * _gridSize.x;
        size.y = _cellSize.y * _gridSize.y;

        //计算Plane一半大小
        Vector2 halfSize = size / 2;

        //计算顶点及UV
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 vertice = Vector3.zero;
        Vector2 uv = Vector3.zero;

        for (int y = 0; y < _gridSize.y + 1; y++)
        {
            vertice.z = y * _cellSize.y - halfSize.y;//计算顶点Y轴
            uv.y = y * _cellSize.y / size.y;//计算顶点纹理坐标V

            for (int x = 0; x < _gridSize.x + 1; x++)
            {
                vertice.x = x * _cellSize.x - halfSize.x;//计算顶点X轴
                uv.x = x * _cellSize.x / size.x;//计算顶点纹理坐标U

                vertices.Add(vertice);//添加到顶点数组
                uvs.Add(uv);//添加到纹理坐标数组
            }
        }

        //顶点序列
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        int startIndex = 0;
        int[] indexs = new int[_gridSize.x * _gridSize.y * 2 * 3];//顶点序列
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                //四边形四个顶点
                a = y * (_gridSize.x + 1) + x;//0
                b = (y + 1) * (_gridSize.x + 1) + x;//1
                c = b + 1;//2
                d = a + 1;//3

                //计算在数组中的起点序号
                startIndex = y * _gridSize.x * 2 * 3 + x * 2 * 3;

                //左上三角形
                indexs[startIndex] = a;//0
                indexs[startIndex + 1] = b;//1
                indexs[startIndex + 2] = c;//2

                //右下三角形
                indexs[startIndex + 3] = c;//2
                indexs[startIndex + 4] = d;//3
                indexs[startIndex + 5] = a;//0
            }
        }
        planeMesh.SetVertices(vertices);//设置顶点
        planeMesh.SetUVs(0, uvs);//设置UV
        planeMesh.SetIndices(indexs, MeshTopology.Triangles, 0);//设置顶点序列
        planeMesh.RecalculateNormals();
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateTangents();
        return planeMesh;
    }

    public static Mesh GenerateEdgeMesh(Mesh planeMesh, List<int> vertexIndexList, float topY, float bottomY, ref float perimeter)
    {
        perimeter = 0f;
        List<Vector3> geometry = new List<Vector3>();
        Vector3[] planeVertices = planeMesh.vertices;
        for (int i = 0; i < vertexIndexList.Count; i++)
        {
            geometry.Add(planeVertices[vertexIndexList[i]]);
            if (i < (vertexIndexList.Count - 2))
            {
                perimeter += Vector3.Distance(planeVertices[vertexIndexList[i]], planeVertices[vertexIndexList[i + 1]]);
            }
        }
        Debug.Log("Perimeter:" + perimeter);

        int numPoints = geometry.Count;

        Vector3[] vertices = new Vector3[numPoints * 2];
        Vector2[] uvs = new Vector2[numPoints * 2];
        for (int i = 0; i < numPoints; ++i)
        {
            Vector3 v = geometry[i];
            vertices[i] = new Vector3(v.x, bottomY, v.z);
            vertices[i + numPoints] = new Vector3(v.x, topY, v.z);
            uvs[i] = new Vector2((float)i / (numPoints - 1), 0.0f);
            uvs[i + numPoints] = new Vector2(uvs[i].x, 1.0f);
        }

        int[] triangles = new int[(numPoints - 1) * 2 * 3];
        for (int i = 0; i < numPoints - 1; ++i)
        {
            // the geometry is built clockwised. only the back faces should be rendered in the camera frame mask
            triangles[i * 6 + 0] = i;
            triangles[i * 6 + 1] = i + numPoints;
            triangles[i * 6 + 2] = i + 1 + numPoints;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + 1 + numPoints;
            triangles[i * 6 + 5] = i + 1;
        }

        Mesh edgeMesh = new Mesh();
        edgeMesh.vertices = vertices;
        edgeMesh.uv = uvs;
        edgeMesh.triangles = triangles;

        return edgeMesh;
    }

    public static Mesh GenerateCylinderMesh(Vector3 circleCenter, float topY, float bottomY, float radio = PlayAreaConstant.STATIONARY_AREA_RADIUS)
    {
        List<Vector3> geometry = new List<Vector3>();
        float angleSpace = 2 * Mathf.PI / PlayAreaConstant.CYLINDER_SPLIT_COUNT;
        for (int i = 0; i <= PlayAreaConstant.CYLINDER_SPLIT_COUNT; i++)
        {
            float angle = i * angleSpace;
            float x = radio * Mathf.Cos(angle);
            float z = radio * Mathf.Sin(angle);
            geometry.Add(new Vector3(circleCenter.x + x, circleCenter.y, circleCenter.z + z));
        }
        int numPoints = geometry.Count;

        Vector3[] vertices = new Vector3[numPoints * 2];
        Vector2[] uvs = new Vector2[numPoints * 2];
        for (int i = 0; i < numPoints; ++i)
        {
            Vector3 v = geometry[i];
            vertices[i] = new Vector3(v.x, bottomY, v.z);
            vertices[i + numPoints] = new Vector3(v.x, topY, v.z);
            uvs[i] = new Vector2((float)i / (numPoints - 1), 0.0f);
            uvs[i + numPoints] = new Vector2(uvs[i].x, 1.0f);
        }

        int[] triangles = new int[(numPoints - 1) * 2 * 3];
        for (int i = 0; i < numPoints - 1; ++i)
        {
            // the geometry is built clockwised. only the back faces should be rendered in the camera frame mask
            triangles[i * 6 + 0] = i;
            triangles[i * 6 + 1] = i + numPoints;
            triangles[i * 6 + 2] = i + 1 + numPoints;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + 1 + numPoints;
            triangles[i * 6 + 5] = i + 1;

            //triangles[i * 6 + 0] = i;
            //triangles[i * 6 + 1] = i + 1 + numPoints;
            //triangles[i * 6 + 2] = i + numPoints;

            //triangles[i * 6 + 3] = i;
            //triangles[i * 6 + 4] = i + 1;
            //triangles[i * 6 + 5] = i + 1 + numPoints;
        }

        Mesh cylinderMesh = new Mesh();
        cylinderMesh.vertices = vertices;
        cylinderMesh.uv = uvs;
        cylinderMesh.triangles = triangles;

        return cylinderMesh;

    }
}
