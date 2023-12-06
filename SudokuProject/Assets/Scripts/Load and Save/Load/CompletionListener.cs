using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CompletionListener : MonoBehaviour
{
    public UnityEvent OnComplete;
    private int completions = 0;

    [SerializeField] private TextMeshProUGUI victoryText;

    private void OnEnable()
    {
        EventManager.OnPuzzleComplete += OnPuzzleComplete;
    }
    
    private void OnDisable()
    {
        EventManager.OnPuzzleComplete -= OnPuzzleComplete;
    }

    private void OnPuzzleComplete()
    {
        OnComplete?.Invoke();
        completions++;

        if (completions > 1)
        {
            victoryText.fontSize = 45;
            victoryText.text = "wow, I guess you solved this one again...";
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
