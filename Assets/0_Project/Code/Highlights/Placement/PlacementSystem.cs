using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class PlacementSystem : IHighlightSystem
    {
        public IHighlightCoordinator Coordinator { get; private set; }
        public IHighlightRegister Register { get; private set; }
        private PlacementController controller;
        
        public PlacementSystem(IHighlightCoordinator coordinator, GameObject prefab)
        {
            Coordinator = coordinator;
            Register = new PlacementRegister(this, prefab, coordinator.Regiments);
            controller = new PlacementController(coordinator, (PlacementRegister)Register);
        }
    }
}
