// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class NewPuzzleCreationSetting : MenuOption
// {
//     // puzzle display setting: carousel, toggle, dropdown
//     public enum CreationDisplayType
//     {
//         None,
//         Toggle,
//         Dropdown,
//     }
//
//     private float startScale = 1f;
//     [SerializeField] float selectScale = 0.9f;
//     
//     public MenuOptionHolder menuOptionHolder;
//
//     public void OnEnable()
//     {
//         startScale = transform.localScale.x;
//     }
//
//
//     
//     public List<NewPuzzleSettingOption> Options = new List<NewPuzzleSettingOption>(); 
//     public CreationDisplayType DisplayType = CreationDisplayType.Toggle;
//     
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
//
//     // Switch to / switch from?
//     public override void Select()
//     {
//         transform.localScale = new Vector3(selectScale, selectScale, selectScale);
//     }
//
//     public override void SelectStart()
//     {
//         Select();
//     }
//
//     public override void Deselect()
//     {
//         transform.localScale = new Vector3(startScale, startScale, startScale);
//     }
// }