// <auto-generated>
// Rewired Constants
// This list was generated on 7/13/2019 12:45:13 AM
// The list applies to only the Rewired Input Manager from which it was generated.
// If you use a different Rewired Input Manager, you will have to generate a new list.
// If you make changes to the exported items in the Rewired Input Manager, you will
// need to regenerate this list.
// </auto-generated>

namespace Duel.Input.Constants {
    public static partial class Action {
        // Default
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Up-Down")]
        public const int Vertical = 12;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Left-Right")]
        public const int Horizontal = 13;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Normal Attack")]
        public const int Normal = 14;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Special Attack")]
        public const int Special = 15;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Jump")]
        public const int Jump = 16;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Block")]
        public const int Block = 17;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Grab and Throw")]
        public const int Throw = 18;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Accept")]
        public const int Accept = 19;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Go Back")]
        public const int Return = 20;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Pause")]
        public const int Pause = 21;
        // UI
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Vertical Movement")]
        public const int UIVertical = 22;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Horizontal Movement")]
        public const int UIHorizontal = 23;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Sumbit")]
        public const int UISubmit = 24;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "UI", friendlyName = "Cancel")]
        public const int UICancel = 25;
    }
    public static partial class Category {
        public const int Default = 0;
        public const int Player_Two = 2;
    }
    public static partial class Layout {
        public static partial class Joystick {
            public const int Default = 0;
        }
        public static partial class Keyboard {
            public const int Default = 0;
        }
        public static partial class Mouse {
            public const int Default = 0;
        }
        public static partial class CustomController {
            public const int Default = 0;
        }
    }
    public static partial class Player {
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "System")]
        public const int System = 9999999;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player0")]
        public const int Player0 = 0;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player1")]
        public const int Player1 = 1;
    }
    public static partial class CustomController {
    }
    public static partial class LayoutManagerRuleSet {
    }
    public static partial class MapEnablerRuleSet {
    }
}
