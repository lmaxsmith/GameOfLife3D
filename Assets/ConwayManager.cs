using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConwayManager : MonoBehaviour
{

    #region ==== Public Options ====------------------

    [Header("Rules")]
    public int deathbyLoneliness = 2;
    public int deathbyOverpopulation = 4;
    public int minimumBirth = 3;
    public int maximumBirth = 3;

    public float _generationInterval = 1.0f;
    
    [Header("Setup")]
    public Vector3Int _seedSize = new Vector3Int(10, 10, 10); 
    public float _seedProbability = 0.5f;
    
    [Header("Visuals")]
    public CellVis _cellPrefab;
    public List<Material> _cellMaterials;
    #endregion -----------------/Public Options ====

    #region ==== Reporting ====------------------

    [Header("Reporting")]
    public int _generationCount = 0;
    public int _livingCellCount = 0;

    #endregion -----------------/Reporting ====
    

    #region ==== Private storage ====------------------
    
    private Dictionary<Vector3Int, CellVis> _livingCells = new Dictionary<Vector3Int, CellVis>();
    

    #endregion -----------------/Private storage ====
    
    // Start is called before the first frame update
    void Start()
    {
        //InitializeSeed();
        //StartSimulating();
    }

    
    float _lastGenerationTime = 0;
    // Update is called once per frame
    void Update()
    {
        //generation loop
        if (Time.time - _lastGenerationTime > _generationInterval && _isSimulating)
        {
            _lastGenerationTime = Time.time;
            EvaluateGeneration();
        }
    }

    
    public void StartSimulating()
    {
        _isSimulating = true;
    }
    bool _isSimulating = false;
    
    public void StopSimulating()
    {
        _isSimulating = false;
    }
    
    public void Clear()
    {
        foreach (var cell in _livingCells)
        {
            Destroy(cell.Value.gameObject);
        }
        _livingCells.Clear();
        
        _generationCount = 0;
        _livingCellCount = 0;
    }
    
    

    public void IterateOnNeighbors(Vector3Int position, System.Action<Vector3Int> action)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    //if (x == 0 && y == 0 && z == 0) continue;
                    action(position + new Vector3Int(x, y, z));
                }
            }
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
                        CellBirth(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }
    
    public void EvaluateGeneration()
    {
        HashSet<Vector3Int> cellsToCheck = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, CellAction> resultDic = new Dictionary<Vector3Int, CellAction>();
        
        //gather all cells to check
        foreach (var cell in _livingCells)
        {
            IterateOnNeighbors(cell.Key, (neighbor) =>
                { cellsToCheck.Add(neighbor); });
        }
        
        //check each cell
        foreach (var candidate in cellsToCheck)
        {
            CheckCandidate(candidate);
        }

        //visualize results
        foreach (var result in resultDic)
        {
            switch (result.Value)
            {
                case CellAction.birth:
                    CellBirth(result.Key);
                    break;
                case CellAction.death:
                    CellDeath(result.Key);
                    break;
                case CellAction.stayinAlive:
                    CellStayinAlive(result.Key);
                    break;
            }
        }
        
        //reporting
        _generationCount++;
        _livingCellCount = _livingCells.Count;

        
        

        void CheckCandidate(Vector3Int candidate)
        {
            int livingNeighbors = 0;

            //count neighbors
            IterateOnNeighbors(candidate, (neighbor) =>
            {
                if (_livingCells.ContainsKey(neighbor))
                    livingNeighbors++;
            });
            
            
            if(_livingCells.ContainsKey(candidate)) //already living
            {
                _livingCells[candidate]._neighbors = livingNeighbors;
                
                if (livingNeighbors <= deathbyLoneliness || livingNeighbors >= deathbyOverpopulation)
                    resultDic[candidate] = CellAction.death;
                else
                    resultDic[candidate] = CellAction.stayinAlive;
            }
            else //not already living
            {
                if (livingNeighbors >= minimumBirth && livingNeighbors <= maximumBirth)
                    resultDic[candidate] = CellAction.birth;
            }
        }
    }

    #region ==== Cell Vis Actions ====------------------


    public void CellBirth(Vector3Int position)
    {
        CellVis newCellVis = Instantiate(_cellPrefab, position, Quaternion.identity);
        newCellVis.transform.parent = transform;
        newCellVis.transform.localPosition = position;
        newCellVis.transform.localScale = Vector3.one;
        
        newCellVis.Initialize(position, this);
        _livingCells[position] = newCellVis;
    }
    
    
    public void CellDeath(Vector3Int position)
    {
        if (_livingCells.ContainsKey(position))
        {
            Destroy(_livingCells[position].gameObject);
            _livingCells.Remove(position);
        }
    }
    
    public void CellStayinAlive(Vector3Int position)
    {
        if (_livingCells.ContainsKey(position))
        {
            _livingCells[position].Increment();
        }
    }
    
    public enum CellAction
    {
        birth, death, stayinAlive
    }
    

    #endregion -----------------/Cell Vis Actions ====
    
}