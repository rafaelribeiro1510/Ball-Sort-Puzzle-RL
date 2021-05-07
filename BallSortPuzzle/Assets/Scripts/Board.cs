using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int h, nTubes, nColors;

    private int _width;

    [SerializeField] private GameObject tubePrefab;
    
    private Transform _background;

    private void Awake()
    {
        _background = transform.GetChild(0);
    }

    private void Start()
    {
        if (nColors > nTubes) throw new Exception("Board can't have more colors than tubes");
        else if (nColors * h < nColors) throw new Exception("There must be at least one piece of each color");
        else if (nColors * h > nTubes * h) throw new Exception("Too many pieces");

        InstantiateTubes();
    }

    private void OnValidate()
    {
        _background = transform.GetChild(0);
        InstantiateTubes();
    }

    private void InstantiateTubes()
    {
        for (var i = 1; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        
        _width = nTubes * 3;
        _background.localScale = new Vector3(_width, h * 3, 0.2f); // Z hardcoded
        for (var i = 0; i < nTubes; i++)
        {
            var newTube = Instantiate(tubePrefab, new Vector3(i * (_width / nTubes) - _width/2 + 1.5f, 0, -0.6f), // Z hardcoded 
                Quaternion.identity, transform);
            
            var script = newTube.GetComponent<Tube>(); 
            script.Height = h;
            script.Resize();

        }
    }
}
