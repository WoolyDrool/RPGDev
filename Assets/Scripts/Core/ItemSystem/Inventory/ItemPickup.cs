using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Items
{
    public class ItemPickup : MonoBehaviour
    {
        public ItemData data;
        public static string ID;
        public GameObject refModel;
        void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            ID = data.itemID;
            gameObject.name = data.name;
            gameObject.AddComponent<BoxCollider>();
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.tag = "Item";

            GameObject weaponInstance = Instantiate(refModel, this.transform.position, this.transform.rotation);
            weaponInstance.transform.parent = this.transform;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
