using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
    public HexGrid hexGrid;

    private Color activeColor;

    bool applyWaterLevel = true;
    bool applyElevation = true;
    bool applyColor = true;

    int activeElevation;
    int activeWaterLevel;
    int brushSize;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    OptionalToggle riverMode, roadMode;

    private void Awake()
    {
        SelectColor(0);
    }

    public void SetBrushSize(float bs) { brushSize = (int)bs; }
    public void SetApplyElevation(bool toggle) { applyElevation = toggle; }
    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor == true)
            activeColor = colors[index];
    }

    public void SetElevation(float new_elevation) { activeElevation = (int)new_elevation; }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    void EditCell(HexCell cell)
    {
        if (cell != null)
        {
            if (applyColor == true)
                cell.Color = activeColor;
            if (applyElevation == true)
                cell.Elevation = activeElevation;
            if (applyWaterLevel)
                cell.WaterLevel = activeWaterLevel;
            if (riverMode == OptionalToggle.No)
                cell.RemoveRiver();
            if (roadMode == OptionalToggle.No)
                cell.RemoveRoads();
            if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    if (riverMode == OptionalToggle.Yes)
                        otherCell.SetOutgoingRiver(dragDirection);
                    if (roadMode == OptionalToggle.Yes)
                        otherCell.AddRoad(dragDirection);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
        else
            previousCell = null;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void SetRiverMode(int mode) { riverMode = (OptionalToggle)mode; }

    public void SetRoadMode(int mode) { roadMode = (OptionalToggle)mode; }

    void ValidateDrag(HexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        )
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }
}
