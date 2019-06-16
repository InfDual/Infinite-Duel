using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Duel.Input;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    public class BindingUI : MonoBehaviour, ISubmitHandler
    {
        [SerializeField]
        private TextMeshProUGUI bindingText;

        [SerializeField]
        private PlayerInput boundInput;

        private Player currentBindingSet;

        private ConfigUIController configController;

        public void Initialize(ConfigUIController configUI)
        {
            configController = configUI;
        }

        public void SetBindingSet(Player bindingSet)
        {
            currentBindingSet = bindingSet;
        }

        public void RefreshBindingUI()
        {
            if (currentBindingSet == Player.PlayerOne)
            {
                bindingText.text = InputManager.Instance.GetPlayerOneBinding(boundInput).ToString();
            }
            else
            {
                bindingText.text = InputManager.Instance.GetPlayerTwoBinding(boundInput).ToString();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            configController.StartListeningForInput(
                (KeyCode k) =>
                {
                    Input.InputManager.Instance.ChangeBinding(currentBindingSet, boundInput, k);
                    RefreshBindingUI();
                }
                );
        }
    }
}