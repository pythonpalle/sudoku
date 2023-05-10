using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NumberButton : MonoBehaviour
{
    [SerializeField] private int number;

    public UnityEvent<int> OnNumberButtonPressed;
    public UnityEvent<int> OnNumberButtonHover;

    public void TryEnterDigit()
    {
        OnNumberButtonPressed?.Invoke(number);
    }
}
