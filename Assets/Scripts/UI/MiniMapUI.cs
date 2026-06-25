using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class MinimapUI : MonoBehaviour
{
    public static MinimapUI Instance;

    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private GameObject roomIconPrefab;     // zeigt besuchten Raum + Türen
    [SerializeField] private GameObject unknownIconPrefab;  // zeigt "?" 
    [SerializeField] private float cellSize = 40f;

    private List<GameObject> spawnedIcons = new List<GameObject>();

    private void Awake() => Instance = this;

    public void Refresh()
    {
        // Alte Icons löschen
        foreach (var icon in spawnedIcons) Destroy(icon);
        spawnedIcons.Clear();
        var currentNode = RoomLoader.Instance.currentNode;

        var connections = GameManager.Instance.connections;
        var allNodes = GetAllNodes(connections);

        foreach (var node in allNodes)
        {
            if (!node.discovered) continue;

            GameObject iconPrefab = node.visited ? roomIconPrefab : unknownIconPrefab;
            GameObject icon = Instantiate(iconPrefab, mapContainer);

            var rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(
                node.mapPosition.x * cellSize,
                node.mapPosition.y * cellSize
            );

            // Türen nur beim "echten" Raum-Icon zeichnen
            if (node.visited)
            {
                var def = node.prefab.GetComponent<RoomDefinition>();
                var doorIndicator = icon.GetComponent<RoomIconUI>();

                if (doorIndicator != null)
                {
                    doorIndicator.SetDoors(def.doors.Select(d => d.direction).ToList());
                    doorIndicator.setCurrent(node.id == currentNode.id);
                }
            }

            spawnedIcons.Add(icon);
        }
        
        CenterOnCurrentRoom(currentNode);
    }

    private List<RoomNode> GetAllNodes(List<RoomConnection> connections)
    {
        var nodes = new HashSet<RoomNode>();
        nodes.Add(GameManager.Instance.startNode);

        foreach (var c in connections)
        {
            nodes.Add(c.nodeA);
            nodes.Add(c.nodeB);
        }

        return nodes.ToList();
    }

    private void CenterOnCurrentRoom(RoomNode currentNode)
    {
        if (currentNode == null) return;

        Vector2 targetPos = new Vector2(
            -currentNode.mapPosition.x * cellSize,
            -currentNode.mapPosition.y * cellSize
        );

        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetPos));
    }

private IEnumerator SmoothMove(Vector2 target)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector2 start = mapContainer.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mapContainer.anchoredPosition = Vector2.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        mapContainer.anchoredPosition = target;
    }
}