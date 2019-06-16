using System.Collections;
using System.Collections.Generic;
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
        [Tooltip("Title, Mode Select, Config")]
        private MenuScreenData[] screens;

        // Start is called before the first frame update
        private void Start()
        {
            SwapToScreen(MenuScreen.Title);
            Input.InputManager.Instance.PlayerOneInputDownEventHandler += OnPlayerOneInput;
        }

        private void OnPlayerOneInput(Input.PlayerInput input)
        {
            if (input == Input.PlayerInput.GoBack)
            {
                ReturnToPreviousScreen();
            }
        }

        private void OnDestroy()
        {
            Input.InputManager.Instance.PlayerOneInputDownEventHandler -= OnPlayerOneInput;
        }

        public void ReturnToPreviousScreen()
        {
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

            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].Swap(i == screen);
            }
        }
    }
}

[System.Serializable]
public struct MenuScreenData
{
    public GameObject screenObject;
    public UnityEngine.UI.Selectable selectionStart;

    public void Swap(bool active)
    {
        screenObject.SetActive(active);
        if (active)
            selectionStart.Select();
    }
}