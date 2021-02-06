using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace RPGSystem.Core.Player
{
    public class LuaPlayerTracker : MonoBehaviour
    {
        public PlayerCharacterSheet player;
        DialogueDatabase dialogueDatabase;
        void Awake()
        {
            DialogueLua.SetVariable("FSkill", (int)player.fWeapons);
            
        }
    }
}
