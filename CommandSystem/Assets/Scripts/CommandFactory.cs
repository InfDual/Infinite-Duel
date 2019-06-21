using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommandFactory
{
    public static ICommand CreateSetFieldValueCommand(Object target, string fieldName, string value)
    {
        SetFieldValueCommand newCommand = new SetFieldValueCommand(target, fieldName, value);
        return newCommand;
    }

    public static ICommand CreateSetPropertyValueCommand(Object target, string fieldName, string value)
    {
        SetPropertyValueCommand newCommand = new SetPropertyValueCommand(target, fieldName, value);
        return newCommand;
    }
}