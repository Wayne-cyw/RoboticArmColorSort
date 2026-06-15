// BlockSpawner.cs
// Spawns 4 colored blocks randomly within reach of the UR5 arm.
// Maintains a registry so other managers can look up blocks by color.
// Attach to: Managers (empty GameObject)

using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // SECTION 1 — Block Prefabs
    // Drag each colored block prefab from Assets/Prefabs/ into
    // these fields in the Inspector.
    // ─────────────────────────────────────────────────────────────
    [Header("Block Prefabs")]
    public GameObject redBlockPrefab;
    public GameObject greenBlockPrefab;
    public GameObject blueBlockPrefab;
    public GameObject yellowBlockPrefab;

    // ─────────────────────────────────────────────────────────────
    // SECTION 2 — Spawn Settings
    // SpawnZone is the empty GameObject placed in front of the arm.
    // Blocks appear randomly within spawnRadius of that point.
    // ─────────────────────────────────────────────────────────────
    [Header("Spawn Settings")]
    public Transform spawnZone;          // empty GameObject in front of arm
    public float spawnRadius      = 0.3f; // how spread out blocks are
    public float spawnHeight      = 0.05f; // height above ground
    public float minSeparation    = 0.15f; // minimum gap between blocks
    public int   maxAttempts      = 30;    // attempts before fallback

    // ─────────────────────────────────────────────────────────────
    // SECTION 3 — Block Registry
    // Public dictionary so any manager can find a block by color:
    //   blockSpawner.spawnedBlocks["Red"] → returns the red block
    // ─────────────────────────────────────────────────────────────
    [HideInInspector]
    public Dictionary<string, GameObject> spawnedBlocks
        = new Dictionary<string, GameObject>();

    // ─────────────────────────────────────────────────────────────
    // SECTION 4 — Unity Lifecycle
    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        // Validate that everything is assigned before spawning
        if (!ValidateSetup()) return;
        SpawnAllBlocks();
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 5 — Public Methods
    // Called by ResetManager to respawn everything fresh.
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Clears all existing blocks and spawns 4 new ones
    /// at new random positions. Called on Start and on Reset.
    /// </summary>
    public void SpawnAllBlocks()
    {
        ClearBlocks();

        SpawnBlock("Red",    redBlockPrefab);
        SpawnBlock("Green",  greenBlockPrefab);
        SpawnBlock("Blue",   blueBlockPrefab);
        SpawnBlock("Yellow", yellowBlockPrefab);

        Debug.Log($"[BlockSpawner] Spawned {spawnedBlocks.Count} blocks.");
    }

    /// <summary>
    /// Destroys all spawned blocks and clears the registry.
    /// </summary>
    public void ClearBlocks()
    {
        foreach (KeyValuePair<string, GameObject> entry in spawnedBlocks)
        {
            if (entry.Value != null)
                Destroy(entry.Value);
        }
        spawnedBlocks.Clear();
    }

    /// <summary>
    /// Returns the block GameObject for a given color name.
    /// Returns null if the block was already picked up or not found.
    /// </summary>
    public GameObject GetBlock(string colorName)
    {
        if (spawnedBlocks.ContainsKey(colorName))
            return spawnedBlocks[colorName];

        Debug.LogWarning($"[BlockSpawner] No block registered for color: {colorName}");
        return null;
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 6 — Spawn Logic
    // ─────────────────────────────────────────────────────────────

    private void SpawnBlock(string colorName, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError($"[BlockSpawner] Prefab for '{colorName}' is not " +
                           $"assigned in the Inspector.");
            return;
        }

        Vector3 position = FindValidSpawnPosition();

        GameObject block = Instantiate(prefab, position, Quaternion.identity);
        block.name = "Block_" + colorName;
        block.tag  = "Block_" + colorName;

        spawnedBlocks[colorName] = block;
        Debug.Log($"[BlockSpawner] '{colorName}' spawned at {position}");
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 7 — Position Validation
    // Tries up to maxAttempts times to find a position that is:
    //   - Within spawnRadius of the SpawnZone
    //   - At least minSeparation away from all other blocks
    // Falls back to SpawnZone center if all attempts fail.
    // ─────────────────────────────────────────────────────────────

    private Vector3 FindValidSpawnPosition()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Pick a random point inside a circle on the XZ plane
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

            Vector3 candidate = spawnZone.position + new Vector3(
                randomCircle.x,
                spawnHeight,
                randomCircle.y
            );

            if (!IsTooCloseToExistingBlocks(candidate))
                return candidate;
        }

        // Fallback — place at zone center offset slightly
        Debug.LogWarning("[BlockSpawner] Could not find valid position " +
                         "after max attempts. Using fallback.");
        return spawnZone.position + Vector3.up * spawnHeight
               + Random.insideUnitSphere * 0.05f;
    }

    private bool IsTooCloseToExistingBlocks(Vector3 candidate)
    {
        foreach (GameObject block in spawnedBlocks.Values)
        {
            if (block == null) continue;
            if (Vector3.Distance(block.transform.position, candidate) < minSeparation)
                return true;
        }
        return false;
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 8 — Validation
    // Checks all required references are assigned before running.
    // ─────────────────────────────────────────────────────────────

    private bool ValidateSetup()
    {
        bool valid = true;

        if (spawnZone == null)
        {
            Debug.LogError("[BlockSpawner] SpawnZone is not assigned in Inspector.");
            valid = false;
        }
        if (redBlockPrefab == null)
        {
            Debug.LogError("[BlockSpawner] Red block prefab is not assigned.");
            valid = false;
        }
        if (greenBlockPrefab == null)
        {
            Debug.LogError("[BlockSpawner] Green block prefab is not assigned.");
            valid = false;
        }
        if (blueBlockPrefab == null)
        {
            Debug.LogError("[BlockSpawner] Blue block prefab is not assigned.");
            valid = false;
        }
        if (yellowBlockPrefab == null)
        {
            Debug.LogError("[BlockSpawner] Yellow block prefab is not assigned.");
            valid = false;
        }

        return valid;
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 9 — Editor Visualization
    // Draws the spawn radius as a wire circle in the Scene view
    // so you can visually confirm blocks will appear in the
    // right area relative to the arm.
    // ─────────────────────────────────────────────────────────────

    void OnDrawGizmos()
    {
        if (spawnZone == null) return;

        // Draw spawn radius circle
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // transparent yellow
        Gizmos.DrawWireSphere(spawnZone.position, spawnRadius);

        // Draw center marker
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(spawnZone.position, 0.02f);

        // Draw lines to each spawned block
        Gizmos.color = Color.white;
        foreach (GameObject block in spawnedBlocks.Values)
        {
            if (block != null)
                Gizmos.DrawLine(spawnZone.position, block.transform.position);
        }
    }
}