using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KaizerWald
{
    public class SelectionSystem
    {
        public SelectionRegister Register { get; private set; }
        private SelectionController controller;
        
        public SelectionSystem(IHighlightCoordinator coordinator, GameObject prefab)
        {
            Register = new SelectionRegister(prefab, coordinator.Regiments);
            controller = new SelectionController(coordinator, Register);
        }


    }
}
