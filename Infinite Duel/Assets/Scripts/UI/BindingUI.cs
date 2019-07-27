using System.Collections;
using System.Collections.Generic;
using Duel.Input;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    public class BindingUI : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        [Rewired.ActionIdProperty(typeof(Duel.Input.Constants.Action))]
        public int actionID;

        public Rewired.AxisRange axisRange;

        [SerializeField]
        private TMPro.TextMeshProUGUI bindingText;

        private int elementMapId;

        [Rewired.PlayerIdProperty(typeof(Duel.Input.Constants.Player))]
        public int targetPlayer;

        private ControlConfigManager manager;

        public void Initialize(ControlConfigManager manager)
        {
            this.manager = manager;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            bindingText.color = Color.white;
        }

        public void OnSelect(BaseEventData eventData)
        {
            bindingText.color = ControlConfigManager.Singleton.playerColors[targetPlayer];
        }

        public void OnSubmit(BaseEventData eventData)
        {
            manager.StartRebinding(targetPlayer, actionID, axisRange, elementMapId);
        }

        public void Refresh()
        {
            bindingText.text = manager?.GetControllerElementForAction(targetPlayer, actionID, axisRange, out elementMapId);
            if (string.IsNullOrEmpty(bindingText.text))
                bindingText.text = "Unbound";
        }
    }
}