using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    COLLECTABLE,
    HOLDING,
    NOT_COLLECTABLE
}

public abstract class Item : MonoBehaviour
{
    [Header("Item Refs")]
    [SerializeField] protected GameObject _visuals;
    [SerializeField] public ItemState itemState;
    [SerializeField] protected bool _isConsumeOnPickup; //true if item is "used" on pickup and not put in inventory for manual use

    protected GameObject _playerRef;
    protected Collider _colliderRef;

    protected void OnCollect()
    {
        //TODO: play sound, aquire item
        _visuals.gameObject.SetActive(false);
        _colliderRef.enabled = false;
        //Destroy(this.gameObject);
    }

    public abstract void UseItem();
}
