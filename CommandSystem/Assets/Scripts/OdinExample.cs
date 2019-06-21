using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class OdinExample : SerializedMonoBehaviour
{
    [SerializeField]
    private System.Action<Object, string, string> SetStringPropertyAction;

    [Button]
    public void Example(Object targetObject, string targetField, string value)
    {
        FindObjectOfType<CommandHelper>().PerformSetStringPropertyValueCommand(targetObject, targetField, value);
    }
}