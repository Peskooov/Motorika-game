using System.Collections.Generic;
using UnityEngine;

public class BackgroundGeneratorNew : MonoBehaviour
{
    [SerializeField] private GameObject[] backgroundPrefabs;
    [SerializeField] private float backgroundSpacing = 5f;
    [SerializeField] private float offsetX = 5f;
    [SerializeField] private float offsetY = 5f;
    [SerializeField] private float destroyDistance = 20f;
    [SerializeField] private Transform player;

    private List<GameObject> backgrounds = new List<GameObject>();
    private float nextBackgroundPosition;

    private void Awake()
    {
        nextBackgroundPosition = player.position.z + backgroundSpacing*3;
        GenerateBackground();
    }

    private void Update()
    {
        if (player != null && player.position.z > nextBackgroundPosition - destroyDistance)
        {
            GenerateBackground();
        }

        if (player != null && backgrounds.Count > 0 && player.position.z > backgrounds[0].transform.position.z + destroyDistance)
        {
            DestroyBackground(backgrounds[0]);
        }
    }

    private void GenerateBackground()
    {
        GameObject selectedPrefab = backgroundPrefabs[Random.Range(0, backgroundPrefabs.Length)];
        GameObject newBackground = Instantiate(selectedPrefab, transform);
        float yPos = Random.Range(-offsetY, offsetY);
        newBackground.transform.position = new Vector3(newBackground.transform.position.x - offsetX, newBackground.transform.position.y + yPos, player.position.z + nextBackgroundPosition);
        backgrounds.Add(newBackground);
        nextBackgroundPosition += backgroundSpacing;
    }

    private void DestroyBackground(GameObject background)
    {
        backgrounds.Remove(background);
        Destroy(background);
    }
}