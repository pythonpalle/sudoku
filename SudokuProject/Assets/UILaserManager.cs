using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzleSelect;
using UnityEngine;
using UnityEngine.UI;

public class UILaserManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject arrowPrefab; // Ett UI-objekt med en Image-komponent
    [SerializeField] private float arrowThickness = 8f; // Hur bred ska lasarlinjen vara i pixlar
    [SerializeField] private Color laserColor = new Color(1f, 0.3f, 0.3f, 0.7f); // Svagt röd/transparent

    // En dictionary för att hålla reda på alla SelectTile UI-objekt (kopplas i din setup)
    private Dictionary<TileIndex, RectTransform> tileTransforms = new Dictionary<TileIndex, RectTransform>();
    private List<GameObject> activeArrows = new List<GameObject>();

    public bool IsSetup { get; private set; }

    // Anropa denna när du initierar spelet för att mappa dina TileIndex till rätt UI RectTransform
    private void RegisterTileUI(TileIndex index, RectTransform rectTransform)
    {
        tileTransforms[index] = rectTransform;
    }
    
    public void SetUp(Dictionary<TileIndex, SelectTile> hintTiles)
    {
        foreach (var kvp in hintTiles)
        {
            RegisterTileUI(kvp.Key, kvp.Value.RectTransform);
        }
    }
    
    private bool hasDrawnArrowsOnce = false;

    public void DrawLaserArrows(List<LaserArrow> arrows)
    {
        if (!hasDrawnArrowsOnce)
        {
            Canvas.ForceUpdateCanvases(); 
        }
        
        ClearArrows();

        foreach (var arrow in arrows)
        {
            // Hämta RectTransforms för start och slut
            if (!tileTransforms.TryGetValue(arrow.StartTile, out RectTransform startRect) ||
                !tileTransforms.TryGetValue(arrow.EndTile, out RectTransform endRect))
            {
                continue; // Hoppa över om UI-rutan inte hittas
            }

            // Hämta exakta UI-positioner i lokala Canvas-koordinater
            Vector3 startPos = GetCanvasPositionOfTile(startRect);
            Vector3 endPos = GetCanvasPositionOfTile(endRect);

            // Skapa pilen från vår Prefab
            GameObject arrowObj = Instantiate(arrowPrefab, transform);
            activeArrows.Add(arrowObj);

            RectTransform arrowRect = arrowObj.GetComponent<RectTransform>();
            Image arrowImage = arrowObj.GetComponent<Image>();
            
            if (arrowImage != null) arrowImage.color = laserColor;

            // 1. Sätt positionen till start-rutan
            arrowRect.localPosition = startPos;

            // 2. Räkna ut riktning, distans och vinkel till slut-rutan
            Vector3 direction = endPos - startPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. Justera rotationen (eftersom vår sprite pekar åt höger roterar vi runt Z-axeln)
            arrowRect.localRotation = Quaternion.Euler(0, 0, angle);

            // 4. Sätt storleken: bredden blir distansen till målet, höjden blir tjockleken på linjen
            arrowRect.sizeDelta = new Vector2(distance, arrowThickness);

            // Sätt pivot till vänsterkant om det inte är gjort i din prefab (viktigt!)
            arrowRect.pivot = new Vector2(0f, 0.5f);
        }

        hasDrawnArrowsOnce = true;
    }

    public void ClearArrows()
    {
        foreach (var arrow in activeArrows)
        {
            if (arrow != null) Destroy(arrow);
        }
        activeArrows.Clear();
    }

    // Hjälpmetod för att säkerställa att vi får rätt position oavsett Canvas inställningar
    private Vector3 GetCanvasPositionOfTile(RectTransform tileRect)
    {
        // Vi transformerar rutan position till det lokala systemet i lasermanagerns panel
        return transform.InverseTransformPoint(tileRect.position);
    }


}
