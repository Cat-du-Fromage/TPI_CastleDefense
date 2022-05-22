using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KaizerWald
{
    public class SelectionSystem : IHighlightSystem
    {
        public IHighlightRegister Register { get; }
        public IHighlightCoordinator Coordinator { get; }

        private SelectionController controller;
        
        public SelectionSystem(IHighlightCoordinator coordinator, GameObject prefab)
        {
            Coordinator = coordinator;
            Register = new SelectionRegister(prefab, coordinator.Regiments);
            controller = new SelectionController(coordinator, (SelectionRegister)Register);
        }
    }
}
