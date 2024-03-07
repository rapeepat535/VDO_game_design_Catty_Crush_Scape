using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{
    // Start is called before the first frame update
    public static Navigator Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void goToMatchThree()
    {
        Debug.Log("goTOMatchThreeCalled");

        SceneManager.LoadScene("MatchThree");
        
    }
    

}
