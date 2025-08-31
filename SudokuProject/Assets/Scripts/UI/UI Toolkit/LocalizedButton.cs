using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

//using UnityEngine.UI;

[UxmlElement]
public partial class LocalizedButton : Button
{
    public static BindingId keyProperty = nameof(key);

    [UxmlAttribute] public string key;
    
    public LocalizedButton()
    {
        
    }
} 
