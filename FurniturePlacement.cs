
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FurniturePlacement : MonoBehaviour
{
    public GameObject[] furniturePrefab;
    public GameObject placeHolderPrefab;
    public GameObject shopPanel, inventoryPanel, blurPanel;
    public string nodeName;
    public const string itemEquipedKey = "EquipedOnNode_";
    private string currentSpriteName;
    public GameObject furniturePlacement;

    public GameObject inventoryButtonGO;
    public string[] furnitureName = {"bowl", 
             "pinkWool", 
             "tv", 
             "smallSquareMat",
             "bigSquareMat", 
             "catSofa", 
            "table",
             "catStick", 
            "RedKnittingWool", 
             "Sharpener", 
            "circlePinkMat", 
            "catCounter", 
             "paperBag",
             "catTower"};


 

    void Start()
    {
        instantiateTheSprite();
    }

    public void instantiateTheSprite()
    {
        Debug.Log("instanceTiateTHeSpriteCalled");
        Debug.Log("instantiateTheSpriteCalled");
        Debug.Log(furniturePrefab.Length);
            if (furniturePrefab.Length > 0)
            {
                // Check if the placeholder prefab has been instantiated
                
                if (furniturePlacement == null)
                {
                // Instantiate the selected placeholder prefab at the spawner's position
                GameObject prefab = placeHolderPrefab;
                foreach (var item in furnitureName)
                {
                    if (PlayerPrefs.GetInt(itemEquipedKey + "_" + nodeName + "_" + item) == 1)
                    {
                        Debug.Log("found item for node: " + nodeName + ", which is : " + item);
                        prefab = System.Array.Find(furniturePrefab, furnitureItem => furnitureItem.name == item);
                    }
                }

                   furniturePlacement = Instantiate(prefab, transform.position, Quaternion.identity);
                    Vector3 OriginalScaling = prefab.transform.localScale;
                    BoxCollider2D collider = furniturePlacement.AddComponent<BoxCollider2D>();

                    SpriteRenderer spriteRenderer = furniturePlacement.GetComponent<SpriteRenderer>();
                Debug.Log("currentSprite");
                Debug.Log(spriteRenderer);

                    // Check if the SpriteRenderer component exists
                    if (spriteRenderer != null)
                    {
                    Debug.Log(OriginalScaling);
                        Debug.Log("generating place point");
                        // Set the sorting order to the specified value
                        Vector2 spriteSize = spriteRenderer.bounds.size*3;
                        // Set the collider size to match the sprite size
                        collider.size = spriteSize*1.1f;

                        // Add CatClickHandler component to the spawned cat
                        ClickHandler clickHandler = furniturePlacement.AddComponent<ClickHandler>();
                        clickHandler.furnitureSpawner = this; // Assign reference to CatSpawners
                        clickHandler.shopPanel = shopPanel;
                        clickHandler.furnitureObject = furniturePlacement;
                        clickHandler.inventoryPanel = inventoryPanel;
                        clickHandler.inventoryButtonGO = inventoryButtonGO;
                        clickHandler.nodeName = nodeName;
                        //spriteRenderer.sortingOrder = 3;
                    }
                    else
                    {
                        Debug.LogError("SpriteRenderer component not found on the spawned cat prefab!");
                    }
                }
                else
            {
                SpriteRenderer spriteRenderer = furniturePlacement.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = placeHolderPrefab.GetComponent<SpriteRenderer>().sprite;
                furniturePlacement.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

        }

    }

    public void ChangeSprite(string spriteName)
    {
       if (spriteName == "none")
        {
            Debug.Log("instantiate");
            instantiateTheSprite();
        } else
        {
            foreach (var item in furniturePrefab)
            {
                if (item.name == spriteName)
                {
                    Debug.Log("Founded matched item" + item);
                    SpriteRenderer spriteRenderer = furniturePlacement.GetComponent<SpriteRenderer>();
                    Debug.Log("Sprite before Change : " + spriteRenderer.sprite);
                    spriteRenderer.sprite = item.GetComponent<SpriteRenderer>().sprite;
                    Debug.Log("Sprite after Change : " + spriteRenderer.sprite);

                    // Update the collider size based on the new scale
                    Vector3 newSpriteScale = item.transform.localScale;
                    furniturePlacement.transform.localScale = newSpriteScale;
                    BoxCollider2D collider = furniturePlacement.GetComponent<BoxCollider2D>();
                    if (collider != null)
                    {
                        // Get the size of the sprite renderer's bounds
                        // Scale the size by the new sprite scale
                        // Set the collider size to match the scaled size
                        collider.size = new Vector2(10f, 10f); ;
                    }

                    Debug.Log("found, begin the changing sequence for: " + item);
                }
            }
        }
    }

    public struct GameObjectAndString
    {
        public string nodeName;
        public GameObject nodeObject;
        public GameObjectAndString(string nodeName, GameObject nodeObject)
        {
            this.nodeName = nodeName;
            this.nodeObject = nodeObject;
        }
    }

    public void OpenInventory()
    {
        Debug.Log("Open from node " + nodeName);
        blurPanel.SetActive(true);
        inventoryPanel.SetActive(true);
    

    }

    public void Equiping(string item)
    {
        Debug.Log("equiping from button" + item);
        Debug.Log("equiped on " + gameObject);
        ChangeSprite(item);
    }

}
public class ClickHandler : MonoBehaviour
{
    public FurniturePlacement furnitureSpawner;
    public GameObject shopPanel, inventoryPanel, inventoryButtonGO;
    public GameObject furnitureObject;
    public string nodeName;

    private void OnMouseDown()
    {
        Debug.Log("!shopPanel.GetComponent<Canvas>().isActiveAndEnabled" + !shopPanel.GetComponent<Canvas>().isActiveAndEnabled);
        Debug.Log("!inventoryPanel.GetComponent<Canvas>().isActiveAndEnabled" + !inventoryPanel.GetComponent<Canvas>().isActiveAndEnabled);
        Debug.Log("OnMouseDown");
        // Check if the shop panel is appear or not 
        if (!shopPanel.GetComponent<Canvas>().isActiveAndEnabled && !inventoryPanel.GetComponent<Canvas>().isActiveAndEnabled)
        {
            Debug.Log("Object clicked!");
            furnitureSpawner.OpenInventory();
            inventoryButtonGO.SendMessage("Start");
            inventoryButtonGO.SendMessage("SetPlacementPoint", new FurniturePlacement.GameObjectAndString(nodeName, furnitureSpawner.gameObject));
        }


    }
}