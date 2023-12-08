using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenereratorTypeHolder : MonoBehaviour
{
    public static GenereratorTypeHolder instance { get; private set; }
    [SerializeField] private GeneratorPort generatorPort;

    private void Awake()
    {
        instance = this;
    }

    public new GridGenerationType GetType()
    {
        return generatorPort.GenerationType;
    }
}
