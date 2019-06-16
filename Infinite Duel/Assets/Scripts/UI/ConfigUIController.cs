using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UInput = UnityEngine.Input;

namespace Duel.UI
{
    public class ConfigUIController : MonoBehaviour
    {
        [SerializeField]
        private Input.Player currentPlayerBindingSet;

        [SerializeField]
        private BindingUI[] bindingUIs;

        [SerializeField]
        private BaseInputModule inputModule;

        [SerializeField]
        private TextMeshProUGUI playerOneControlSetIndicator;

        [SerializeField]
        private TextMeshProUGUI playerTwoControlSetIndicator;

        [SerializeField]
        private Color activeSetColor;

        [SerializeField]
        private Color unactiveSetColor;

        private void Start()
        {
            foreach (var item in bindingUIs)
            {
                item.Initialize(this);
            }
        }

        private void OnEnable()
        {
            SetPlayerBindingSet(0);
        }

        public void ResetCurrentBindings()
        {
            if (currentPlayerBindingSet == Input.Player.PlayerOne)
            {
                Input.InputManager.Instance.ResetPlayerOneBindings();
            }
            else
            {
                Input.InputManager.Instance.ResetPlayerTwoBindings();
            }
        }

        public void SetPlayerBindingSet(int bindingSet)
        {
            if (bindingSet == 0)
            {
                playerOneControlSetIndicator.color = activeSetColor;
                playerTwoControlSetIndicator.color = unactiveSetColor;
            }
            else
            {
                playerOneControlSetIndicator.color = unactiveSetColor;
                playerTwoControlSetIndicator.color = activeSetColor;
            }
            currentPlayerBindingSet = (Input.Player)bindingSet;
            RefreshUI();
        }

        internal void StartListeningForInput(Action<KeyCode> onChooseKey)
        {
            inputModule.enabled = false;
            StartCoroutine(ListenForInput(onChooseKey));
        }

        private IEnumerator ListenForInput(Action<KeyCode> onChooseKey)
        {
            yield return null;
            //Skip a frame so it doesn't count the enter key
            yield return new WaitUntil(() =>
            {
                Debug.Log(UInput.anyKeyDown);
                return UInput.anyKeyDown;
            });
            Debug.Log("Key Pressed : " + Input.InputManager.Instance.KeyDown);
            onChooseKey(Input.InputManager.Instance.KeyDown);
            inputModule.enabled = true;
        }

        private void RefreshUI()
        {
            foreach (var item in bindingUIs)
            {
                item.SetBindingSet(currentPlayerBindingSet);
                item.RefreshBindingUI();
            }
        }
    }
}