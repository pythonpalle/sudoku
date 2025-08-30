using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoFillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] private GridPort gridPort;

    private SudokuGenerator9x9 fillGenerator;
    private SudokuGrid9x9 autoFilledGrid;

    public void AutoFill()
    {
        if (gridPort.GridStatus == GridStatus.MultipleSolutions)
            EventManager.AutoFill(autoFilledGrid);
    }
    
    private IEnumerator AutoFillPreviewRoutine()
    {
        Debug.Log("Creating suggestion");

        if (gridPort.GridStatus == GridStatus.MultipleSolutions)
        {
            gridPort.RequestGrid();
            
            fillGenerator = new SudokuGenerator9x9(PuzzleDifficulty.Easy, gridPort.grid);
            // SudokuGrid9x9 fillGrid = fillGenerator.GetRandomCompleteGrid();
            // gridSolver.HumanlySolvable(grid, out _);

            yield return fillGenerator.Generate(PuzzleDifficulty.Easy, gridPort.grid);
            yield return new WaitUntil(() => fillGenerator.IsGenerating == false);
            
            Debug.Log("Suggested grid:");
            autoFilledGrid = new SudokuGrid9x9(fillGenerator.Grid);
            Debug.Log(autoFilledGrid);

            EventManager.AutoFillPreview(autoFilledGrid);
        }
        
        /*

        1. hämta referens för nuvarande grid

        2. om grid är löst/ har en lösning, gör inget

        3. om grid har flera lösningar:
        4. skapa en kopia av grid
        5. gör ett färdigt pussel utifrån det (easy svårighetsgrad)
        6. printa ut (sen animera, koppla till knapp)
        7. när hovrar över knappen, kör denna logik
        8. loopa över brädet, tillfälligt fyll i rutorna med den tillfälliga siffran
            behöver ny bool "suggested" för att inte påverka annan logik
        9. spara senaste versionen så att man inte generar en ny för varje hover?
        10. när trycker, kör save logik (här behöver alla _suggested boolsen nollställas

        3b. om grid har 0 lösningar
        4. skapa kopia av grid
        5. gör undos till det har flera lösningar
        6. gör ett fullständigt pussel



          */
        
        
        
        Debug.LogWarning("not created");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(AutoFillPreviewRoutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.RemoveAutoFillPreview(autoFilledGrid);
    }
}
