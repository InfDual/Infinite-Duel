using System.Collections;
using System.Collections.Generic;
using Duel.Input;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    [RequireComponent(typeof(Selectable))]
    public class BindingUI : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        [Rewired.ActionIdProperty(typeof(Duel.Input.Constants.Action))]
        public int actionID;

        private Selectable selectable;

        public Rewired.AxisRange axisRange;

        [SerializeField]
        private TMPro.TextMeshProUGUI nameText;

        [SerializeField]
        private TMPro.TextMeshProUGUI bindingText;

        private int elementMapId;

        private ControlConfigManager manager;

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        public void Initialize(ControlConfigManager manager)
        {
            this.manager = manager;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            nameText.color = bindingText.color = Color.white;
        }

        public void OnSelect(BaseEventData eventData)
        {
            nameText.color = bindingText.color = selectable.colors.selectedColor;
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