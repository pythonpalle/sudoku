using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnLoad : MonoBehaviour
{
    [SerializeField] private GeneratorPort GeneratorPort;
    [SerializeField] private List<GridGenerationType> typesToDeactivate;

    private void Awake()
    {
        foreach (var typeToDeactivate in typesToDeactivate)
        {
            if (typeToDeactivate == GeneratorPort.GenerationType)
                Destroy(gameObject);
        }
    }
}
