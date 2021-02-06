using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class QuestItem : MonoBehaviour
{
    public string variableName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            SetFlag();
        }
    }

    void SetFlag()
    {
        Debug.Log("Set");
        DialogueLua.SetVariable(variableName, true);
    }
}
