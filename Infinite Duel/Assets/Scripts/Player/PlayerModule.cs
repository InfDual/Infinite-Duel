using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duel.PlayerSystems
{
    /// <summary>
    /// Basic functionality that should be required for all player modules
    /// </summary>
    public abstract class PlayerModule : SerializedMonoBehaviour
    {
        protected PlayerMaster master;

        public void Initialize(PlayerMaster playerMaster)
        {
            master = playerMaster;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }
    }
}