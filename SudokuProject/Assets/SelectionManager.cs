using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public List<TileBehaviour> SelectedTiles { get; private set; } = new List<TileBehaviour>();
    
    public static SelectionManager Instance { get; private set; }

    public bool IsSelecting;// { get; private set; }


    private void Awake()
    {
        MakeSingleton();
    }

    private void OnEnable()
    {
        EventManager.OnTileSelect += OnTileSelect;
        EventManager.OnTileDeselect += OnTileDeselect;
    }
    
    private void OnDisable()
    {
        EventManager.OnTileSelect -= OnTileSelect;
        EventManager.OnTileDeselect -= OnTileDeselect;
    }

    private void OnTileSelect(TileBehaviour tile)
    {
        if (!SelectedTiles.Contains(tile))
        {
            SelectedTiles.Add(tile);
        }
    }

    private void OnTileDeselect(TileBehaviour tile)
    {
        if (SelectedTiles.Contains(tile))
        {
            SelectedTiles.Remove(tile);
        }
    }

    private void MakeSingleton()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            Debug.Log("Instance exists, destory");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Debug.Log("Instance doesn't exist, make singleton");
        }
    }

    private void Update()
    {
        HandleSetSelect();
    }

    private void HandleSetSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsSelecting = true;
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            IsSelecting = false;
        }
    }
}
