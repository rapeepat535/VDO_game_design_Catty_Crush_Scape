
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FurniturePlacement;

public class InventoryButton : MonoBehaviour


{
    // Start is called before the first frame update
    public GameObject shopPanel, inventoryPanel, blurPanel, warningPanel, warningBGPanel;
    private Dictionary<string, GameObject> gameObjectMap = new Dictionary<string, GameObject>();
    private Dictionary<string, TMP_Text> textObjectMap = new Dictionary<string, TMP_Text>();
    public const string itemUnlockedkey = "ItemUnlocked_";
    public const string itemEquipedKey = "Equiped_";
    public const string itemOnNode = "EquipedOnNode_";
    public string nodeName;

    public GameObject bowl, pinkWool, tv, smallSquareMat, bigSquareMat,
        catSofa, brownSquareMat, table, catStick, RedKnittingWool, Sharpener, circlePinkMat, catCounter, paperBag, catTower;
    public TMP_Text bowlText, pinkWoolText, tvText, smallSquareMatText,
        bigSquareMatText, catSofaText, brownSquareMatText, tableText, catStickText, RedKnittingWoolText, SharpenerText, circlePinkMatText, catCounterText, paperBagText, catTowerText;

    private GameObject furniturePlacementGO;
    //    public GameObject
    //bowlPrefab, pinkWoolPrefab, tvPrefab, smallSquareMatPrefab, bigSquareMatPrefab, catSofaPrefab, brownSquareMatPrefab, tablePrefab, catStickPrefab,
    //        redKnittingWoolPrefab, sharpenerPrefab, circlePinkMatPrefab, catCounterPrefab, paperBagPrefab, catTowerPrefab;
    void Start()
    {
        InitializedDictionary();
        InitAndEnable();
    }

    public void InitAndEnable()
    {
        InitButtonStatus();
        EnableEquipButton();
    }

    public void ResetPlayerPref()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    

    // Function to receive the placement point reference
    public void SetPlacementPoint(GameObjectAndString gas)
    {
        furniturePlacementGO = gas.nodeObject;
        nodeName = gas.nodeName;
        Debug.Log("receive the data from " + furniturePlacementGO.name);
    }



    public void InitButtonStatus()
    {
        foreach (var pair in textObjectMap)
        {
            string key = pair.Key.Replace("Text", "");  // Remove "Text" from the key
            TMP_Text value = pair.Value;    // This will give you the value (TMP_Text) of the current entry


            if (PlayerPrefs.GetInt(itemEquipedKey + key) == 0)
            {
                value.text = "equip";
            }
            else
            {
                value.text = "equiped";
            }

            if (PlayerPrefs.GetInt(itemUnlockedkey + key) == 0)
            {
                value.text = "not own";
            }
        }
    }

    public void EnableEquipButton()
    {
        foreach (var pair in gameObjectMap)
        {
            string key = pair.Key;
            GameObject gameObject = pair.Value;
            if (PlayerPrefs.GetInt(itemUnlockedkey + key) == 0)
            {
                gameObject.GetComponent<Button>().interactable = false;
            }
            else
            {
                gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }



    public void InitializedDictionary()
    {

        gameObjectMap = new Dictionary<string, GameObject>
        {
            { "bowl", bowl },
            { "pinkWool", pinkWool },
            { "tv", tv },
            { "smallSquareMat", smallSquareMat },
            { "bigSquareMat", bigSquareMat },
            { "catSofa", catSofa },
            { "brownSquareMat", brownSquareMat },
            { "table", table },
            { "catStick", catStick },
            { "RedKnittingWool", RedKnittingWool },
            { "Sharpener", Sharpener },
            { "circlePinkMat", circlePinkMat },
            { "catCounter", catCounter },
            { "paperBag", paperBag },
            { "catTower", catTower },

        };

        textObjectMap = new Dictionary<string, TMP_Text>
        {
            { "bowlText", bowlText },
            { "pinkWoolText", pinkWoolText },
            { "tvText", tvText },
            { "smallSquareMatText", smallSquareMatText },
            { "bigSquareMatText", bigSquareMatText },
            { "catSofaText", catSofaText },
            { "brownSquareMatText", brownSquareMatText },
            { "tableText", tableText },
            { "catStickText", catStickText },
            { "RedKnittingWoolText", RedKnittingWoolText },
            { "SharpenerText", SharpenerText },
            { "circlePinkMatText", circlePinkMatText },
            { "catCounterText", catCounterText },
            { "paperBagText", paperBagText },
            { "catTowerText", catTowerText }
        };
    }

    public void initButtonStatus()
    {
        foreach (var pair in textObjectMap)
        {
            string key = pair.Key.Replace("Text", "");  // Remove "Text" from the key
            TMP_Text value = pair.Value;    // This will give you the value (TMP_Text) of the current entry


            if (PlayerPrefs.GetInt(itemEquipedKey + key) == 0)
            {
                value.text = "equip";
            }
            else
            {
                value.text = "equiped";
            }

            if (PlayerPrefs.GetInt(itemUnlockedkey + key) == 0)
            {
                value.text = "not own";
            }
        }
    }


    public void equipFurniture(string furnitureName)
    {
        string keyForEquip = itemEquipedKey + furnitureName;
        string keyForEquipOnNode = itemOnNode + "_" + nodeName + "_" + furnitureName;
        Debug.Log("furnitureName = " + furnitureName);
        Debug.Log("equipedKey status" + PlayerPrefs.GetInt(keyForEquip).ToString());
        if (PlayerPrefs.GetInt(itemUnlockedkey + furnitureName) == 1)
        {
            // Check if the gameObjectMap contains the key
            if (gameObjectMap.ContainsKey(furnitureName))
            {
                GameObject furnitureObject = gameObjectMap[furnitureName];
                //furnitureObject.GetComponent<Button>().interactable = false;
                // Now you have access to the GameObject, you can perform whatever actions you need
                Debug.Log("You bought this, proceed to placing " + furnitureName);
                string furnitureText = furnitureName + "Text";
                if (textObjectMap.ContainsKey(furnitureText))
                {

                    //saving the object and position

                    if (PlayerPrefs.GetInt(keyForEquip) == 0)
                    {
                        Debug.Log("Equip");
                        textObjectMap[furnitureText].text = "equiped";
                        PlayerPrefs.SetInt(keyForEquip, 1);
                        //just addded//
                        Debug.Log("KeyForEquip on node: " + keyForEquipOnNode);
                        PlayerPrefs.SetInt(keyForEquipOnNode,1);
                        //just addded//
                        PlayerPrefs.Save();
                        furniturePlacementGO.SendMessage("Equiping", furnitureName);
                        Debug.Log("equipOnNode: " + nodeName);
                        Debug.Log("equipOnGameOBject" + furniturePlacementGO);
                        Debug.Log("Key to equip on node" + keyForEquipOnNode);
                        Invoke("CloseInventory", 0.2f);

                    }
                    else
                    {
                        //just added//
                        Debug.Log("Key for unequip on node" + keyForEquipOnNode);
                        if (PlayerPrefs.GetInt(keyForEquipOnNode) == 1)
                        {
                            Debug.Log("Unequip on node :" + nodeName);
                            textObjectMap[furnitureText].text = "equip";
                            PlayerPrefs.SetInt(keyForEquip, 0);
                            PlayerPrefs.SetInt (keyForEquipOnNode, 0);
                            PlayerPrefs.Save();
                            furniturePlacementGO.SendMessage("Equiping", "none");
                            Invoke("CloseInventory", 0.2f);
                        } else
                        {
                            Debug.Log("UnEquip from another node, which is : " + nodeName);
                            EnableWarningPanel();
                        }

                        //just added//
                       
                    }
                }
              
            }
            else
            {
                Debug.LogError("GameObject for " + furnitureName + " not found in gameObjectMap.");
            }
        }
        else
        {
            Debug.Log("Didn't buy yet");
        }
        InitAndEnable();
    }
    public void CloseInventory()
    {
        blurPanel?.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    public void DisableWarningPanel()
    {
        warningBGPanel.SetActive(false);
        warningPanel.SetActive(false);
    }

    public void EnableWarningPanel()
    {
        warningBGPanel.SetActive(true);
        warningPanel.SetActive(true);
    }
}
