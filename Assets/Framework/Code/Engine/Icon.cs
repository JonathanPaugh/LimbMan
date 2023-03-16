using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jape
{
    [Serializable]
    public struct Icon
    {
        [SerializeField]
        private Color color;
        public Color Color => color;

        [SerializeField, FormerlySerializedAs("baseTexture")]
        private Texture2D texture;
        public Texture2D Texture => texture;

        public bool IsSet => texture != null;

        public Icon(Texture2D texture, Color color)
        {
            this.texture = texture;
            this.color = color;
        }

        public Texture2D Render()
        {
            if (texture == null) { return null; }
            return texture.Colorize(color);
        }
    }
}