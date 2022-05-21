using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class PreselectionSystem : IHighlightSystem
    {
        public IHighlightRegister Register { get; }
        public IHighlightCoordinator Coordinator { get; }
        
        private PreselectionController controller;
        
        public PreselectionSystem(IHighlightCoordinator coordinator, GameObject prefab)
        {
            //coordinator.Controls.Placement.Enable();
            Coordinator = coordinator;
            Register = new PlacementRegister(this, prefab, coordinator.Regiments);
            //controller = new PlacementController(coordinator, (PlacementRegister)Register);
        }
    }
}
