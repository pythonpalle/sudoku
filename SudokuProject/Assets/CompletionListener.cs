using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompletionListener : MonoBehaviour
{
    public UnityEvent OnComplete;

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
    }
}
