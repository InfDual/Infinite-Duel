using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public struct SetFieldValueCommand : ICommand
{
    public object targetObject;
    public FieldInfo targetField;
    public object value;
    public object originalValue;

    public SetFieldValueCommand(object targetObject, string targetFieldName, object value)
    {
        this.targetObject = targetObject;
        this.targetField = targetObject.GetType().GetField(targetFieldName);
        this.value = value;
        this.originalValue = targetField.GetValue(targetObject);
    }

    public void Do()
    {
        targetField.SetValue(targetObject, value);
    }

    public void Undo()
    {
        targetField.SetValue(targetObject, originalValue);
    }
}

public struct SetPropertyValueCommand : ICommand
{
    public object targetObject;
    public PropertyInfo targetProperty;
    public object value;
    public object originalValue;

    public SetPropertyValueCommand(object targetObject, string targetFieldName, object value)
    {
        this.targetObject = targetObject;
        this.targetProperty = targetObject.GetType().GetProperty(targetFieldName);
        this.value = value;
        this.originalValue = targetProperty.GetValue(targetObject);
    }

    public void Do()
    {
        targetProperty.SetValue(targetObject, value);
    }

    public void Undo()
    {
        targetProperty.SetValue(targetObject, originalValue);
    }
}