using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class ConfigSelectable : SerializedMonoBehaviour
    {
        [SerializeField]
        private int playerId = -1;

        [HideIf("playerId", -1)]
        [SerializeField]
        private TMPro.TextMeshProUGUI displayText;

        public ConfigSelectable OnUpSelectable;
        public ConfigSelectable OnDownSelectable;
        public ConfigSelectable OnLeftSelectable;
        public ConfigSelectable OnRightSelectable;

        private IDeselectHandler[] deselectHandlers;
        private ISelectHandler[] selectHandlers;
        private ISubmitHandler[] submitHandlers;
        private ICancelHandler[] cancelHandlers;

        private void Awake()
        {
            deselectHandlers = GetComponents<IDeselectHandler>();
            selectHandlers = GetComponents<ISelectHandler>();
            submitHandlers = GetComponents<ISubmitHandler>();
            cancelHandlers = GetComponents<ICancelHandler>();
        }

        public void Deselect()
        {
            if (playerId > -1)
                displayText.color = Color.white;
            for (int i = 0; i < deselectHandlers.Length; i++)
            {
                deselectHandlers[i].OnDeselect(new UnityEngine.EventSystems.BaseEventData(EventSystem.current));
            }
        }

        public void Select()
        {
            if (playerId > -1)
                displayText.color = ControlConfigManager.Singleton.playerColors[playerId];

            for (int i = 0; i < selectHandlers.Length; i++)
            {
                selectHandlers[i].OnSelect(new UnityEngine.EventSystems.BaseEventData(EventSystem.current));
            }
        }

        public void Submit()
        {
            for (int i = 0; i < submitHandlers.Length; i++)
            {
                submitHandlers[i].OnSubmit(new UnityEngine.EventSystems.BaseEventData(EventSystem.current));
            }
        }

        public void Cancel()
        {
            for (int i = 0; i < cancelHandlers.Length; i++)
            {
                cancelHandlers[i].OnCancel(new UnityEngine.EventSystems.BaseEventData(EventSystem.current));
            }
        }

        [Button]
        public void GenerateSelectable()
        {
            var configSelectables = FindObjectsOfType<ConfigSelectable>();

            foreach (var item in configSelectables)
            {
                var uSelectable = item.GetComponent<UnityEngine.UI.Selectable>();
                if (uSelectable != null)
                {
                    item.OnUpSelectable = uSelectable.FindSelectableOnUp()?.GetComponent<ConfigSelectable>();
                    item.OnRightSelectable = uSelectable.FindSelectableOnRight()?.GetComponent<ConfigSelectable>();
                    item.OnLeftSelectable = uSelectable.FindSelectableOnLeft()?.GetComponent<ConfigSelectable>();
                    item.OnDownSelectable = uSelectable.FindSelectableOnDown()?.GetComponent<ConfigSelectable>();
                }
            }
        }
    }
}