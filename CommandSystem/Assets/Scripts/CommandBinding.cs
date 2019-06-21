using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBinding : MonoBehaviour
{
    [SerializeField]
    private Object target;

    [SerializeField]
    private string valueName;

    private CommandHelper helper;

    public void SetStringFieldValue(string value)
    {
        if (helper == null)
            helper = FindObjectOfType<CommandHelper>();

        helper.PerformSetStringFieldValueCommand(target, valueName, value);
    }

    public void SetStringPropertyValue(string value)
    {
        if (helper == null)
            helper = FindObjectOfType<CommandHelper>();

        helper.PerformSetStringPropertyValueCommand(target, valueName, value);
    }
}