using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData data;

    public void UpdateEquipmentData(Equipment equipment, int index){

        data.equipment.SetValue(equipment,index);
    }
        
    
}
