using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable;
    public GameObject gem;

    public Node (bool _isUsable, GameObject _gem)
    {
        isUsable = _isUsable;
        gem = _gem;
    }
}
