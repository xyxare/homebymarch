using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "HealSpellStrategy", menuName = "Spells/HealSpellStrategy")]
public class HealSpellStrategy : SpellStrategy {
    public GameObject HealPrefab;
    public float duration = 5f;

    public override void CastSpell(Transform origin) {
        Debug.Log("heal is casted");
        var heal = Instantiate(HealPrefab, origin.position, Quaternion.identity, origin);
        Destroy(heal, duration);
    }
}