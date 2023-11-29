using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportBehaviour : MonoBehaviour
{
    [SerializeField] private ImportObject importObject;
    [SerializeField] private InputField inputField;

    public void OnInputEnter(string seedString)
    {
        Debug.Log("Seed: " + seedString);
    }
}
