using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSystem.Core.Player;
using RPGSystem.Core.Inventory;
using RPGSystem.Core.Items;

[CreateAssetMenu(menuName = "Player/New GameState")]
public class GameStateManager : ScriptableObject
{
    public bool playerCanMove = true;
    public PlayerManager manager;
    public void FreezeGame()
    {
        //Time.timeScale = 0;
        playerCanMove = false;
        manager.canMove = false;
        
    }

    public void ResumeGame()
    {
        //Time.timeScale = 1;
        playerCanMove = true;
        manager.canMove = true;

    }

    public void RevealMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
