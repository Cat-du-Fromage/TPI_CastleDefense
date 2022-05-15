using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static PlayerControls;

namespace KaizerWald
{
    public class SelectionController: ISelectionActions
    {
        private ButtonControl controlKey;
        
        private IHighlightCoordinator coordinator;
        private SelectionRegister register;
        
        public SelectionController(IHighlightCoordinator highlightCoordinator, SelectionRegister selectionRegister)
        {
            register = selectionRegister;
            coordinator = highlightCoordinator;
            highlightCoordinator.Controls.Selection.Enable();
            highlightCoordinator.Controls.Selection.SetCallbacks(this);
            controlKey = Keyboard.current.ctrlKey;
        }
        
        public void OnLeftMouseClick(InputAction.CallbackContext context)
        {
            if (!context.canceled) return;
            
            if (!controlKey.isPressed)
            {
                DeselectNotPreselected();
            }
            SelectPreselectedRegiment();
            coordinator.DispatchEvent(register);
        }

        private void SelectPreselectedRegiment()
        {
            for (int i = 0; i < coordinator.PreselectedRegiments.Count; i++)
            {
                if (coordinator.PreselectedRegiments[i].IsSelected) continue;
                register.OnEnableHighlight(coordinator.PreselectedRegiments[i]);
            }
        }

        private void DeselectNotPreselected()
        {
            if (register.SelectedRegiments.Count == 0) return;
            
            int iteration = register.SelectedRegiments.Count;
            for (int i = iteration - 1; i > -1; i--)
            {
                if (register.SelectedRegiments[i].IsPreselected) continue;
                register.OnDisableHighlight(register.SelectedRegiments[i]);
            }
        }
    }
}
