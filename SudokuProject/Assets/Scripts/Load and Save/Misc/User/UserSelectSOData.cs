using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserEnum
{
    User0,
    User1,
    User2,
    User3
}

public class UserSelectSOData : MonoBehaviour
{
    [SerializeField] private UserEnum user;

    public void SelectUser(UserEnum selected)
    {
        user = selected;
    }
}
