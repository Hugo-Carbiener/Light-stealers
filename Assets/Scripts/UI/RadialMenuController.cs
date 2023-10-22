using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using System;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPlain;
    [SerializeField] private GameObject menuForest;
    [SerializeField] private GameObject menuMountain;
    private GameObject menu;
    private RadialMenuOptions menuOptions;
    [SerializeField] private Transform overlay;
    private Vector3[] overlayPositions = { new Vector3(-4.34f, -4.34f, 0), new Vector3(-4.34f, 4.34f, 0), new Vector3(4.34f, 4.34f, 0), new Vector3(4.34f, -4.34f, 0) };
    [SerializeField] private GameObject[] imageContainer;
    [SerializeField] private Tilemap selectionTilemap;
    private Vector2Int fixedPosition;
    private Vector2 moveInput;
    private int selectedOption;
    private bool menuIsOpened;
    private Camera cam;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!menuPlain) { menuPlain = GameObject.Find("Radial Menu Plain") as GameObject; }
        if (!menuForest) { menuForest = GameObject.Find("Radial Menu Forest") as GameObject; }
        if (!menuMountain) { menuMountain = GameObject.Find("Radial Menu Mountain") as GameObject; }
        if(!overlay) { overlay = (GameObject.Find("Overlay")).transform; }
        menuPlain.SetActive(false);
        menuForest.SetActive(false);
        menuMountain.SetActive(false);
        menu = menuPlain;
        menuOptions = menu.GetComponent<RadialMenuOptions>();
        CloseMenu();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (menu.activeInHierarchy)
        {
            moveInput.x = Input.mousePosition.x - (cam.WorldToScreenPoint(selectionTilemap.CellToWorld((Vector3Int) fixedPosition)).x);
            moveInput.y = Input.mousePosition.y - (cam.WorldToScreenPoint(selectionTilemap.CellToWorld((Vector3Int) fixedPosition)).y);

            moveInput.Normalize();


            if (moveInput != Vector2.zero)
            {
                float angle = Mathf.Atan2(moveInput.y, -moveInput.x) / Mathf.PI * 180;
                angle += 90;
                if (angle < 0)
                {
                    angle += 360;
                }
                for (int i = 0; i < imageContainer.Length; i++)
                {

                    if (angle > i * 90 && angle < (i + 1) * 90)
                    {
                        selectedOption = i;
                        overlay.position = selectionTilemap.CellToWorld((Vector3Int) fixedPosition);
                        overlay.localPosition += overlayPositions[i];
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                switch (selectedOption)
                {
                    case 0:
                        menuOptions.Option0();
                        break;
                    case 1:
                        menuOptions.Option1();
                        break;
                    case 2:
                        menuOptions.Option2();
                        break;
                    case 3:
                        menuOptions.Option3();
                        break;
                }
                CloseMenu();
            }
        }
    }

    private void FixedUpdate()
    {
        if (menu.activeInHierarchy)
        {
            UpdateScale();
            transform.position = selectionTilemap.CellToWorld((Vector3Int) fixedPosition);
        }
    }

    public void OpenMenu(Vector2Int pos, environments env)
    {
        switch(env)
        {
            case environments.plain:
                menu = menuPlain;
                break;
            case environments.forest:
                menu = menuForest;
                break;
            case environments.mountain:
                menu = menuMountain;
                break;
        }
        menuOptions = menu.GetComponent<RadialMenuOptions>();
        fixedPosition = pos;
        transform.position = selectionTilemap.CellToWorld((Vector3Int) fixedPosition);
        menu.SetActive(true);
        overlay.gameObject.SetActive(true);
        menuIsOpened = true;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        overlay.gameObject.SetActive(false);
        menuIsOpened = false;
    }

    private void UpdateScale()
    {
        float factor = cam.orthographicSize * 0.032f + 0.263f;
        transform.localScale = Vector3.one * factor;
    }

    public bool MenuIsOpened() { return this.menuIsOpened; }
}
