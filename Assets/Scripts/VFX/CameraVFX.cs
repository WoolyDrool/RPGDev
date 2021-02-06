using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem.VFX
{
    public class CameraVFX : MonoBehaviour
    {
        [Header("Recoil Settings")]
        //Speed of the camera rotation
        public float rotationSpeed = 6;
        //Speed of the return to normal
        public float returnSpeed = 25;

        public Image fadeInImage;

        Camera mainCam;


        [Header("Hipfire")]
        //The +- rotation changes 
        public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);

        [Header("Aiming")]
        //The +- rotation changes 
        public Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 0.5f);
        public float aimSwitchSpeed = 6;
        float timeSinceStarted;
        float percentageComplete;
        //Aiming switch
        public bool aiming;
        public float aimFov = 60;
        public float startingFov;
        float currentFOV;
        bool isAiming;
        public Transform aimPoint;
        public Transform hipPoint;
        public Vector3 hipPos;
        public Vector3 aimPos;

        public Transform swingToPoint;

        //Current rotation data container
        private Vector3 currentRotation;
        //Calculated rotation data container
        private Vector3 Rot;

        //Instance
        private static CameraVFX cvfx;

        void Awake()
        {
            //Set instance
            cvfx = this;
            mainCam = Camera.main;
            startingFov = mainCam.fieldOfView;
        }

        void Start()
        {
            hipPos = new Vector3(hipPoint.localPosition.x, hipPoint.localPosition.y, hipPoint.localPosition.z);
            aimPos = new Vector3(aimPoint.localPosition.x, aimPoint.localPosition.y, aimPoint.localPosition.z);


        }

        void FixedUpdate()
        {
            fadeInImage.gameObject.SetActive(true);
            fadeInImage.CrossFadeAlpha(0, 2f, false);

            //Update the current rotation of the camera at a fixed speed
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            //Apply rotational 
            //Calculate the rotation vector at a fixed speed
            Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
            //Convert vector to euler angles
            transform.localRotation = Quaternion.Euler(Rot);

            if (aiming)
            {
                currentFOV = Mathf.Lerp(mainCam.fieldOfView, aimFov, aimSwitchSpeed * Time.deltaTime);
                mainCam.fieldOfView = currentFOV;
                hipPoint.localPosition = Vector3.Slerp(hipPoint.localPosition, aimPos, aimSwitchSpeed * Time.deltaTime);
            }
            else
            {
                hipPoint.transform.localPosition = Vector3.MoveTowards(hipPoint.transform.localPosition, hipPos, aimSwitchSpeed * Time.deltaTime);
                currentFOV = Mathf.Lerp(mainCam.fieldOfView, startingFov, aimSwitchSpeed * Time.deltaTime);
                mainCam.fieldOfView = currentFOV;
            }
        }

        public void Fire()
        {
            if (aiming)
            {
                //Create a random rotation modifier using the RecoilRotationAiming XYZ positions
                currentRotation += new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
            }
            else
            {
                //Create a random rotation modifier using the RecoilRotationAiming XYZ positions
                currentRotation += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            }
        }
    }
}
