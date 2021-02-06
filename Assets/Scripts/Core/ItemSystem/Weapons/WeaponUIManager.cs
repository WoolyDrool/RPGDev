using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUIManager : MonoBehaviour
{
    public TextMeshProUGUI clipAmmo;
    public TextMeshProUGUI reserveAmmo;
    public Slider reloadProgress;
    // Start is called before the first frame update
    void Start()
    {
        clipAmmo.gameObject.SetActive(false);
        reserveAmmo.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
