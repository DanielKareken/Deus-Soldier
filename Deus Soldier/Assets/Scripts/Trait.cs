using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitTier
{
    INHERENT,
    CORE,
    ADVANCED,
    SUPERHUMAN
}

[CreateAssetMenu(menuName = "new Trait")] 
public class Trait : ScriptableObject
{
    public string traitName;
    public float modifierValue; //value associated with trait's modification of a mechanic
    public float maxLevel = 1;
    [Range(1, 5)] public int traitLevel;
    public TraitTier traitTier;
    public bool isAquired = false;
    public Sprite sprite;
    public string description;
}
