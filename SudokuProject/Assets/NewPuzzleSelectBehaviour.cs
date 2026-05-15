using UnityEngine;

public class NewPuzzleSelectBehaviour : MonoBehaviour
{
    public void CreateOwnPuzzle()
    {
        SudokuGameSceneManager.instance.LoadCreateOwnScene();
    }
}
