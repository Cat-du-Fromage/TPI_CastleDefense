using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public sealed class Selection : MonoBehaviour, IHighlightable
    {
        public Transform HighlightTransform { get; private set; }
        public Renderer HighlightRenderer { get; private set; }

        private void Awake()
        {
            HighlightTransform = transform;
            HighlightRenderer = GetComponent<Renderer>();
            HighlightRenderer.enabled = false;
        }
    }
}
