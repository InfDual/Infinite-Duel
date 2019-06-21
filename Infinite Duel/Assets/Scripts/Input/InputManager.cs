using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Duel.Input

{
    public class InputManager : SerializedMonoBehaviour
    {
        private string ControlFilePath
        {
            get => Application.persistentDataPath + "/controls.ini";
        }

        private static InputManager instance;

        public static InputManager Instance
        {
            get => instance ?? (instance = FindObjectOfType<InputManager>());
        }

        public event Action<PlayerInput> PlayerOneInputDownEventHandler;

        public event Action<PlayerInput> PlayerTwoInputDownEventHandler;

        [SerializeField]
        private InputBinding[] playerOneInput = new InputBinding[12];

        [SerializeField]
        private InputBinding[] playerTwoInput = new InputBinding[12];

        [SerializeField]
        private KeyCode[] playerOneDefaultBindings = new KeyCode[12];

        [SerializeField]
        private KeyCode[] playerTwoDefaultBindings = new KeyCode[12];

        public KeyCode KeyDown
        {
            get
            {
                KeyCode[] keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
                for (int i = 0; i < keyCodes.Length; i++)
                {
                    if (UInput.GetKeyDown(keyCodes[i]))
                    {
                        return keyCodes[i];
                    }
                }
                return KeyCode.None;
            }
        }

        private void Start()
        {
            foreach (var item in UInput.GetJoystickNames())
            {
                print(item);
            }
            GetBindingsFromFile();
        }

        private void Update()
        {
            print($"C1X1 : {UInput.GetAxis("ControllerOneJoystickOneX")}");
            print($"C1Y1 : {UInput.GetAxis("ControllerOneJoystickOneY")}");

            DownInput();
        }

        private void DownInput()
        {
            for (int i = 0; i < playerOneInput.Length; i++)
            {
                if (UInput.GetKeyDown(playerOneInput[i].keyCode))
                {
                    PlayerOneInputDownEventHandler?.Invoke(playerOneInput[i].id);
                }
            }
            for (int i = 0; i < playerTwoInput.Length; i++)
            {
                if (UInput.GetKeyDown(playerTwoInput[i].keyCode))
                {
                    PlayerTwoInputDownEventHandler?.Invoke(playerTwoInput[i].id);
                }
            }
        }

        public KeyCode GetPlayerOneBinding(PlayerInput boundInput)
        {
            return playerOneInput[(int)boundInput].keyCode;
        }

        public KeyCode GetPlayerTwoBinding(PlayerInput boundInput)
        {
            return playerTwoInput[(int)boundInput].keyCode;
        }

        public void ResetPlayerOneBindings()
        {
            for (int i = 0; i < playerOneInput.Length; i++)
            {
                playerOneInput[i].keyCode = playerOneDefaultBindings[i];
            }
            SaveBindingsToFile();
        }

        public void ResetPlayerTwoBindings()
        {
            for (int i = 0; i < playerTwoInput.Length; i++)
            {
                playerTwoInput[i].keyCode = playerTwoDefaultBindings[i];
            }
            SaveBindingsToFile();
        }

        public void ChangeBinding(Player targetPlayer, PlayerInput targetInput, KeyCode newKeyCode)
        {
            if (targetPlayer == Player.PlayerOne)
            {
                playerOneInput[(int)targetInput].keyCode = newKeyCode;
            }
            else
            {
                playerTwoInput[(int)targetInput].keyCode = newKeyCode;
            }

            SaveBindingsToFile();
        }

        private void SaveBindingsToFile()
        {
            using (StreamWriter writer = File.CreateText(ControlFilePath))
            {
                Debug.Log(ControlFilePath);
                foreach (var item in playerOneInput)
                {
                    writer.WriteLine($"playerOne {item.id} {(int)item.keyCode}");
                }
                foreach (var item in playerTwoInput)
                {
                    writer.WriteLine($"playerTwo {item.id} {(int)item.keyCode}");
                }
            }
        }

        private void GetBindingsFromFile()
        {
            if (File.Exists(ControlFilePath))
            {
                using (StreamReader reader = File.OpenText(ControlFilePath))
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            string[] args = reader.ReadLine().Split(' ');
                            if (args.Length > 3)
                            {
                                Debug.LogError("Error Reading Input Binding File. Ensure that all lines only have 3 arguments");
                                return;
                            }
                            else
                            {
                                if (args[0] == "playerOne")
                                {
                                    playerOneInput[i] = new InputBinding((PlayerInput)i, (KeyCode)int.Parse(args[2]));
                                }
                                else if (args[0] == "playerTwo")
                                {
                                    playerTwoInput[i] = new InputBinding((PlayerInput)i, (KeyCode)int.Parse(args[2]));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                SetDefaultBindings();
            }
        }

        public void SetDefaultBindings()
        {
            playerOneInput = new InputBinding[12];
            for (int i = 0; i < playerOneInput.Length; i++)
            {
                playerOneInput[i] = new InputBinding((PlayerInput)i, playerOneDefaultBindings[i]);
            }

            playerTwoInput = new InputBinding[12];
            for (int i = 0; i < playerTwoInput.Length; i++)
            {
                playerTwoInput[i] = new InputBinding((PlayerInput)i, playerTwoDefaultBindings[i]);
            }

            SaveBindingsToFile();
        }

        [Button]
        private void ResetInputBindings()
        {
            for (int i = 0; i < playerOneInput.Length; i++)
            {
                playerOneInput[i] = new InputBinding((PlayerInput)i, playerOneDefaultBindings[i]);
            }
            for (int i = 0; i < playerTwoInput.Length; i++)
            {
                playerTwoInput[i] = new InputBinding((PlayerInput)i, playerTwoDefaultBindings[i]);
            }
        }
    }

    [System.Serializable]
    public struct InputBinding
    {
        public string inputName;
        public PlayerInput id;
        public KeyCode keyCode;

        public InputBinding(PlayerInput playerInput, KeyCode binding)
        {
            keyCode = binding;
            id = playerInput;
            inputName = Utilities.AddSpacesBeforeCapitals(id.ToString());
        }
    }

    public enum PlayerInput
    {
        Up,
        Down,
        Left,
        Right,
        Normal,
        Special,
        Jump,
        Block,
        Throw,
        Accept,
        GoBack,
        Pause
    }

    public enum Player
    {
        PlayerOne,
        PlayerTwo
    }
}