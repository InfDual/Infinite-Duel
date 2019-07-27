using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using Actions = Duel.Input.Constants.Action;

namespace Duel.UI
{
    public class MainMenuController : SerializedMonoBehaviour
    {
        public enum MenuScreen
        {
            Title,
            ModeSelect,
            Config
        }

        [SerializeField]
        private MenuScreen currentScreen;

        [SerializeField]
        private Audio.AudioManager audioManager;

        [SerializeField]
        [Tooltip("Title, Mode Select, Config")]
        private MenuScreenData[] screens;

        public bool ManageInput
        {
            get; set;
        }

        [ShowInInspector]
        private UI.ConfigSelectable currentlySelectedObj;

        private bool returnFrame;

        private Player playerOne;

        // Start is called before the first frame update
        private void OnEnable()
        {
            ManageInput = true;
            playerOne = ReInput.players.GetPlayer(Input.Constants.Player.Player0);
            playerOne.AddInputEventDelegate(OnReceivedInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
            SwapToTitle();
        }

        private void OnDisable()
        {
            playerOne?.RemoveInputEventDelegate(OnReceivedInput);
        }

        private void OnReceivedInput(InputActionEventData obj)
        {
            if (obj.actionId == Actions.Return)
            {
                ReturnToPreviousScreen();
            }
            if (!ManageInput)
                return;

            if (obj.actionId == Actions.Vertical)
            {
                currentlySelectedObj?.Deselect();

                if (currentlySelectedObj != null)
                {
                    if (obj.GetAxis() > 0)
                        currentlySelectedObj = currentlySelectedObj.OnUpSelectable ?? currentlySelectedObj;
                    else if (obj.GetAxis() < 0)
                        currentlySelectedObj = currentlySelectedObj.OnDownSelectable ?? currentlySelectedObj;
                }

                currentlySelectedObj?.Select();
            }
            else if (obj.actionId == Actions.Accept)
            {
                currentlySelectedObj?.Submit();
            }
        }

        private void Update()
        {
            if (currentScreen == MenuScreen.Title)
            {
                if (UnityEngine.Input.anyKeyDown && !returnFrame)
                    SwapToScreen(MenuScreen.ModeSelect);
            }
            returnFrame = false;
        }

        public void ReturnToPreviousScreen()
        {
            returnFrame = true;
            switch (currentScreen)
            {
                case MenuScreen.Title:
                    currentScreen = MenuScreen.Title;
                    break;

                case MenuScreen.ModeSelect:
                    currentScreen = MenuScreen.Title;

                    break;

                case MenuScreen.Config:
                    currentScreen = MenuScreen.ModeSelect;

                    break;
            }
            SwapToScreen(currentScreen);
        }

        public void SwapToScreen(MenuScreen screen)
        {
            SwapToScreen((int)screen);
        }

        public void SwapToScreen(int screen)
        {
            currentScreen = (MenuScreen)screen;
            audioManager.SwapToSong(screens[screen].themeIndex);
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Swap(i == screen);
            }
            currentlySelectedObj?.Deselect();
            currentlySelectedObj = screens[screen].initialSelectable;
            currentlySelectedObj?.Select();
        }

        private void SwapToTitle()
        {
            currentScreen = MenuScreen.Title;
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Swap(i == (int)currentScreen);
            }
            currentlySelectedObj?.Deselect();
            currentlySelectedObj = screens[(int)MenuScreen.Title].initialSelectable;
            currentlySelectedObj?.Select();
        }
    }
}

[System.Serializable]
public struct MenuScreenData
{
    public GameObject screenObject;
    public int themeIndex;
    public Duel.UI.ConfigSelectable initialSelectable;

    public void Swap(bool active)
    {
        screenObject.SetActive(active);
    }
}