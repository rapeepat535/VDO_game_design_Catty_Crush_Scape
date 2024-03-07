using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject backgroundPanel,confirmPanel,newGameBGPanel, victoryPanel, losePanel, gems, darkenPanel, shopPanel, blurPanel, greyUnlockedPanel, brownUnlockedPanel, siameseUnlockedPanel;
    public GameObject continueButton;
    private const string ItemUnlockKeyPrefix = "ItemUnlocked_";
    public string catUnlockedKey = "CatUnlocked_";

    private string[] buttonNames =
    {
        "catTower",
    "bowl",
    "pinkWool",
    "tv",
    "smallSquareMat",
    "bigSquareMat",
    "catSofa",
    "brownSquareMat",
    "table",
    "catStick",
    "RedKnittingWool",
    "Sharpener",
    "circlePinkMat",
    "catCounter",
    "paperBag",
    };

    private string[] listOfButton =  {
    "catTower",
    "bowl",
    "pinkWool",
    "tv",
    "smallSquareMat",
    "bigSquareMat",
    "catSofa",
    "brownSquareMat",
    "table",
    "catStick",
    "RedKnittingWool",
    "Sharpener",
    "circlePinkMat",
    "catCounter",
    "paperBag",
};

    private List<List<int>> levelInfo = new List<List<int>> {
        new List<int> { 50, 22 },
        new List<int> { 60, 24 },
        new List<int> { 70, 26 },
        new List<int> { 80, 28 },
        new List<int> { 90, 30 },
        new List<int> { 100, 32 },
        new List<int> { 110, 34 },
        new List<int> { 120, 36 },
        new List<int> { 130, 38 },
        // Add more levels here
        new List<int> { 200, 50 },
        new List<int> { 210, 52 },
        new List<int> { 220, 54 },
        new List<int> { 230, 56 },
        new List<int> { 240, 58 },
        new List<int> { 250, 60 },
        new List<int> { 260, 62 },
        new List<int> { 270, 64 },
        new List<int> { 280, 66 },
        new List<int> { 290, 68 },
        new List<int> { 300, 70 },
        new List<int> { 310, 72 },
        new List<int> { 320, 74 },
        new List<int> { 330, 76 },
        new List<int> { 340, 78 },
        new List<int> { 350, 80 },
        new List<int> { 360, 82 },
        new List<int> { 370, 84 },
        new List<int> { 380, 86 },
        new List<int> { 390, 88 },
        new List<int> { 400, 90 }
    };

    public Text itemName;
    public int itemPrice;

    private GameObject winSoundObject, loseSoundObject,startSoundObject;
    public AudioSource winSoundSource, loseSoundSource, startSoundSource, bgmSoundSource;

   



    public int goal, moves, points, usedMove;

    public bool isGameEnded;

    public TMP_Text pointText;
    public TMP_Text movesText;
    public TMP_Text goalText;
    public TMP_Text winText;
    public TMP_Text coinText;
    public TMP_Text bonusText;
    public TMP_Text levelText;



    public void Awake()
    {
        Instance = this;
    }

    public void Initialized(int _moves, int _goal)
    {

        moves = _moves;
        goal = _goal;
    }
    // Start is called before the first frame update
    void Start()
    {
     
        if (SceneManager.GetActiveScene().name == "MatchThree")
        {      
            int level = PlayerPrefs.GetInt("PlayerProgression", 0);
            int goalPoint = levelInfo[level][0];
            int moveLimit = levelInfo[level][1];
            Initialized(moveLimit, goalPoint);
            winSoundSource = winSoundObject.GetComponent<AudioSource>();
            //loseSoundSource = loseSoundObject.GetComponent<AudioSource>();
       

        }

        if (SceneManager.GetActiveScene().name == "CatPlayGround")
        {
            showCatDialog();

        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Debug.Log("played?" + PlayerPrefs.GetInt("Played"));
            if (PlayerPrefs.GetInt("Played") == 1)
            {
                continueButton.GetComponent<Button>().interactable = true;
            } else
            {
                continueButton.GetComponent<Button>().interactable= false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MatchThree")
        {
            pointText.text = "Points: " + points.ToString();
            goalText.text = "Goal: : " + goal.ToString();
            movesText.text = "Moves: " + moves.ToString();
            
        } else if (SceneManager.GetActiveScene().name == "CatPlayGround")
        {
            UpdateCoinText(); // Update the coin count text
            UpdateLevelText();
        }
    }

    // Update the UI Text element with the current coin count
    void UpdateCoinText()
    {
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        coinText.text = "Coins: " + totalCoins.ToString();
    }

    void UpdateLevelText()
    {
        int currentLv = PlayerPrefs.GetInt("PlayerProgression", 0);
        levelText.text = "Current LV: " + currentLv.ToString();
    }

    public void ProcessTurn(int _pointToGain, bool _subtractMoves)
    {
        points += _pointToGain;
        if (_subtractMoves)
        {
            moves--;
            usedMove++;
        }

        if (points >= goal)
        {
            int bonus = moves * 20;
            winSoundSource.Play();
            winText.text = "You won in " + usedMove.ToString() + " moves and scored " + points.ToString() + " points";
            bonusText.text = "Bonus: +" + bonus.ToString() + " coins";
            isGameEnded = true;
            darkenPanel.SetActive(true);
            gems.SetActive(false);
            backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);

            return;
        }

        if (moves == 0)
        {
            isGameEnded = true;
            darkenPanel.SetActive(true);
            gems.SetActive(false);
            backgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            return;
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        toCatPlayground();
    }


    public void NewGameConfirmation()
    {
        if (PlayerPrefs.GetInt("Played") == 1)
        {
            confirmPanel.SetActive(true);
            newGameBGPanel.SetActive(true);
        } else
        {
            NewGame();
        }
    }


    public void CloseConfirmationPanel()
    {
        confirmPanel.SetActive(false);
        newGameBGPanel.SetActive(false);
    }

    public void WinGame()
    {
        int baseReward = 500;
        int bonus = moves * 20;
        int total = baseReward + bonus;
        PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins", 0) + total); // Update total coins
        PlayerPrefs.SetInt("PlayerProgression", PlayerPrefs.GetInt("PlayerProgression", 0) + 1); //update Level 
        PlayerPrefs.Save(); // Save the updated total coins
        SceneManager.LoadScene("CatPlayGround");

    }

    public void LoseGame()
    {
        SceneManager.LoadScene("CatPlayGround");
        
    }

    public void toTheMenu()
    {

        SceneManager.LoadScene("Menu");
    }


    public void goToMatchThree()
    {

        SceneManager.LoadScene("MatchThree");
        PlayerPrefs.SetInt("Played", 1);

    }

    public void closeUnlockPanel(string catName)
    {
        greyUnlockedPanel.SetActive(false);
        brownUnlockedPanel.SetActive(false);
        siameseUnlockedPanel.SetActive(false);
        PlayerPrefs.SetInt(catUnlockedKey + catName, 1);
    }

    public void showCatDialog()
    {
        if (PlayerPrefs.GetInt("PlayerProgression", 0) >= 1 && PlayerPrefs.GetInt(catUnlockedKey + "brown") == 0) 
        {
            brownUnlockedPanel.SetActive(true);
            
        } 
        if (PlayerPrefs.GetInt("PlayerProgression", 0) >= 2 && PlayerPrefs.GetInt(catUnlockedKey + "grey") == 0)
        {
            greyUnlockedPanel.SetActive(true);
        }  
        if (PlayerPrefs.GetInt("PlayerProgression", 0) >= 3 && PlayerPrefs.GetInt(catUnlockedKey + "siamese") == 0)
        {
            siameseUnlockedPanel.SetActive(true);
        }
    }

    public void toCatPlayground()
    {
        startSoundSource.Play();

        Invoke("loadCutscene", 0.7f);
    }

    public void loadCutscene()
    {
        SceneManager.LoadScene("Cuscene");
    }

    public void loadCatPlayGround()
    {
        SceneManager.LoadScene("CatPlayGround");
    }

    public void openShop()
    {
        //PlayerPrefs.DeleteAll();
        blurPanel.SetActive(true);
        PlayerPrefs.SetInt("itemKey", PlayerPrefs.GetInt("TotalCoins", 0) + 9999);
        PlayerPrefs.Save();
        shopPanel.SetActive(true);  
        InteractWithItemButton();
       
    }


    public void closeShop()
    {
        blurPanel.SetActive(false);
        shopPanel.SetActive(false);
    }


    public void InteractWithItemButton()
    {
        for (int i = 0; i < buttonNames.Length; i++)
        {
            //Debug.Log(ItemUnlockKeyPrefix + listOfButton[i]);
            string buttonName = buttonNames[i].ToString();
            string buttonNameWithButton = buttonName + "Button";
            // Find the GameObject of the item button
            Debug.Log("button name:" + buttonNameWithButton);
            GameObject itemButton = GameObject.Find(buttonNameWithButton);
            //Debug.Log(itemButton);
            //Debug.Log(buttonName);
            string key = ItemUnlockKeyPrefix + buttonName;
            //Debug.Log("key from interactWithItemButton" + key);
            //Debug.Log(buttonNameWithButton);
            if (itemButton != null)
            {
                Debug.Log("itemFounded");
                // Get the Button component of the item button
                Button buttonComponent = itemButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    //Debug.Log(buttonComponent);
                    if (PlayerPrefs.GetInt(key) == 1)
                    {

                        Debug.LogWarning("this item has been bought" + buttonNameWithButton);
                        buttonComponent.interactable = false;
                    }
                }
                else
                {
                    Debug.LogError("Button component not found on " + buttonNameWithButton);
                }
            }
            else
            {
                Debug.LogError("Item button not found: " + buttonNameWithButton);
            }
        }
        // Construct the button name using the item name
       
    }

    public void NewItemBuying(string itemNameAndPrice)
    {
        string[] split = itemNameAndPrice.Split(',');
        string itemName = split[0];
        int price = int.Parse(split[1]);
        //Debug.Log("itemName " + itemName);
        //Debug.Log("itemPrice " + price);

        string key = ItemUnlockKeyPrefix + itemName;
       
        // Check if the player has enough coins to make the purchase
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        if (totalCoins >= price)
        {
            Debug.Log(key);
            // Deduct the price of the item from the total coins
            PlayerPrefs.SetInt("TotalCoins", totalCoins - price);
            PlayerPrefs.SetInt(key, 1);
            // Disable the button associated with the purchased item
            
        }
        else
        {
            Debug.Log("Not enough coins to purchase " + itemName);
        }

        PlayerPrefs.Save();
        InteractWithItemButton();
    }

    public void GameExist()
    {
        Application.Quit();
    }



}

