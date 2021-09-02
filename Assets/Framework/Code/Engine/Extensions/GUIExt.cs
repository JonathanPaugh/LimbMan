using UnityEngine;

namespace Jape
{
    // ReSharper disable once InconsistentNaming
    public static class GUIExt
    {
        public static GUIStyle Offset(this GUIStyle style, Vector2 offset)
        {
            style.contentOffset = offset;
            return style;
        }

        public static GUIStyle Padding(this GUIStyle style, RectOffset padding)
        {
            style.padding = padding;
            return style;
        }

        public static GUIStyle Margin(this GUIStyle style, RectOffset margin)
        {
            style.margin = margin;
            return style;
        }

        public static GUIStyle Border(this GUIStyle style, RectOffset border)
        {
            style.border = border;
            return style;
        }

        public static GUIStyle FontStyle(this GUIStyle style, FontStyle fontStyle)
        {
            style.fontStyle = fontStyle;
            return style;
        }

        public static GUIStyle FontSize(this GUIStyle style, int size)
        {
            style.fontSize = size;
            return style;
        }

        public static GUIStyle Color(this GUIStyle style, Color color)
        {
            return style;
        }

        public static GUIStyle FontColor(this GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            return style;
        }

        public static GUIStyle Background(this GUIStyle style, Texture2D texture)
        {
            style.normal.background = texture;
            return style;
        }

        public static GUIStyle Alignment(this GUIStyle style, TextAnchor alignment)
        {
            style.alignment = alignment;
            return style;
        }

        public static GUIStyle RichText(this GUIStyle style, bool value)
        {
            style.richText = value;
            return style;
        }

        public static GUIStyle Stretch(this GUIStyle style, bool value)
        {
            style.stretchWidth = value;
            style.stretchHeight = value;
            return style;
        }
    }
}