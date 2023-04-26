using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraitManager : MonoBehaviour
{    
    public List<Trait> traitList = new List<Trait>(); //for initialization
    public Dictionary<string, Trait> traitDict = new Dictionary<string, Trait>(); //for reference

    // Start is called before the first frame update
    void Awake()
    {
        InitializeTraitDict();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void InitializeTraitDict()
    {
        foreach(Trait trait in traitList)
        {
            if (trait == null)
                Debug.Log("Empty Trait Slot");
            else if (trait.traitName == "")
                Debug.Log("Forgot to put name in for trait");
            else
                traitDict.Add(trait.traitName, trait);
        }
    }

    //return if trait exists and has been aquired
    public bool TraitCheck(string traitName)
    {
        if(traitDict.TryGetValue(traitName, out Trait trait))
            if (trait.isAquired)
                return true;

        return false;
    }

    //return reference to a requested trait
    public Trait GetTrait(string traitName)
    {
        if (traitDict.TryGetValue(traitName, out Trait trait))
            return traitDict[traitName];
        else
            return null;
    }

    //activate a trait
    public void ActivateTrait(string traitName)
    {
        Trait trait = traitDict[traitName];
        if (trait != null)
        {
            //activate trait if inactive
            if (!trait.isAquired)
                trait.isAquired = true;      
            //if already active and trait is not max level, level++
            else if(trait.traitLevel < trait.maxLevel)
                trait.traitLevel++;
        }         
    }
}
