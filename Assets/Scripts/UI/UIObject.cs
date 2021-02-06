using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPGSystem.UI
{
    [CreateAssetMenu(menuName = "UI/UI Object")]
    public class UIObject : ScriptableObject
    {
        public GameObject UIScene;
        public TextMeshProUGUI[] textComponents;
        public Slider[] sliderComponents;
        public GridLayoutGroup[] layoutGroups;

        public void Init()
        {
            if (UIScene != null)
            {
                textComponents = UIScene.GetComponentsInChildren<TextMeshProUGUI>();
                sliderComponents = UIScene.GetComponentsInChildren<Slider>();
                layoutGroups = UIScene.GetComponentsInChildren<GridLayoutGroup>();
            }
        }

    }
}
