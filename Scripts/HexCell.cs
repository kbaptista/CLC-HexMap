using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;

    private Color color;
    int elevation = int.MinValue;

    public int Elevation
    {
        get { return elevation; }
        set
        {
            if (elevation != value)
            {
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                                HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;

                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                Refresh();
            }
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            if (color != value)
            {
                color = value;
                Refresh();
            }
        }
    }

    [SerializeField] HexCell[] neighbors;

    private void Awake()
    {
        neighbors = new HexCell[Enum.GetNames(typeof(HexDirection)).Length];
    }

    void Refresh()
    {
        if (chunk != null)
        {
            chunk.Refresh();
            for(int i = 0; i < neighbors.Length; ++i)
            {
                HexCell nei = neighbors[i];
                if(nei != null && nei.chunk != chunk)
                {
                    nei.chunk.Refresh();
                }
            }
        }
    }

    public HexCell GetNeighbor(HexDirection dir) { return neighbors[(int)dir]; }
    public void SetNeighbor(HexDirection dir, HexCell new_neighbor)
    {
        neighbors[(int)dir] = new_neighbor;
        new_neighbor.neighbors[(int)dir.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexEdgeTypeExtensions.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexEdgeTypeExtensions.GetEdgeType(elevation, otherCell.elevation);
    }
}
