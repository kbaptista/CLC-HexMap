using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexEdgeType
{
    Flat,
    Slope,
    Cliff
}

public static class HexEdgeTypeExtensions
{
    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        HexEdgeType res;

        if (elevation1 == elevation2)
            res = HexEdgeType.Flat;
        else if (Mathf.Abs(elevation1 - elevation2) <= 1f)
            res = HexEdgeType.Slope;
        else
            res = HexEdgeType.Cliff;

        return res;
    }
}

