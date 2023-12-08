using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class GoBackToSelectBehaviour : MonoBehaviour
{
    [SerializeField] private GeneratorPort _generatorPort;
    [SerializeField] private SudokuGameSceneManager _sceneManager;
    
    public void OnGoBackButtonPressed()
    {
        if (_generatorPort.isGenerating)
        {
            _sceneManager.LoadPuzzleSelectScene();
        }
        else
        {
            SaveManager.TrySave(SaveRequestLocation.ExitGameButton, false);
        }
    }
}
