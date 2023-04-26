using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenuRef;
    [SerializeField] private WeaponManager _weaponManagerRef;

    bool _gamePaused;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MenuInputs();
        SelectWeaponInput();
    }

    //interacting with menus
    void MenuInputs()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_gamePaused)
            {
                _pauseMenuRef.OnPauseGame();
                _gamePaused = false;
            }
            else
            {
                _pauseMenuRef.OnResumeGame();
                _gamePaused = true;
            }
        }
    }

    //swapping weapons
    void SelectWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponManagerRef.SwapWeapons(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponManagerRef.SwapWeapons(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _weaponManagerRef.SwapWeapons(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _weaponManagerRef.SwapWeapons(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _weaponManagerRef.SwapWeapons(4);
        }
    }

}
