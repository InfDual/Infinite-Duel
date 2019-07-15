using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    public class ControllerConfigNavigator : MonoBehaviour
    {
        private Player playerOne;
        private Player playerTwo;

        private ConfigSelectable playerOneSelectedElement;
        private ConfigSelectable playerTwoSelectedElement;

        [SerializeField]
        private ConfigSelectable playerOneInitialSelectable;

        [SerializeField]
        private ConfigSelectable playerTwoInitialSelectable;

        [SerializeField]
        private MonoBehaviour rewiredUIManager;

        private void OnEnable()
        {
            rewiredUIManager.enabled = false;

            playerOne = ReInput.players.GetPlayer(Input.Constants.Player.Player0);
            playerTwo = ReInput.players.GetPlayer(Input.Constants.Player.Player1);

            playerOne.AddInputEventDelegate(OnPlayerOneInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
            playerTwo.AddInputEventDelegate(OnPlayerTwoInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);

            playerOneSelectedElement = playerOneInitialSelectable;
            playerTwoSelectedElement = playerTwoInitialSelectable;

            playerOneSelectedElement.Select();
            playerTwoSelectedElement.Select();
        }

        private void OnDisable()
        {
            playerOne.RemoveInputEventDelegate(OnPlayerOneInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
            playerTwo.RemoveInputEventDelegate(OnPlayerTwoInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);

            if (rewiredUIManager != null)
                rewiredUIManager.enabled = true;
        }

        private void OnPlayerOneInput(InputActionEventData obj)
        {
            if (obj.actionId == Input.Constants.Action.Vertical)
            {
                playerOneSelectedElement.Deselect();
                print($"Vertical : {obj.GetAxis()}");
                if (obj.GetAxis() > 0)
                    playerOneSelectedElement = playerOneSelectedElement.OnUpSelectable ?? playerOneSelectedElement;
                else if (obj.GetAxis() < 0)
                    playerOneSelectedElement = playerOneSelectedElement.OnDownSelectable ?? playerOneSelectedElement;

                playerOneSelectedElement.Select();
            }
            else if (obj.actionId == Input.Constants.Action.Horizontal)
            {
                print($"Horizontal : {obj.GetAxis()}");
                playerOneSelectedElement.Deselect();

                if (obj.GetAxis() > 0)
                    playerOneSelectedElement = playerOneSelectedElement.OnRightSelectable ?? playerOneSelectedElement;
                else if (obj.GetAxis() < 0)
                    playerOneSelectedElement = playerOneSelectedElement.OnLeftSelectable ?? playerOneSelectedElement;
                playerOneSelectedElement.Select();
            }
            else if (obj.actionId == Input.Constants.Action.Accept)
            {
                playerOneSelectedElement.Submit();
            }
            else if (obj.actionId == Input.Constants.Action.Return)
            {
                playerOneSelectedElement.Cancel();
            }
        }

        private void OnPlayerTwoInput(InputActionEventData obj)
        {
            if (obj.actionId == Input.Constants.Action.Vertical)
            {
                playerTwoSelectedElement.Deselect();

                if (obj.GetAxis() > 0)
                    playerTwoSelectedElement = playerTwoSelectedElement.OnUpSelectable ?? playerTwoSelectedElement;
                else if (obj.GetAxis() < 0)
                    playerTwoSelectedElement = playerTwoSelectedElement.OnDownSelectable ?? playerTwoSelectedElement;
                playerTwoSelectedElement.Select();
            }
            else if (obj.actionId == Input.Constants.Action.Horizontal)
            {
                playerTwoSelectedElement.Deselect();

                if (obj.GetAxis() > 0)
                    playerTwoSelectedElement = playerTwoSelectedElement.OnRightSelectable ?? playerTwoSelectedElement;
                else if (obj.GetAxis() < 0)
                    playerTwoSelectedElement = playerTwoSelectedElement.OnLeftSelectable ?? playerTwoSelectedElement;
                playerTwoSelectedElement.Select();
            }
            else if (obj.actionId == Input.Constants.Action.Accept)
            {
                playerTwoSelectedElement.Submit();
            }
            else if (obj.actionId == Input.Constants.Action.Return)
            {
                playerTwoSelectedElement.Cancel();
            }
        }
    }
}