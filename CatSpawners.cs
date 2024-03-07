using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CatSpawners : MonoBehaviour
{
    public GameObject[] catPrefabs; // Array to hold cat prefabs
    public GameObject[] meowSounds;
    public GameObject shopPanel, inventoryPanel;
    public GameObject jiSpawner, brownSpanwer, greySpawner;
 

    void Start()
    {
        Debug.Log("cat spawn init");
        ActivateCats();
        // Check if there are any cat prefabs assigned
        if (catPrefabs.Length > 0)
        {
            // Randomly select a cat prefab
            int randomIndex = Random.Range(0, catPrefabs.Length);

            // Instantiate the selected cat prefab at the spawner's position
            GameObject spawnedCat = Instantiate(catPrefabs[randomIndex], transform.position, Quaternion.identity);
            BoxCollider2D collider = spawnedCat.AddComponent<BoxCollider2D>();

            SpriteRenderer spriteRenderer = spawnedCat.GetComponent<SpriteRenderer>();

            // Check if the SpriteRenderer component exists
            if (spriteRenderer != null)
            {
                // Set the sorting order to the specified value
                Debug.Log(spriteRenderer.bounds.size);
                collider.size = spriteRenderer.bounds.size * 7.0f;

                // Add CatClickHandler component to the spawned cat
                CatClickHandler clickHandler = spawnedCat.AddComponent<CatClickHandler>();
                clickHandler.catSpawner = this; // Assign reference to CatSpawners
                clickHandler.shopPanel = shopPanel;
                clickHandler.inventoryPanel = inventoryPanel;

                spriteRenderer.sortingOrder = 3;
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found on the spawned cat prefab!");
            }
        }
        else
        {
            Debug.LogError("No cat prefabs assigned to the CatSpawner!");
        }
    }

    public void PlayRandomMeow()
    {
        Debug.Log("PlayRandomMeow Called");
        // Check if there are any sounds in the array
        if (meowSounds.Length > 0)
        {
            // Choose a random index within the range of the array
            int randomIndex = Random.Range(0, meowSounds.Length);

            // Get the GameObject at the randomly chosen index
            GameObject soundObject = meowSounds[randomIndex];

            // Check if the GameObject has an AudioSource component
            if (soundObject != null && soundObject.GetComponent<AudioSource>() != null)
            {
                // Get the AudioSource component attached to the chosen GameObject
                AudioSource soundSource = soundObject.GetComponent<AudioSource>();

                // Play the sound
                soundSource.Play();
            }
            else
            {
                Debug.LogWarning("No AudioSource found on the selected GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("No sounds assigned to the array.");
        }
    }

    void ActivateCats()
    {
        int currentLv = PlayerPrefs.GetInt("PlayerProgression", 0);
        if (currentLv >= 1)
        {
            brownSpanwer.SetActive(true);
           
        }
          if (currentLv >= 2)
            {
            greySpawner.SetActive(true);
        }
            if (currentLv >= 3)
            {
                jiSpawner.SetActive(true);

            }
        
    }

 

    // Nested class CatClickHandler
    public class CatClickHandler : MonoBehaviour
    {
        public CatSpawners catSpawner; // Reference to the CatSpawners class
        public GameObject shopPanel, inventoryPanel;
        private void OnMouseDown()
        {
            // Check if the shop panel is appear or not 
            if (!shopPanel.GetComponent<Canvas>().isActiveAndEnabled && !inventoryPanel.GetComponent<Canvas>().isActiveAndEnabled) 
            {
                    Debug.Log("Cat clicked!");
                    catSpawner.PlayRandomMeow();            
            }
        }
    }
}
