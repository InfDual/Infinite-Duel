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
        private Audio.AudioManager audioManager;

        [SerializeField]
        [Tooltip("Title, Mode Select, Config")]
        private MenuScreenData[] screens;

        // Start is called before the first frame update
        private void Start()
        {
            SwapToScreen(MenuScreen.Title);
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
            audioManager.SwapToSong(screens[screen].themeIndex);
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
    public int themeIndex;

    public void Swap(bool active)
    {
        screenObject.SetActive(active);
        if (active)
            selectionStart.Select();
    }
}