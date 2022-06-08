using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ScanLineFillHelper
{
    public static List<int> ScaneLine(Func<int, bool> conditionFunc, List<int> edgeIndices)
    {
        List<int> fillIndices = new List<int>();
        for (int i = 0; i < (PlayAreaConstant.GRID_SIZE + 1); i++)
        {
            bool lastCondition = false;
            int startIndex = -1;
            int endIndex = -1;
            for (int j = 0; j < (PlayAreaConstant.GRID_SIZE + 1); j++)
            {
                int index = i * (PlayAreaConstant.GRID_SIZE + 1) + j;

                //表示找到起点
                if (!conditionFunc(index) && lastCondition)
                {
                    if (!edgeIndices.Contains(index - 1))
                    {
                        startIndex = index;
                    }
                }

                //表示找到终点
                if (conditionFunc(index) && !lastCondition)
                {
                    if (startIndex != -1)
                    {
                        endIndex = index;
                        for (int k = startIndex; k < endIndex; k++)
                        {
                            fillIndices.Add(k);
                        }
                        startIndex = -1;
                    }
                }

                lastCondition = conditionFunc(index);
            }
        }
        return fillIndices;
    }
}
