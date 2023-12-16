using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleSelect
{
    public class SelectListLoader : MonoBehaviour
    {
        [SerializeField] private PuzzleSelectList _selectList;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            _selectList.gameObject.SetActive(scene.name == SudokuGameSceneManager.puzzleSelectSceneName);
        }
    }

}

