using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem.UI
{
    public class UIManager : MonoBehaviour
    {
        public Canvas globalCanvas;
        //GameObject canvasGameObject;
        // Start is called before the first frame update
        void Awake()
        {
            //canvasGameObject = globalCanvas.gameObject;

        }

        void Update()
        {

        }

        public void AddUIComponent(UIObject UiComponent)
        {
            //GameObject instance = Instantiate(UiComponent.UIScene, canvasGameObject.transform.position, canvasGameObject.transform.rotation);
            //instance.transform.SetParent(canvasGameObject.transform);
        }
    }
}
