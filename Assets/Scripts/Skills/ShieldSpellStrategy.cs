using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "ShieldSpellStrategy", menuName = "Spells/ShieldSpellStrategy")]
public class ShieldSpellStrategy : SpellStrategy {
    public GameObject shieldPrefab;
    public float duration = 5f;

    public override void CastSpell(Transform origin) {
        Debug.Log("Shield is casted");
        var shield = Instantiate(shieldPrefab, origin.position, Quaternion.identity, origin);
        Destroy(shield, duration);
    }
}