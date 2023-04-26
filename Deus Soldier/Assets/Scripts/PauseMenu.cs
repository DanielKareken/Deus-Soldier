using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPauseGame()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnResumeGame()
    {
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
    }
}
