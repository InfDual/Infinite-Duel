using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CommandManager : SerializedMonoBehaviour
{
    [SerializeField]
    private Stack<ICommand> undoStack = new Stack<ICommand>();

    [SerializeField]
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (undoStack.Count > 0)
                {
                    ICommand command = undoStack.Pop();
                    command.Undo();
                    redoStack.Push(command);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                if (redoStack.Count > 0)
                {
                    ICommand command = redoStack.Pop();
                    command.Do();
                    undoStack.Push(command);
                }
            }
        }
    }

    public void PerformCommand(ICommand commandToPerform)
    {
        commandToPerform.Do();
        undoStack.Push(commandToPerform);
    }
}