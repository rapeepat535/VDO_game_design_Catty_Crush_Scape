using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GemBoard : MonoBehaviour
{
    //define the size of the board
    public int width = 6;
    public int height = 8;
    //define some spacing for the board
    public float spacingX;
    public float spacingY;
    //get a reference to our gem prefabs
    public GameObject[] gemPrefabs;
    //get a reference to the collection nodes gemBoard + GO
    public Node[,] gemBoard;
    public GameObject gemBoardGO;
    private List<GameObject> gemsToDestroy = new();
    public GameObject gemParent;
    private AudioSource popSoundSource;
    public GameObject popSoundObject;
    [SerializeField]
    private bool isProcessingMove;
    [SerializeField]
    private Gem selectedGem;
    private bool isGameStart = false;


    public GameObject gemsParent;


    [SerializeField]
    List<Gem> gemsToRemove = new();

    //layoutArray
    public ArrayLayout arrayLayout;
    //public static of gemboard
    public static GemBoard Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        popSoundSource = popSoundObject.GetComponent<AudioSource>();
        InitializeBoard();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Gem>())
            {
                if (isProcessingMove)
                    return;

                Gem gem = hit.collider.gameObject.GetComponent<Gem>();
                //Debug.Log("I have hit a gem it is : " + gem.gameObject);

                SelectGem(gem);
            }
        }
    }

    void InitializeBoard()
    {
        DestroyGems();
        gemBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                if (arrayLayout.rows[y].row[x])
                {
                    gemBoard[x, y] = new Node(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, gemPrefabs.Length);

                    GameObject gem = Instantiate(gemPrefabs[randomIndex], position, Quaternion.identity);
                    gem.transform.SetParent(gemParent.transform);
                    gem.GetComponent<Gem>().SetIndicies(x, y);
                    gemBoard[x, y] = new Node(true, gem);
                    gemsToDestroy.Add(gem);
                }
            }
        }

        if (CheckBoard())
        {
            //Debug.Log("We have matches let's re-create the board");
            InitializeBoard();
        }
        else
        {
            isGameStart = true;
            //Debug.Log("There are no matches, it's time to start the game!");
        }
    }

    private void DestroyGems()
    {
        if (gemsToDestroy != null)
        {
            foreach (GameObject gem in gemsToDestroy)
                Destroy(gem);
            gemsToDestroy.Clear();
        }
    }


    public bool CheckBoard()
    {
        if (GameManager.Instance.isGameEnded) {
            return false;
        }
        //Debug.Log("Checking Board");
        bool hasMatched = false;

        gemsToRemove.Clear();

       

        foreach (Node nodeGem in gemBoard)
        {
            if (nodeGem.gem != null)
            {
                nodeGem.gem.GetComponent<Gem>().isMatched = false;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //checking if gem node is usable
                if (gemBoard[x, y].isUsable)
                {
                    //then proceed to get gem class in node.
                    Gem gem = gemBoard[x, y].gem.GetComponent<Gem>();

                    //ensure its not matched
                    if (!gem.isMatched)
                    {
                        //run some matching logic

                        MatchResult matchedGems = IsConnected(gem);

                        if (matchedGems.connectedGems.Count >= 3)
                        {
                            MatchResult superMatchGems = SuperMatchGems(matchedGems);
                            //complex matching...

                            gemsToRemove.AddRange(superMatchGems.connectedGems);


                            foreach (Gem pot in superMatchGems.connectedGems)
                                pot.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }


        return hasMatched;
    }

    public IEnumerator ProcessTurnOnMatchedBoard(bool _subtractMoves)
    {
        foreach (Gem gemToRemove in gemsToRemove)
        {
            gemToRemove.isMatched = false;
        }

        RemoveAndRefill(gemsToRemove);

        GameManager.Instance.ProcessTurn(gemsToRemove.Count, _subtractMoves);
        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
    }

    private void RemoveAndRefill(List<Gem> gemsToRemove)
    {
        //remove gem and clear the board

        foreach (Gem gem in gemsToRemove)
        {
            //getting x and y incies and storing them

            int _xIndex = gem.xIndex;
            int _yIndex = gem.yIndex;

            Destroy(gem.gameObject);

            gemBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gemBoard[x, y].gem == null)
                {
                    //Debug.Log("The location x:" + x + " Y:" + y + " is empty, attempting to refill it");
                    RefillGem(x, y);
                }

            }
        }
    }

    private void RefillGem(int x, int y)
    {
        int yOffset = 1;

        while (y + yOffset < height && gemBoard[x, y + yOffset].gem == null)
        {
            //Debug.Log("Current Yoffset: " + yOffset);
            yOffset++;
        }

        if (y + yOffset < height && gemBoard[x, y + yOffset].gem != null)
        {
            Gem gemAbove = gemBoard[x, y + yOffset].gem.GetComponent<Gem>();

            Vector3 targetPost = new Vector3(x - spacingX, y - spacingY, gemAbove.transform.position.z);

            gemAbove.MoveToTarget(targetPost);

            gemAbove.SetIndicies(x, y);

            gemBoard[x, y] = gemBoard[x, y + yOffset];

            gemBoard[x, y + yOffset] = new Node(true, null);
        }

        if (y + yOffset == height)
        {
            //Debug.Log("reach the top without finding a gem");

            SpawnGemAtTop(x);
        }
    }

    private void SpawnGemAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationTomoveTo = 8 - index;
        //Debug.Log("Spawing gem, at the index of :" + index);

        int randomIndex = Random.Range(0, gemPrefabs.Length);
        GameObject newGem = Instantiate(gemPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);


        newGem.transform.SetParent(gemParent.transform);

        //set indicies
        newGem.GetComponent<Gem>().SetIndicies(x, index);

        //set it on board
        gemBoard[x, index] = new Node(true, newGem);
        //move it to that location
        Vector3 targetPosition = new Vector3(newGem.transform.position.x, newGem.transform.position.y - locationTomoveTo, newGem.transform.position.z);
        newGem.GetComponent<Gem>().MoveToTarget(targetPosition);

    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = 7; y >= 0; y--)
        {
            if (gemBoard[x, y].gem == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #region Cascadding Gems

    #endregion



    private MatchResult SuperMatchGems(MatchResult _matchedResults)
    {
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            foreach (Gem gem in _matchedResults.connectedGems)
            {
                List<Gem> extraConnectedGems = new();
                CheckDirection(gem, new Vector2Int(0, 1), extraConnectedGems);

                CheckDirection(gem, new Vector2Int(0, -1), extraConnectedGems);

                if (extraConnectedGems.Count >= 2)
                {
                    Debug.Log("I have a super Horizontal Match");
                
                    if (isGameStart)
                    {
                        Debug.Log("PLAYING SOUNDS");
                        popSoundSource.Play();
                    }
                   
                    extraConnectedGems.AddRange(_matchedResults.connectedGems);
                    return new MatchResult
                    {
                        connectedGems = extraConnectedGems,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedGems = _matchedResults.connectedGems,
                direction = _matchedResults.direction,
            };
        }
        if (_matchedResults.direction == MatchDirection.Vertical || _matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Gem gem in _matchedResults.connectedGems)
            {
                List<Gem> extraConnectedGems = new();
                CheckDirection(gem, new Vector2Int(1, 0), extraConnectedGems);

                CheckDirection(gem, new Vector2Int(-1, 0), extraConnectedGems);

                if (extraConnectedGems.Count >= 2)
                {
                    Debug.Log("I have a super vertical Match");
                    if (isGameStart)
                    {
                        Debug.Log("PLAYING SOUNDS");
                        popSoundSource.Play();
                    }

                    extraConnectedGems.AddRange(_matchedResults.connectedGems);
                    return new MatchResult
                    {
                        connectedGems = extraConnectedGems,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedGems = _matchedResults.connectedGems,
                direction = _matchedResults.direction,
            };
        }
        return null;
    }

    MatchResult IsConnected(Gem gem)
    {
        List<Gem> connectedGems = new();
        GemType gemType = gem.gemtype;

        connectedGems.Add(gem);

        //check right
        CheckDirection(gem, new Vector2Int(1, 0), connectedGems);
        //check left
        CheckDirection(gem, new Vector2Int(-1, 0), connectedGems);
        //have we made a 3 match? (Horizontal Match)
        if (connectedGems.Count == 3)
        {
            Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedGems[0].gemtype);
            if (isGameStart)
            {
                Debug.Log("PLAYING SOUNDS");
                popSoundSource.Play();
            }
            return new MatchResult
            {
               
                connectedGems = connectedGems,
                direction = MatchDirection.Horizontal
            };
        }
        //checking for more than 3 (Long horizontal Match)
        else if (connectedGems.Count > 3)
        {
            Debug.Log("I have a Long horizontal match, the color of my match is: " + connectedGems[0].gemtype);
            if (isGameStart)
            {
                Debug.Log("PLAYING SOUNDS");
                popSoundSource.Play();
            }

            return new MatchResult
            {
                connectedGems = connectedGems,
                direction = MatchDirection.LongHorizontal
            };
        }
        //clear out the connectedgems
        connectedGems.Clear();
        //readd our initial gem
        connectedGems.Add(gem);

        //check up
        CheckDirection(gem, new Vector2Int(0, 1), connectedGems);
        //check down
        CheckDirection(gem, new Vector2Int(0, -1), connectedGems);

        //have we made a 3 match? (Vertical Match)
        if (connectedGems.Count == 3)
        {
            Debug.Log("I have a normal vertical match, the color of my match is: " + connectedGems[0].gemtype);
            if (isGameStart)
            {
                Debug.Log("PLAYING SOUNDS");
                popSoundSource.Play();
            }

            return new MatchResult
            {
                connectedGems = connectedGems,
                direction = MatchDirection.Vertical
            };
        }
        //checking for more than 3 (Long Vertical Match)
        else if (connectedGems.Count > 3)
        {
            Debug.Log("I have a Long vertical match, the color of my match is: " + connectedGems[0].gemtype);
            if (isGameStart)
            {
                Debug.Log("PLAYING SOUNDS");
                popSoundSource.Play();
            }

            return new MatchResult
            {
                connectedGems = connectedGems,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedGems = connectedGems,
                direction = MatchDirection.None
            };
        }
    }

    void CheckDirection(Gem pot, Vector2Int direction, List<Gem> connectedGems)
    {
        GemType gemType = pot.gemtype;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        //check that we're within the boundaries of the board
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (gemBoard[x, y].isUsable)
            {
                Gem neighbourGem = gemBoard[x, y].gem.GetComponent<Gem>();

                //does our gemType Match? it must also not be matched
                if (!neighbourGem.isMatched && neighbourGem.gemtype == gemType)
                {
                    connectedGems.Add(neighbourGem);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }

            }
            else
            {
                break;
            }
        }
    }

    #region SwappingLogic

    public void SelectGem(Gem _gem)
    {
        if (isProcessingMove)
        {
            return;
        }

        if (selectedGem == null)
        {
            selectedGem = _gem;
        }
        else if (selectedGem == _gem)
        {
            selectedGem = null;
        }
        else if (selectedGem != _gem)
        {
            SwapGem(selectedGem, _gem);
            selectedGem = null;
        }
    }

    private void SwapGem(Gem _currentGem, Gem _targetGem)
    {
        if (!IsAdjacent(_currentGem, _targetGem))
        {
            //Debug.Log("not adjacent");
            return;
        }

        DoSwap(_currentGem, _targetGem);

        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentGem, _targetGem));
    }

    private void DoSwap(Gem _currentGem, Gem _targetGem)
    {
        GameObject temp = gemBoard[_currentGem.xIndex, _currentGem.yIndex].gem;

        gemBoard[_currentGem.xIndex, _currentGem.yIndex].gem = gemBoard[_targetGem.xIndex, _targetGem.yIndex].gem;
        gemBoard[_targetGem.xIndex, _targetGem.yIndex].gem = temp;

        int tempXIndex = _currentGem.xIndex;
        int tempYIndex = _currentGem.yIndex;

        _currentGem.xIndex = _targetGem.xIndex;
        _currentGem.yIndex = _targetGem.yIndex;
        _targetGem.xIndex = tempXIndex;
        _targetGem.yIndex = tempYIndex;

        //moves current gem to target gem (physically on the screen)
        _currentGem.MoveToTarget(gemBoard[_targetGem.xIndex, _targetGem.yIndex].gem.transform.position);

        //moves target gem to current gem (physically on the screen)
        _targetGem.MoveToTarget(gemBoard[_currentGem.xIndex, _currentGem.yIndex].gem.transform.position);
    }

    private bool IsAdjacent(Gem _currentGem, Gem _targetGem)
    {
        return Mathf.Abs(_currentGem.xIndex - _targetGem.xIndex) + Mathf.Abs(_currentGem.yIndex - _targetGem.yIndex) == 1;
    }

    private IEnumerator ProcessMatches(Gem _currentGem, Gem _targetGem)
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckBoard())
        {

            //start a caroutine to process our matches
            StartCoroutine(ProcessTurnOnMatchedBoard(true));

        }
        else
        {
            DoSwap(_currentGem, _targetGem);
        }
        isProcessingMove = false;
    }

    #endregion


}

public class MatchResult
{
    public List<Gem> connectedGems;
    public MatchDirection direction;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}


