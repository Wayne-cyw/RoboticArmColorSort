// BlockSpawner.cs
// Spawns 4 colored blocks randomly within a circular zone.
// A rectangular exclusion zone inside it prevents blocks from
// spawning too close to the arm base.
// Attach to: Managers (empty GameObject)

using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // SECTION 1 — Block Prefabs
    // ─────────────────────────────────────────────────────────────
    [Header("Block Prefabs")]
    public GameObject redBlockPrefab;
    public GameObject greenBlockPrefab;
    public GameObject blueBlockPrefab;
    public GameObject yellowBlockPrefab;

    // ─────────────────────────────────────────────────────────────
    // SECTION 2 — Spawn Zone (Circle)
    // Blocks spawn randomly within a circle on the XZ plane
    // centred on the SpawnZone GameObject's position.
    //
    //         spawnRadius
    //       ╭───────────╮
    //      │  [exclusion] │
    //      │              │
    //       ╰───────────╯
    //          SpawnZone
    // ─────────────────────────────────────────────────────────────
    [Header("Spawn Zone — Circle")]
    public Transform spawnZone;
    public float spawnRadius = 0.4f;   // radius of the spawn circle
    public float spawnHeight = 0.05f;  // height above ground

    // ─────────────────────────────────────────────────────────────
    // SECTION 3 — Exclusion Zone (Rectangle)
    // Blocks will NOT spawn inside this rectangle.
    // Use it to block out the arm base or any obstacle.
    // Offset it from the SpawnZone centre using exclusionOffset.
    // ─────────────────────────────────────────────────────────────
    [Header("Exclusion Zone — Rectangle")]
    public bool    useExclusionZone  = true;
    public Vector2 exclusionOffset   = Vector2.zero; // XZ offset from SpawnZone
    public float   exclusionWidth    = 0.2f;         // size along X axis
    public float   exclusionDepth    = 0.2f;         // size along Z axis

    // ─────────────────────────────────────────────────────────────
    // SECTION 4 — Placement Settings
    // ─────────────────────────────────────────────────────────────
    [Header("Placement Settings")]
    public float minSeparation = 0.15f;
    public int   maxAttempts   = 50;

    // ─────────────────────────────────────────────────────────────
    // SECTION 5 — Block Registry
    // ─────────────────────────────────────────────────────────────
    [HideInInspector]
    public Dictionary<string, GameObject> spawnedBlocks
        = new Dictionary<string, GameObject>();

    // ─────────────────────────────────────────────────────────────
    // SECTION 6 — Unity Lifecycle
    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (!ValidateSetup()) return;
        SpawnAllBlocks();
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 7 — Public Methods
    // ─────────────────────────────────────────────────────────────

    public void SpawnAllBlocks()
    {
        ClearBlocks();
        SpawnBlock("Red",    redBlockPrefab);
        SpawnBlock("Green",  greenBlockPrefab);
        SpawnBlock("Blue",   blueBlockPrefab);
        SpawnBlock("Yellow", yellowBlockPrefab);
        Debug.Log($"[BlockSpawner] Spawned {spawnedBlocks.Count} blocks.");
    }

    public void ClearBlocks()
    {
        foreach (KeyValuePair<string, GameObject> entry in spawnedBlocks)
            if (entry.Value != null) Destroy(entry.Value);
        spawnedBlocks.Clear();
    }

    public GameObject GetBlock(string colorName)
    {
        if (spawnedBlocks.ContainsKey(colorName))
            return spawnedBlocks[colorName];
        Debug.LogWarning($"[BlockSpawner] No block registered for: {colorName}");
        return null;
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 8 — Spawn Logic
    // ─────────────────────────────────────────────────────────────

    private void SpawnBlock(string colorName, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError($"[BlockSpawner] Prefab for '{colorName}' not assigned.");
            return;
        }

        Vector3 position = FindValidPosition();
        GameObject block = Instantiate(prefab, position, Quaternion.identity);
        block.name = "Block_" + colorName;
        block.tag  = "Block_" + colorName;
        spawnedBlocks[colorName] = block;
        Debug.Log($"[BlockSpawner] '{colorName}' spawned at {position}");
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 9 — Position Finding
    // Picks a random point inside the spawn circle, rejects it
    // if it falls inside the exclusion rectangle or too close
    // to another block, and retries up to maxAttempts times.
    // ─────────────────────────────────────────────────────────────

    private Vector3 FindValidPosition()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Random point inside a circle on the XZ plane
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

            Vector3 candidate = spawnZone.position + new Vector3(
                randomCircle.x,
                spawnHeight,
                randomCircle.y
            );

            // Reject if inside the exclusion rectangle
            if (useExclusionZone && IsInsideExclusionZone(candidate))
                continue;

            // Reject if too close to another block
            if (IsTooCloseToOthers(candidate))
                continue;

            return candidate;
        }

        Debug.LogWarning($"[BlockSpawner] Could not find valid position after " +
                         $"{maxAttempts} attempts. Using fallback.");
        return GetFallbackPosition();
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 10 — Exclusion Zone Check
    // Returns true if the candidate falls inside the exclusion
    // rectangle — meaning it should be rejected and retried.
    // ─────────────────────────────────────────────────────────────

    private bool IsInsideExclusionZone(Vector3 candidate)
    {
        Vector3 exclusionCentre = spawnZone.position + new Vector3(
            exclusionOffset.x, 0f, exclusionOffset.y);

        float halfW = exclusionWidth  / 2f;
        float halfD = exclusionDepth  / 2f;

        bool insideX = candidate.x >= exclusionCentre.x - halfW &&
                       candidate.x <= exclusionCentre.x + halfW;
        bool insideZ = candidate.z >= exclusionCentre.z - halfD &&
                       candidate.z <= exclusionCentre.z + halfD;

        return insideX && insideZ;
    }

    private bool IsTooCloseToOthers(Vector3 candidate)
    {
        foreach (GameObject block in spawnedBlocks.Values)
        {
            if (block == null) continue;
            if (Vector3.Distance(block.transform.position, candidate) < minSeparation)
                return true;
        }
        return false;
    }

    // Tries the 4 edges of the circle outside the exclusion zone as fallback
    private Vector3 GetFallbackPosition()
    {
        Vector3[] edgePoints = {
            spawnZone.position + new Vector3( spawnRadius - 0.05f, spawnHeight, 0),
            spawnZone.position + new Vector3(-spawnRadius + 0.05f, spawnHeight, 0),
            spawnZone.position + new Vector3(0, spawnHeight,  spawnRadius - 0.05f),
            spawnZone.position + new Vector3(0, spawnHeight, -spawnRadius + 0.05f),
        };

        foreach (Vector3 point in edgePoints)
        {
            if (!IsInsideExclusionZone(point) && !IsTooCloseToOthers(point))
                return point;
        }

        return spawnZone.position + Vector3.up * spawnHeight;
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 11 — Validation
    // ─────────────────────────────────────────────────────────────

    private bool ValidateSetup()
    {
        bool valid = true;
        if (spawnZone == null)
        {
            Debug.LogError("[BlockSpawner] SpawnZone is not assigned.");
            valid = false;
        }
        if (redBlockPrefab    == null) { Debug.LogError("[BlockSpawner] Red prefab missing.");    valid = false; }
        if (greenBlockPrefab  == null) { Debug.LogError("[BlockSpawner] Green prefab missing.");  valid = false; }
        if (blueBlockPrefab   == null) { Debug.LogError("[BlockSpawner] Blue prefab missing.");   valid = false; }
        if (yellowBlockPrefab == null) { Debug.LogError("[BlockSpawner] Yellow prefab missing."); valid = false; }
        return valid;
    }

    // ─────────────────────────────────────────────────────────────
    // SECTION 12 — Editor Visualization (Gizmos)
    // Draws the spawn circle and exclusion rectangle in the
    // Scene view so you can adjust them visually before Play.
    // ─────────────────────────────────────────────────────────────

    void OnDrawGizmos()
    {
        if (spawnZone == null) return;

        Vector3 centre = spawnZone.position + Vector3.up * spawnHeight;

        // Draw spawn circle — yellow
        Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
        DrawCircle(centre, spawnRadius, 64);
        Gizmos.color = Color.yellow;
        DrawCircleOutline(centre, spawnRadius, 64);

        // Draw exclusion rectangle — red
        if (useExclusionZone)
        {
            Vector3 exCentre = spawnZone.position + new Vector3(
                exclusionOffset.x, spawnHeight, exclusionOffset.y);

            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawCube(exCentre, new Vector3(exclusionWidth, 0.001f, exclusionDepth));
            Gizmos.color = Color.red;
            DrawRectangleOutline(exCentre, exclusionWidth, exclusionDepth);
        }

        // Draw lines to each spawned block
        Gizmos.color = Color.white;
        foreach (GameObject block in spawnedBlocks.Values)
            if (block != null)
                Gizmos.DrawLine(spawnZone.position, block.transform.position);
    }

    private void DrawCircle(Vector3 centre, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prev = centre + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 next = centre + new Vector3(
                Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    private void DrawCircleOutline(Vector3 centre, float radius, int segments)
    {
        DrawCircle(centre, radius, segments);
    }

    private void DrawRectangleOutline(Vector3 centre, float width, float depth)
    {
        float hw = width / 2f;
        float hd = depth / 2f;

        Vector3 tl = centre + new Vector3(-hw, 0,  hd);
        Vector3 tr = centre + new Vector3( hw, 0,  hd);
        Vector3 br = centre + new Vector3( hw, 0, -hd);
        Vector3 bl = centre + new Vector3(-hw, 0, -hd);

        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
        Gizmos.DrawLine(bl, tl);
    }
}