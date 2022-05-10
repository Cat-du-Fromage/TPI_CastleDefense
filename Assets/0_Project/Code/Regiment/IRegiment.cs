using UnityEngine;

namespace KaizerWald
{
    
    public interface IRegiment
    {
        public Transform[] Units { get; set; }
        public ICoordinator Coordinator { get; set; }
        public int RegimentID { get; set; }

        //UNIT REARRANGE HERE!
        public void OnUnitKilled(int unitIndexInRegiment)
        {
            //Units.Rearrange(unitIndexInRegiment);
            Coordinator.OnUnitUpdate(RegimentID, unitIndexInRegiment);
            //Faire le ménage ICI
            
            //Placer l'algorithme ici
            
            //Envoyer le message au IScrivener
        }
    }

    public abstract class RegimentBehaviour : MonoBehaviour
    {
        public Transform[] Units { get; set; }
        public ICoordinator Coordinator { get; set; }
        public int RegimentID { get; set; }
        
        public RegimentClass RegimentClass { get; private set; }
        public void SetRegimentClass(RegimentClass regimentClass) => RegimentClass = regimentClass;

        //UNIT REARRANGE HERE!
        public void OnUnitKilled(int unitIndexInRegiment)
        {
            //Units.Rearrange(unitIndexInRegiment);
            Coordinator.OnUnitUpdate(RegimentID, unitIndexInRegiment);
            //Faire le ménage ICI
            
            //Placer l'algorithme ici
            
            //Envoyer le message au IScrivener
        }
    }
}