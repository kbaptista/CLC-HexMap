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

    bool applyElevation = true;
    bool applyColor = true;

    private int activeElevation;
    private int brushSize;

    private void Awake()
    {
        SelectColor(0);
    }

    public void SetBrushSize(float bs) { brushSize = (int)bs; }
    public void SetApplyElevation(bool toggle) { applyElevation = toggle; }


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
            EditCells(hexGrid.GetCell(hit.point));
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
}
