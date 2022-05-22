using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KWUtils.UGuiUtils;

namespace KaizerWald
{
    public class SquarePreselection : MonoBehaviour
    {
        private PreselectionController controller;

        private void Start()
        {
            controller = GetComponent<IHighlightCoordinator>().PreselectionSystem.Controller;
        }

        //public bool ClickDragPerformed{ get; private set; }
        private Vector2 startLMouse;
        private Vector2 endLMouse;
        
        //==================================================================================================================
        //Rectangle OnScreen
        //==================================================================================================================
        private void OnGUI()
        {
            if (!controller.ClickDragPerformed) return;
            // Create a rect from both mouse positions
            Rect rect = GetScreenRect(controller.StartLMouse, controller.EndLMouse);
            DrawScreenRect(rect);
            DrawScreenRectBorder(rect, 1);
        }
    }
}
