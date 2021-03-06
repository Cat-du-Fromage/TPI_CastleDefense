using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class UnitFactory : MonoBehaviour
    {
        public List<Transform> CreateRegimentsUnit(Regiment regiment)
        {
            RegimentClass regimentClass = regiment.RegimentClass;
            Vector3 regimentPosition = regiment.transform.position;
            
            float unitSizeX = regimentClass.UnitSize.x;

            List<Transform> units = new(regimentClass.BaseNumUnits);

            for (int i = 0; i < regimentClass.BaseNumUnits; i++)
            {
                Vector3 positionInRegiment = GetPositionInRegiment(i, regiment.IsPlayer);
                GameObject newUnit = Instantiate(regimentClass.PrefabUnit, positionInRegiment, regiment.transform.rotation);
                newUnit.name = $"{regimentClass.PrefabUnit.name}_{i}";

                units.Add(InitializeUnitComponent(newUnit, i).transform);
                
                //==================================================================================================
                //TEMP FOR AI
                newUnit.layer = regiment.IsPlayer ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
                //==================================================================================================
            }
            return units;

            Unit InitializeUnitComponent(GameObject unit, int index)
            {
                Unit component = unit.GetComponent<Unit>();
                component.RegimentAttach = regiment.GetComponent<Regiment>();
                component.IndexInRegiment = index;
                component.GetComponent<UnitAnimation>().InitIdleRandom(index);
                return component;
            }

            //Internal Methods
            Vector3 GetPositionInRegiment(int index, bool player)
            {
                int row = player ? regimentClass.MinRow : regimentClass.BaseNumUnits;
                int column = player ? regimentClass.MinColumn : 1;
                
                //Coord according to index
                int z = index / row;
                int x = index - (z * row);
                //Offset to place regiment in the center of the mass
                float offsetX = regimentPosition.x - GetOffset(row);
                float offsetZ = regimentPosition.z - GetOffset(column);
                return new Vector3(x * unitSizeX + offsetX, 0, -(z * unitSizeX) + offsetZ);
            }

            float GetOffset(int row)
            {
                float unitHalfOffset = unitSizeX / 2f;
                float halfRow = row / 2f;
                return halfRow * unitSizeX - unitHalfOffset;
            }
        }
    }
    
}
