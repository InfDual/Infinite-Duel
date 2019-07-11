using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duel.UI
{
    public class PlayerBindingSetIndicator : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        private ControlConfigManager manager;

        private bool active;

        [Rewired.PlayerIdProperty(typeof(Input.Constants.Player))]
        [SerializeField]
        private int playerId;

        private Color displayColor;

        [SerializeField]
        private UnityEngine.UI.Graphic[] targetGraphics;

        private bool selected;

        private void OnEnable()
        {
            selected = false;

            manager = GetComponentInParent<ControlConfigManager>();
            manager.SetCurrentPlayerEventHandler += OnSetCurrentPlayer;
        }

        private void OnSetCurrentPlayer(int curPlayerID)
        {
            active = playerId == curPlayerID;

            displayColor = active ? Color.white : manager.inactivePlayerColor;
            if (!selected)
            {
                foreach (var item in targetGraphics)
                {
                    item.color = displayColor;
                }
            }
        }

        private void OnDisable()
        {
            selected = false;

            manager.SetCurrentPlayerEventHandler -= OnSetCurrentPlayer;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            selected = false;

            foreach (var item in targetGraphics)
            {
                item.color = displayColor;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            selected = true;
            foreach (var item in targetGraphics)
            {
                item.color = manager.selectedBindingColor;
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            manager.SetCurrentPlayer(playerId);
        }
    }
}