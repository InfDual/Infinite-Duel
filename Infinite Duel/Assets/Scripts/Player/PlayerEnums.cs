using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duel.PlayerSystems
{
    public enum PlayerInputButton
    {
        None,
        Normal,
        Special,
        Throw,
        Block,
        Jump,
        Pause,
        Up,
        Left,
        Down,
        Right
    }

    public enum ButtonPhase
    {
        NotPressed,
        Down,
        Pressed,
        Up
    }

    public enum AttackType
    {
        Normal,
        Special
    }

    public enum AttackDirection
    {
        Neutral,
        Forwards,
        Backwards,
        Up,
        Down
    }
}