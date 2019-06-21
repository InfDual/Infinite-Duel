using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandHelper : MonoBehaviour
{
    [SerializeField]
    private CommandManager manager;

    public void PerformSetStringFieldValueCommand(Object target, string fieldName, string value)
    {
        manager.PerformCommand(CommandFactory.CreateSetFieldValueCommand(target, fieldName, value));
    }

    public void PerformSetStringPropertyValueCommand(Object target, string fieldName, string value)
    {
        manager.PerformCommand(CommandFactory.CreateSetPropertyValueCommand(target, fieldName, value));
    }
}