using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using HomeByMarch;

[CreateAssetMenu(fileName = "MeteorSpellStrategy", menuName = "Spells/MeteorSpellStrategy")]
public class ChnyBossSkill : SpellStrategy
{
    public GameObject meteorPrefab;
    public float damage = 50f;
    public float radius = 5f;
    public float spawnDistance = 2f; // Distance in front of the player
    public float spawnHeight = 5f; // Height above the player
    public float duration = 5f; // Duration of the meteor spell
    public float detectionDelay = 1f; // Delay before starting detection
    public int soundref = 0;

    public override void CastSpell(Transform origin)
    {
        Debug.Log("Meteor is casted");

        // Calculate the spawn position in front of and above the player
        Vector3 spawnPosition = origin.position + origin.forward * spawnDistance + Vector3.up * spawnHeight;

        var meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        // SFXManager.PlaySFX(SoundTypes.Skills, soundref);
        Destroy(meteor, duration); // Destroy the meteor after the duration

        // Start detection after a delay
        origin.GetComponent<MonoBehaviour>().StartCoroutine(DelayedDetection(spawnPosition));
    }

    private IEnumerator DelayedDetection(Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(detectionDelay);

        // Detect and apply damage to the player within the radius of the spawn position
        Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, radius);
        foreach (var hitCollider in hitColliders)
        {
            Health playerHealth = hitCollider.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }   
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the radius of the meteor's damage area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
}
