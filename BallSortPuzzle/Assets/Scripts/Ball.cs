using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private MeshRenderer _renderer;
    [SerializeField] public BallColor color;
    [Serializable] public struct MakeShiftDictionaryEntry
    {
        public BallColor key;
        public Material materials;
    }
    
    [SerializeField] private List<MakeShiftDictionaryEntry> spriteDictionary = new List<MakeShiftDictionaryEntry>();
    private readonly Dictionary<BallColor, Material> _colors = new Dictionary<BallColor, Material>();
    
    private void Awake() 
    {
        _renderer = GetComponent<MeshRenderer>();
        foreach (var dictEntry in spriteDictionary) _colors.Add(dictEntry.key, dictEntry.materials);
        _renderer.material = _colors[color];
    }

    public void SetColor(BallColor _color)
    {
        color = _color;
        _renderer.material = _colors[_color];
    }
}
