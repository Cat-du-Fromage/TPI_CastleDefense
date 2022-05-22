using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class PreselectionSystem : IHighlightSystem
    {
        public IHighlightRegister Register { get; }
        public IHighlightCoordinator Coordinator { get; }
        
        public PreselectionController Controller{ get; }
        
        public PreselectionSystem(IHighlightCoordinator coordinator, GameObject prefab)
        {
            Coordinator = coordinator;
            Register = new PreselectionRegister(this, prefab, coordinator.Regiments);
            Controller = new PreselectionController(coordinator, (PreselectionRegister)Register);
        }
    }
}
