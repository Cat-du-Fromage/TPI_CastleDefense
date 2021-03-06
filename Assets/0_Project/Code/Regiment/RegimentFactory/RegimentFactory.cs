using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using UnityEngine;

namespace KaizerWald
{
    
    [Serializable]
    public struct RegimentSpawner
    {
        public int Number;
        public RegimentClass RegimentClass;
    }

    [RequireComponent(typeof(UnitFactory))]
    public class RegimentFactory : MonoBehaviour
    {
        //==================================================================================================
        //TEMP FOR AI
        [field: SerializeField] private bool IsForPlayerRegiment = true;
        //==================================================================================================
        
        private const float SPACE_BETWEEN_REGIMENT = 2.5f;
        private UnitFactory unitFactory;

        //[field: SerializeField] public PreselectionResponse Preselection { get; private set; }
        [field: SerializeField] private RegimentSpawner[] CreationOrders;

        public List<Regiment> CreateRegiments()
        {
            Debug.Log($"Num regiment To Create: {CreationOrders.Length}");
            unitFactory = GetComponent<UnitFactory>();
            if (CreationOrders.IsNullOrEmpty()) return null;

            List<Regiment> regiments = new List<Regiment>(2);

            float offsetPosition = 0;
            RegimentClass previousRegimentClass = null;
            RegimentClass currentRegimentClass;
            
            for (int i = 0; i < CreationOrders.Length; i++)
            {
                if (CreationOrders[i].RegimentClass == null) continue;
                int numberToSpawn = CreationOrders[i].Number;

                currentRegimentClass = CreationOrders[i].RegimentClass;
                previousRegimentClass ??= currentRegimentClass;

                float offset = GetOffset(currentRegimentClass, i) / 2f + GetOffset(previousRegimentClass, i) / 2f;
                offsetPosition += offset;

                for (int j = 0; j < numberToSpawn; j++)
                {
                    offsetPosition += GetOffset(currentRegimentClass, j) + SPACE_BETWEEN_REGIMENT; //Careful it adds the const even if j=0!
                    Regiment regiment = InstantiateRegiment(currentRegimentClass, i * numberToSpawn + j, offsetPosition);
                    regiments.Add(regiment);
                    
                    //==================================================================================================
                    //TEMP FOR AI
                    regiment.IsPlayer = IsForPlayerRegiment; 
                    //==================================================================================================
                    
                    regiment.UnitsTransform = unitFactory.CreateRegimentsUnit(regiment).ToArray();
                }

                previousRegimentClass = currentRegimentClass;
            }

            //regiments.TrimExcess();
            return regiments;
        }

        private Regiment InstantiateRegiment(RegimentClass regimentClass, int index, float offsetPosition)
        {
            //==================================================================================================
            //TEMP FOR AI
            Vector3 center = GetComponent<RegimentManager>().transform.position;
            //==================================================================================================
            
            
            Vector3 position = center + offsetPosition * Vector3.right;
            

            GameObject newRegiment = new($"{regimentClass.name}", typeof(Regiment));
            newRegiment.transform.SetPositionAndRotation(position, Quaternion.identity);

            Regiment regimentComponent = newRegiment.GetComponent<Regiment>();
            regimentComponent.SetRegimentClass(regimentClass);
            regimentComponent.RegimentID = newRegiment.GetInstanceID();
            newRegiment.name = $"{regimentClass.name}_{regimentComponent.RegimentID}";
            

            return regimentComponent;
        }

        private float GetOffset(RegimentClass regimentClass, int index)
        {
            //==================================================================================================
            //TEMP FOR AI
            int row = IsForPlayerRegiment ? regimentClass.MinRow : regimentClass.BaseNumUnits;
            //==================================================================================================
            //DISTANCE from regiment to an other IS NOT divide by 2 !!
            if (index == 0) return 0;
            float offset = (row * regimentClass.UnitSize.x + regimentClass.SpaceBetweenUnits);
            return offset;
        }
    }
    
}