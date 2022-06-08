using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyAreaEightNeighbourHelper
{
    private static List<int> edgeVertexIndexList = new List<int>();
    private static Dictionary<int, int> eightNeighboursDic = new Dictionary<int, int>()
    {
        { 0, 6},
        { 1, 6},
        { 2, 0},
        { 3, 0},
        { 4, 2},
        { 5, 2},
        { 6, 4},
        { 7, 4}
    };

    /// <summary>
    /// 1 2 3
    /// 0 x 4
    /// 7 6 5
    /// </summary>
    private static  Dictionary<int, Func<int, int>> eightNeighboursIndexCaculatorDic = new Dictionary<int, Func<int, int>>()
    {
        { 0, (index) => { return index - 1; } },
        { 1, (index) => { return index - 1 + PlayAreaConstant.GRID_SIZE + 1 ; } },
        { 2, (index) => { return index + PlayAreaConstant.GRID_SIZE + 1; } },
        { 3, (index) => { return index + 1 + PlayAreaConstant.GRID_SIZE + 1; } },
        { 4, (index) => { return index + 1; } },
        { 5, (index) => { return index + 1 - (PlayAreaConstant.GRID_SIZE + 1); } },
        { 6, (index) => { return index - (PlayAreaConstant.GRID_SIZE + 1); } },
        { 7, (index) => { return index - 1 - (PlayAreaConstant.GRID_SIZE + 1); } }
    };

    public static void EightNeighbours(int startCaculateIndex, Func<int, bool> conditionFunc, Action<List<int>> onGetEdgeCallback)
    {
        int firstRedColorIndex = FindFirstConditionIndex(startCaculateIndex, conditionFunc);
        edgeVertexIndexList.Clear();
        loopCount = 0;
        EightNeighboursRecursion(0, firstRedColorIndex, conditionFunc, onGetEdgeCallback);
    }

    private static int loopCount = 0;
    private static void EightNeighboursRecursion(int neighbourIndex, int vertexIndex, Func<int, bool> condition, Action<List<int>> onGetEdgeCallback)
    {
        loopCount++;
        if (loopCount > 5000)
        {
            Debug.LogError("Too much loop");
            onGetEdgeCallback?.Invoke(edgeVertexIndexList);
            return;
        }

        Func<int, int> checkIndexFunc = eightNeighboursIndexCaculatorDic[neighbourIndex];
        int checkIndex = checkIndexFunc(vertexIndex);
        if (checkIndex >= 0 && checkIndex <= ((PlayAreaConstant.GRID_SIZE + 1) * (PlayAreaConstant.GRID_SIZE + 1) - 1) && condition(checkIndex))
        {

            if (edgeVertexIndexList.Contains(checkIndex))
            {
                //for (int i = 0; i < edgeVertexIndexList.Count; i++)
                //{
                //    colors[edgeVertexIndexList[i]] = Color.yellow;
                //}
                //mesh.colors = colors;
                edgeVertexIndexList.Add(checkIndex);
                onGetEdgeCallback?.Invoke(edgeVertexIndexList);
                Debug.Log("LoopCount:" + loopCount);
                return;
            }
            else
            {
                edgeVertexIndexList.Add(checkIndex);
                int nextNeighbourIndex = eightNeighboursDic[neighbourIndex];
                EightNeighboursRecursion(nextNeighbourIndex, checkIndex, condition, onGetEdgeCallback);
            }
        }
        else
        {
            EightNeighboursRecursion((neighbourIndex + 1) % 8, vertexIndex, condition, onGetEdgeCallback);
        }
    }

    private static int FindFirstConditionIndex(int index, Func<int, bool> conditionFunc)
    {
        int rowIndex = index / (PlayAreaConstant.GRID_SIZE + 1);
        int startIndex = rowIndex * (PlayAreaConstant.GRID_SIZE + 1);
        if (index == startIndex)
        {
            return startIndex;
        }

        int currentIndex = 0;
        for (int i = 0; i < (PlayAreaConstant.GRID_SIZE + 1); i++)
        {
            currentIndex = startIndex + i;
            if (conditionFunc(currentIndex))
            {
                break;
            }
        }
        return currentIndex;
    }
}
