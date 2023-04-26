using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InjectionUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TraitManager _traitManager;
    [SerializeField] private Image[] _traitImages;
    [SerializeField] private TextMeshProUGUI[] _traitNames;
    [SerializeField] private TextMeshProUGUI _traitDescription;
    [SerializeField] private Trait _placeholderTrait;

    Trait[] _traitSelection;

    // Start is called before the first frame update
    void Start()
    {
        _traitDescription.text = "Select a trait to inject.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //set UI active and display passed traits
    public void DisplayInjectionUI(Trait[] injectionTraits)
    {
        gameObject.SetActive(true);
        _traitSelection = injectionTraits;

        //set relevant data
        for (int i = 0; i < 3; i++)
        {
            //trait
            if(i < injectionTraits.Length)
            {
                _traitImages[i].sprite = injectionTraits[i].sprite;
                _traitNames[i].text = injectionTraits[i].traitName;

            }
            //no trait to pass to slot, use placeholder
            else
            {
                _traitImages[i].sprite = _placeholderTrait.sprite;
                _traitNames[i].text = _placeholderTrait.traitName;
            }
        }
    }

    //update UI to display data of trait hovered
    public void OnHover(int index)
    {
        if (index < _traitSelection.Length)
        {
            _traitDescription.text = _traitSelection[index].description;
        }
    }

    //choose trait and close UI
    public void ChooseTrait(int index)
    {
        if(index < _traitSelection.Length)
        {
            _traitManager.ActivateTrait(_traitSelection[index].name);
            gameObject.SetActive(false);
        }     
    }
}
