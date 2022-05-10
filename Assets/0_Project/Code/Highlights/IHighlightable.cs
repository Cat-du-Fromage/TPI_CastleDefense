using UnityEngine;

namespace KaizerWald
{
    public interface IHighlightable
    {
        public Transform HighlightTransform { get; }
        public Renderer HighlightRenderer { get; }
    }
}