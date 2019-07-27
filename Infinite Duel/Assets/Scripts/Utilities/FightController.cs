﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using Actions = Duel.Input.Constants.Action;

namespace Duel
{
    public class FightController : SerializedMonoBehaviour
    {
        [ShowInInspector]
        private bool paused;

        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private TMPro.TextMeshProUGUI playerPauseText;

        [SerializeField]
        private UI.ConfigSelectable initialSelectable;

        private UI.ConfigSelectable currentlySelectedObj;

        private Player[] players = new Player[2];

        private int pausingPlayer;

        private void OnEnable()
        {
            for (int i = 0; i < 2; i++)
            {
                players[i] = ReInput.players.GetPlayer(i);
                players[i].AddInputEventDelegate(OnInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
                players[i].AddInputEventDelegate(OnPause, UpdateLoopType.Update, InputActionEventType.ButtonJustPressedForTime, Actions.Pause, new object[] { 2f });
            }
        }

        private void OnPause(InputActionEventData obj)
        {
            if (obj.actionId == Actions.Pause)
            {
                pausingPlayer = obj.playerId;
                TogglePause();
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < 2; i++)
            {
                players[i]?.RemoveInputEventDelegate(OnInput);
                players[i]?.RemoveInputEventDelegate(OnPause);
            }
        }

        private void OnInput(InputActionEventData obj)
        {
            if (!paused)
                return;

            if (obj.actionId == Actions.Vertical)
            {
                currentlySelectedObj.Deselect();
                if (obj.GetAxis() > 0)
                    currentlySelectedObj = currentlySelectedObj.OnUpSelectable ?? currentlySelectedObj;
                else if (obj.GetAxis() < 0)
                    currentlySelectedObj = currentlySelectedObj.OnDownSelectable ?? currentlySelectedObj;

                currentlySelectedObj.Select();
            }
            else if (obj.actionId == Actions.Accept)
            {
                currentlySelectedObj.Submit();
            }
        }

        public void TogglePause()
        {
            paused = !paused;
            Time.timeScale = paused ? 0 : 1;
            pauseMenu.SetActive(paused);
            if (paused)
            {
                playerPauseText.text = pausingPlayer == 0 ? "Player One Paused" : "Player Two Paused";
                currentlySelectedObj?.Deselect();
                currentlySelectedObj = initialSelectable;
                currentlySelectedObj.Select();
            }
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1;

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}