using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConwayManager : MonoBehaviour
{

    #region ==== Public Options ====------------------

    public int deathbyLoneliness = 2;
    public int deathbyOverpopulation = 4;
    public int minimumBirth = 3;
    public int maximumBirth = 3;

    public float _generationInterval = 1.0f;
    
    public Vector3Int _seedSize = new Vector3Int(10, 10, 10); 
    public float _seedProbability = 0.5f;
    
    #endregion -----------------/Public Options ====


    #region ==== Private storage ====------------------
    
    private HashSet<Vector3Int> _livingCells = new HashSet<Vector3Int>();
    private List<GameObject> _cellObjects = new List<GameObject>();

    #endregion -----------------/Private storage ====
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeSeed();
        VisualizeCells();
    }

    
    float _lastGenerationTime = 0;
    // Update is called once per frame
    void Update()
    {
        //generation loop
        if (Time.time - _lastGenerationTime > _generationInterval)
        {
            _lastGenerationTime = Time.time;
            EvaluateGeneration();
        }
    }

    public void InitializeSeed()
    {
        for (int x = -_seedSize.x / 2; x < _seedSize.x / 2; x++)
        {
            for (int y = -_seedSize.y / 2; y < _seedSize.y / 2; y++)
            {
                for (int z = -_seedSize.z / 2; z < _seedSize.z / 2; z++)
                {
                    if (Random.value < _seedProbability)
                    {
                        _livingCells.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }
    
    public void EvaluateGeneration()
    {
        //gather all cells to check
        HashSet<Vector3Int> cellsToCheck = new HashSet<Vector3Int>();
        HashSet<Vector3Int> resultSet = new HashSet<Vector3Int>();
        
        foreach (var cell in _livingCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        cellsToCheck.Add(cell + new Vector3Int(x, y, z));
                    }
                }
            }
        }
        
        //count living neighbors
        foreach (var candidate in cellsToCheck)
        {
            CheckCandidate(candidate);
        }
        _livingCells = resultSet;
        
        VisualizeCells();


        void CheckCandidate(Vector3Int candidate)
        {
            int livingNeighbors = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue;
                        if (_livingCells.Contains(candidate + new Vector3Int(x, y, z)))
                        {
                            livingNeighbors++;
                        }
                    }
                }
            }
            
            if(_livingCells.Contains(candidate))
            {
                if (livingNeighbors <= deathbyLoneliness || livingNeighbors >= deathbyOverpopulation)
                {
                    //do nothing
                }
                else
                {
                    resultSet.Add(candidate);
                }
            }
            else //not already living
            {
                if (livingNeighbors >= minimumBirth && livingNeighbors <= maximumBirth)
                {
                    resultSet.Add(candidate);
                }
            }
            
        }
    }

    private void VisualizeCells()
    {
        //clear and visualize
        foreach (var cell in _cellObjects)
        {
            Destroy(cell);
        }
        _cellObjects.Clear();
        foreach (var livingCell in _livingCells)
        {
            GameObject cellVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cellVis.transform.parent = transform;
            cellVis.transform.position = livingCell;
        }
    }
}
