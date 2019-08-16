using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Duel
{
    public static class Utilities
    {
        public static string AddSpacesBeforeCapitals(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static byte GetByte(ref byte[] bytes, int index)
        {
            byte value = bytes[index];
            Debug.Log(value);
            return value;
        }

        public static bool TryParseHtmlStringToNullableColor(string htmlString, out Color? color)
        {
            Color value = new Color();
            bool succeeded = ColorUtility.TryParseHtmlString(htmlString, out value);
            color = succeeded ? (Color?)value : null;
            return succeeded;
        }

        public static byte GetByte(int value, int index)
        {
            return System.BitConverter.GetBytes(value)[index];
        }
    }

    public static class Layers
    {
        public static readonly int hitbox = LayerMask.NameToLayer("Hitbox");
        public static readonly int hurtbox = LayerMask.NameToLayer("Hurtbox");
    }
}