using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuController : MonoBehaviour
{
    public VisualElement ui;

    public Button playButton;
    public Button exitButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        playButton = ui.Q<Button>("PlayButton");
        playButton.clicked += OnPlayButtonClicked;
        
        exitButton = ui.Q<Button>("ExitButton");
        exitButton.clicked += OnExitButtonClicked;
    }
    
    private void OnDisable()
    {
        playButton.clicked -= OnPlayButtonClicked;
        exitButton.clicked -= OnExitButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        SudokuGameSceneManager sceneManager = FindFirstObjectByType<SudokuGameSceneManager>();
        sceneManager.LoadPuzzleSelectScene();
    }

    private void OnExitButtonClicked()
    {
        var startScreen = FindFirstObjectByType<StartScreen>();
        startScreen.OnQuitButtonPressed();
    }
}
