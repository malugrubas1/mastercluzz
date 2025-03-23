using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableHiveSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject hivePrefab;  // Drag your hive prefab here in the Inspector
    public float spawnDepth = 1f; // Adjust based on your camera and scene setup

    private Vector3 originalPosition;           // To store the UI image's original screen position
    private GameObject currentSpawnedHive;      // Reference to the spawned hive in the scene
    private PlayerValues PlayerVal;             // Reference to your PlayerValues script

    void Awake()
    {
        originalPosition = transform.position;

        GameObject logicObj = GameObject.FindGameObjectWithTag("Logic");
        if (logicObj == null)
        {
            Debug.LogError("❌ No GameObject with tag 'Logic' found in the scene!");
        }
        else
        {
            PlayerVal = logicObj.GetComponent<PlayerValues>();
            if (PlayerVal == null)
            {
                Debug.LogError("❌ The GameObject with tag 'Logic' does NOT have a PlayerValues component attached!");
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (hivePrefab == null)
        {
            Debug.LogError("❌ Hive prefab not assigned in the Inspector!");
            return;
        }

        if (PlayerVal == null)
        {
            Debug.LogError("❌ PlayerVal is null! Check if the 'Logic' object exists and has the PlayerValues component.");
            return;
        }

        if (PlayerVal.Tier1Hive == null)
        {
            Debug.LogError("❌ Tier1Hive is null in PlayerValues! Make sure it's assigned.");
            return;
        }

        PlayerVal.Tier1Hive.HivePCount++;

        Vector3 spawnPos = GetWorldPosition(eventData.position);
        currentSpawnedHive = Instantiate(hivePrefab, spawnPos, Quaternion.identity);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSpawnedHive != null)
        {
            Vector3 worldPos = GetWorldPosition(eventData.position);
            currentSpawnedHive.transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the UI image back to its original position in the UI
        transform.position = originalPosition;
        currentSpawnedHive = null;
    }

    // Helper method to convert screen coordinates to world coordinates
    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, spawnDepth);
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
