using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public enum State {
        Start = 0,
        OriginTubeSelected = 1,
        MoveMade = 2,
        GameOver = 3
    };
    
    [SerializeField] [Range(3,5)] public int tubeH;
    [SerializeField] [Range(3,10)] private int nTubes;
    [SerializeField] [Range(2,7)] private int nColors;
    public List<Stack<BallColor>> Tubes { get; private set; }
    public List<Tube> TubesScripts { get; private set; }

    private int _width;
    private Transform _background;
    private MeshRenderer _backgroundRenderer;
    private Material defaultMat;
    [SerializeField] private Material successMat, failureMat;

    [SerializeField] private GameObject tubePrefab;
    [SerializeField] private GameObject ballPrefab;
    
    private void Awake()
    {
        _background = transform.GetChild(0);
        _backgroundRenderer = _background.gameObject.GetComponent<MeshRenderer>();
        defaultMat = _backgroundRenderer.material;
    }

    public void InitializeBoardInXSeconds(int x)
    {
        print("Resettin board in " + x + " seconds");
        StartCoroutine(InitializeBoardInXSeconds_CR(x));
    }
    
    private IEnumerator InitializeBoardInXSeconds_CR(int x)
    {
        yield return new WaitForSeconds(x);
        InitializeBoard();
    }
    
    [ContextMenu("New Board")]
    public void InitializeBoard()
    {
        //RandomizeParameters();
        InstantiateTubes();
        RandomizeBalls();
        //_backgroundRenderer.material = defaultMat;
    }

    private void RandomizeParameters()
    {
        do
        {
            tubeH = Random.Range(3, 8);
            nTubes = Random.Range(3, 8);
            nColors = Random.Range(3, 8);
        } while (!validParameters());
    }

    private void InstantiateTubes()
    {
        if (!validParameters())
            throw new Exception("Invalid parameters");
        
        for (var i = 1; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        Tubes = new List<Stack<BallColor>>();
        TubesScripts = new List<Tube>();
        for (var i = 0; i < nTubes; i++)
        {
            Tubes.Add(new Stack<BallColor>(tubeH));
        }
        
        _width = nTubes * 3;
        _background.localScale = new Vector3(_width, tubeH * 3, 0.2f); // Z hardcoded
        var id = 0;
        for (var i = 0; i < nTubes; i++)
        {
            var newTube = Instantiate(tubePrefab, transform, false);
            newTube.transform.localPosition = tubePosByIndex(i);
            
            var script = newTube.GetComponent<Tube>(); 
            script.Height = tubeH / 2f;
            script.Resize();
            script.index = id;
            TubesScripts.Add(script);
            id++;
        }
    }

    private void RandomizeBalls()
    {
        int numPieces = nColors * tubeH;
        Dictionary<BallColor, int> numPiecesPerColor = new Dictionary<BallColor, int>();
        foreach (var color in Enum.GetValues(typeof(BallColor)))
        {
            numPiecesPerColor.Add((BallColor) color, tubeH);
        }

        for (var i = 0; i < nColors; i++)
        {
            var iColor = (BallColor) i;
            while (numPiecesPerColor[iColor] > 0)
            {
                int tube;
                do
                {
                    tube = (Random.Range(0, nTubes));
                } while (Tubes[tube].Count >= tubeH);
                Tubes[tube].Push(iColor);
                --numPiecesPerColor[iColor];
            }
        }

        for (var i = 0 ; i < Tubes.Count ; i++)
        {
            var tube = Tubes[i].ToList();
            for (var j = 0 ; j < tube.Count ; j++)
            {
                var newBall = Instantiate(ballPrefab, transform, false);
                newBall.transform.localPosition = tubePosByIndex(i, j);
                
                newBall.GetComponent<Ball>().SetColor(tube[j]);
                TubesScripts[i].Balls.Push(newBall);
            }
        }

        for(var i=0; i < Tubes.Count; i++) {
            var rev = new Stack<BallColor>();
            while (Tubes[i].Count != 0) {
                rev.Push(Tubes[i].Pop());
            }
            Tubes[i] = rev;
        }   
        
        if (GetAllMoves().Count == 0) InitializeBoard();
    }

    public bool CanMove(int from, int to)
    {
        if (from == to) return false;
        if (from >= nTubes || to >= nTubes) return false;

        return (Tubes[from].Count != 0 &&
                Tubes[to].Count < tubeH &&
                (Tubes[to].Count == 0 ||
                 Tubes[to].Peek() == Tubes[from].Peek()));
    }

    public List<Tuple<int, int>> GetAllMoves()
    {
        var result = new List<Tuple<int, int>>();

        if (Tubes.Count < 2) return result;
        for (var i = 0; i < Tubes.Count; i++)
        {
            for (var j = 1; j < Tubes.Count; j++)
            {
                if (i == j) continue;
                if (CanMove(i,j)) result.Add(new Tuple<int, int>(i, j));
            }
            
        }
        
        return result;
    }

    public bool IsGameOver()
    {
        foreach (var tube in Tubes)
        {
            var s = tube.Count;
            if (!(s == 0 || s == tubeH)) return false;
            var tubeList = tube.ToList();
            for (var j = 1; j < tubeList.Count; j++)
            {
                if (tubeList[j] != tubeList[0]) return false;
            }
        }

        return true;
    }
    
    private bool validParameters()
    {
        return (nColors <= nTubes) && (nColors * tubeH >= nColors) && (nColors * tubeH <= nTubes * tubeH);
    }
    
    private Vector3 tubePosByIndex(int i)
    {
        return new Vector3(i * (_width / nTubes) - _width / 2 + 1, 0, -0.6f);
    }
    
    private Vector3 tubePosByIndex(int i, int h)
    {
        return new Vector3(i * (_width / nTubes) - _width / 2 + 1, h - tubeH / 2f + 0.4f, -0.6f);
    }

    public void SetSuccessMat()
    {
        _backgroundRenderer.material = successMat;
    }
    
    public void SetFailureMat()
    {
        _backgroundRenderer.material = failureMat;
    }
}
