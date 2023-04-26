using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBuffsUI : MonoBehaviour
{
    [SerializeField] private Image[] _buffImages;

    Item[] _buffs;
    int _curIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //add buff to end of list
    public void AddBuff(Item itemRef)
    {
        _buffs[_curIndex] = itemRef;
        //_buffImages[_curIndex].sprite = itemRef.buffIcon;
        _curIndex++;
    }

    //remove given buff and reorganize list
    public void RemoveBuffImage()
    {
        _curIndex--;
    }
}
