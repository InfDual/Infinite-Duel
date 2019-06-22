using System.Collections;
using System.Collections.Generic;
using Duel.Input;
using UnityEngine;

namespace Duel.UI
{
    public class BindingUI : MonoBehaviour
    {
        [Rewired.ActionIdProperty(typeof(Duel.Input.Constants.Action))]
        public int actionID;

        public Rewired.AxisRange axisRange;

        [SerializeField]
        private TMPro.TextMeshProUGUI bindingText;

        private ControlConfigManager manager;

        public void Initialize(ControlConfigManager manager)
        {
            this.manager = manager;
        }

        public void Refresh()
        {
            bindingText.text = manager.GetControllerElementNameForAction(actionID);
        }
    }
}