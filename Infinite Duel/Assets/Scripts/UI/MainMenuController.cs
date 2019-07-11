using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Duel.UI
{
    public class MainMenuController : MonoBehaviour
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

        private bool returnFrame;

        // Start is called before the first frame update
        private void Start()
        {
            ReInput.players.GetPlayer(Input.Constants.Player.Player0).AddInputEventDelegate(OnReceivedInput, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
            SwapToTitle();
        }

        private void OnReceivedInput(InputActionEventData obj)
        {
            if (obj.actionId == Input.Constants.Action.Return)
            {
                ReturnToPreviousScreen();
            }
        }

        private void OnDisable()
        {
            //ReInput.players.GetPlayer(Input.Constants.Player.Player0).RemoveInputEventDelegate(OnReceivedInput);
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
            currentScreen = screen;

            SwapToScreen((int)currentScreen);
        }

        public void SwapToScreen(int screen)
        {
            currentScreen = (MenuScreen)screen;
            audioManager.SwapToSong(screens[screen].themeIndex);
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Swap(i == screen);
            }
        }

        private void SwapToTitle()
        {
            currentScreen = MenuScreen.Title;
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Swap(i == (int)currentScreen);
            }
        }
    }
}

[System.Serializable]
public struct MenuScreenData
{
    public GameObject screenObject;
    public UnityEngine.UI.Selectable selectionStart;
    public int themeIndex;

    public void Swap(bool active)
    {
        screenObject.SetActive(active);
        if (active)
            selectionStart.Select();
    }
}