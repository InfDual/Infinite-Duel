using System.Collections;
using System.Collections.Generic;
using Duel.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    public class BindingUI : MonoBehaviour, ISubmitHandler
    {
        [Rewired.ActionIdProperty(typeof(Duel.Input.Constants.Action))]
        public int actionID;

        public Rewired.AxisRange axisRange;

        [SerializeField]
        private TMPro.TextMeshProUGUI bindingText;

        private int elementMapId;

        private ControlConfigManager manager;

        public void Initialize(ControlConfigManager manager)
        {
            this.manager = manager;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            manager.StartRebinding(actionID, axisRange, elementMapId);
        }

        public void Refresh()
        {
            bindingText.text = manager.GetControllerElementForAction(actionID, axisRange, out elementMapId);
        }
    }
}