using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SpellStrategy : ScriptableObject {

    public Sprite uiDisplay;
    public abstract void CastSpell(Transform origin);
    
}
