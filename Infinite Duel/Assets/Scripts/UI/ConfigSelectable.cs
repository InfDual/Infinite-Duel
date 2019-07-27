using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    public class ConfigSelectable : SerializedMonoBehaviour
    {
        [SerializeField]
        private int playerId = -1;

        [HideIf("playerId", -1)]
        [SerializeField]
        private TMPro.TextMeshProUGUI displayText;

        [ShowIf("playerId", -2)]
        [SerializeField]
        private Color selectedColor;

        [ShowIf("playerId", -2)]
        [SerializeField]
        private Color normalColor;

        private bool initialized = false;
        public ConfigSelectable OnUpSelectable;
        public ConfigSelectable OnDownSelectable;
        public ConfigSelectable OnLeftSelectable;
        public ConfigSelectable OnRightSelectable;

        private List<IDeselectHandler> deselectHandlers = new List<IDeselectHandler>();
        private List<ISelectHandler> selectHandlers = new List<ISelectHandler>();
        private List<ISubmitHandler> submitHandlers = new List<ISubmitHandler>();
        private List<ICancelHandler> cancelHandlers = new List<ICancelHandler>();

        private void Awake()
        {
            if (!initialized)
                Initialize();
        }

        public void Initialize()
        {
            if (initialized)
                return;
            displayText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            deselectHandlers.AddRange(GetComponents<IDeselectHandler>());
            selectHandlers.AddRange(GetComponents<ISelectHandler>());
            submitHandlers.AddRange(GetComponents<ISubmitHandler>());
            cancelHandlers.AddRange(GetComponents<ICancelHandler>());
            initialized = true;
        }

        public void Deselect()
        {
            if (playerId > -1)
                displayText.color = Color.white;
            if (playerId == -2)
            {
                displayText.color = normalColor;
            }
            for (int i = 0; i < deselectHandlers.Count; i++)
            {
                deselectHandlers[i].OnDeselect(new BaseEventData(EventSystem.current));
            }
        }

        public void Select()
        {
            if (playerId > -1)
                displayText.color = ControlConfigManager.Singleton.playerColors[playerId];
            if (playerId == -2)
            {
                displayText.color = selectedColor;
            }
            for (int i = 0; i < selectHandlers.Count; i++)
            {
                selectHandlers[i].OnSelect(new BaseEventData(EventSystem.current));
            }
        }

        public void Submit()
        {
            for (int i = 0; i < submitHandlers.Count; i++)
            {
                submitHandlers[i].OnSubmit(new BaseEventData(EventSystem.current));
            }
        }

        public void Cancel()
        {
            for (int i = 0; i < cancelHandlers.Count; i++)
            {
                cancelHandlers[i].OnCancel(new BaseEventData(EventSystem.current));
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