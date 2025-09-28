using System;
using System.Collections.Generic;
using UnityEngine;

public class NewPuzzleCreationSetting : NewPuzzleSetting
{
    // puzzle display setting: carousel, toggle, dropdown

    private float startScale = 1f;
    [SerializeField] float selectScale = 0.9f;

    public void OnEnable()
    {
        startScale = transform.localScale.x;
    }

    public enum CreationDisplayType
    {
        None,
        Toggle,
        Dropdown,
    }
    
    public List<NewPuzzleSettingOption> Options = new List<NewPuzzleSettingOption>(); 
    public CreationDisplayType DisplayType = CreationDisplayType.Toggle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Switch to / switch from?
    public override void Select()
    {
        transform.localScale = new Vector3(selectScale, selectScale, selectScale);
    }

    public override void SelectStart()
    {
        Select();
    }

    public override void Deselect()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
    }
}