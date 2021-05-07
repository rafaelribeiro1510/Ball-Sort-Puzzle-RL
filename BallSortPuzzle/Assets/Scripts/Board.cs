using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private int tubeH, nTubes, nColors;
    private List<List<BallColor>> _tubes;
    
    private int _width;
    private Transform _background;

    [SerializeField] private GameObject tubePrefab;
    [SerializeField] private GameObject ballPrefab;
    
    private void Awake()
    {
        _background = transform.GetChild(0);
    }

    private void Start()
    {

        InstantiateTubes();
        RandomizeBalls();
    }

    private void InstantiateTubes()
    {
        for (var i = 1; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        
        if (nColors > nTubes) throw new Exception("Board can't have more colors than tubes");
        else if (nColors * tubeH < nColors) throw new Exception("There must be at least one piece of each color");
        else if (nColors * tubeH > nTubes * tubeH) throw new Exception("Too many pieces");

        _tubes = new List<List<BallColor>>();
        for (var i = 0; i < nTubes; i++)
        {
            _tubes.Add(new List<BallColor>(tubeH));
        }
        
        _width = nTubes * 3;
        _background.localScale = new Vector3(_width, tubeH * 3, 0.2f); // Z hardcoded
        for (var i = 0; i < nTubes; i++)
        {
            var newTube = Instantiate(tubePrefab, tubePosByIndex(i), // Z hardcoded 
                Quaternion.identity, transform);
            
            var script = newTube.GetComponent<Tube>(); 
            script.Height = tubeH / 2;
            script.Resize();

        }
    }

    private Vector3 tubePosByIndex(int i)
    {
        return new Vector3(i * (_width / nTubes) - _width / 2 + 1, 0, -0.6f);
    }
    
    private Vector3 tubePosByIndex(int i, int h)
    {
        return new Vector3(i * (_width / nTubes) - _width / 2 + 1, h - tubeH/2 + 0.4f, -0.6f);
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
                } while (_tubes[tube].Count >= tubeH);
                _tubes[tube].Add(iColor);
                --numPiecesPerColor[iColor];
            }
        }

        for (var i = 0 ; i < _tubes.Count ; i++)
        {
            var tube = _tubes[i];
            for (var j = 0 ; j < tube.Count ; j++)
            {
                var newBall = Instantiate(ballPrefab, tubePosByIndex(i, j), 
                    Quaternion.identity, transform);
                newBall.GetComponent<Ball>().SetColor(tube[j]); 
            }
        }
    }
}
