using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.VFX
{
    public class WeaponVFX : MonoBehaviour
    {
        #region Recoil
        [Header("References")]
        //Starting position reference
        public Transform recoilPosition;
        //Starting rotation reference
        public Transform rotationPoint;

        [Header("Speed")]
        //Speed of the recoil position changes
        public float positionalRecoilSpeed = 8f;
        //Speed of the recoil rotation changes
        public float rotationalRecoilSpeed = 8f;
        //Speed of the positional return
        public float positionalReturnSpeed = 18f;
        //Speed of the rotaitonal return
        public float rotationalReturnSpeed = 38f;

        [Header("Aiming")]


        [Header("Modifiers")]
        public float kickBackPowerHip = 10;
        public float kickBackPowerAim = 4;
        //The +- rotational changes to be applied
        Vector3 RecoilRotation;
        //The +- kickback changes to be applied
        Vector3 RecoilKickBack;

        //The +- rotational changes to be applied while aiming
        Vector3 RecoilRotationAim;
        //The +- kickback changes to be applied while aiming
        Vector3 RecoilKickBackAim;

        //Positional data container
        Vector3 rotationalRecoil;
        //Rotational data container
        Vector3 positionalRecoil;
        //Calculated rotation to apply
        Vector3 Rot;
        #endregion

        [Header("Sway")]
        #region Sway
        public float swayAmount;
        public float tiltScale;
        public float swayMaxAmount;
        public float swaySmoothAmount;

        private Vector3 initialPosition;
        #endregion

        //Aiming switch
        public bool aiming;

        //Instance
        private WeaponVFX vfx;

        void Awake()
        {
            //ReceiveVectors();

            //Set instance
            vfx = this;
        }

        void Start()
        {
            initialPosition = transform.localPosition;

        }

        public void ReceiveVectors()
        {
            RecoilRotation = new Vector3(kickBackPowerHip, kickBackPowerHip / 2, kickBackPowerHip * 0.7f);
            RecoilKickBack = new Vector3(kickBackPowerHip * 0.015f, 0f, -kickBackPowerHip * 0.2f);
            RecoilRotationAim = new Vector3(kickBackPowerAim, kickBackPowerAim * 0.4f, kickBackPowerAim * 0.6f);
            RecoilKickBackAim = new Vector3(kickBackPowerAim * 0.015f, 0f, -kickBackPowerAim * 0.2f);
        }

        void FixedUpdate()
        {
            //Apply the positional changes at a fixed speed
            rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
            //Apply the rotational changes at a fixed speed
            positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

            //Change the position of the weapon at a fixed speed
            recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);

            //Calculate the rotation vector at a fixed speed
            Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
            //Conver the vector to a euler angle
            rotationPoint.localRotation = Quaternion.Euler(Rot);

            #region Sway Calculation
            float movementX = -Input.GetAxis("Mouse X") * swayAmount;
            float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
            movementX = Mathf.Clamp(movementX, -swayMaxAmount, swayMaxAmount);
            movementY = Mathf.Clamp(movementY, -swayMaxAmount, swayMaxAmount);


            Vector3 finalPosition = new Vector3(0, movementY, movementX);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * swaySmoothAmount);
            transform.localRotation = Quaternion.Euler(0, 90, movementX * tiltScale);
            #endregion


        }

        void Update()
        {

        }

        public void Fire()
        {
            //Called from weapon scripts 

            //While the player is aiming
            if (aiming)
            {
                //Create a random rotation modifier using the RecoilRotationAim XYZ positions
                rotationalRecoil += new Vector3(-RecoilRotationAim.x, Random.Range(-RecoilRotationAim.y, RecoilRotationAim.y), Random.Range(-RecoilRotationAim.z, RecoilRotationAim.z));
                //Create a random position modifier using the RecoilKickBackAim XYZ positions
                positionalRecoil += new Vector3(-RecoilKickBackAim.x, Random.Range(-RecoilKickBackAim.y, RecoilKickBackAim.y), Random.Range(-RecoilKickBackAim.z, RecoilKickBackAim.z));
            }
            else
            {
                //Create a random rotation modifier using the RecoilRotation XYZ positions
                rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotationAim.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
                //Create a random position modifier using the RecoilKickBack XYZ positions
                positionalRecoil += new Vector3(-RecoilKickBack.x, Random.Range(-RecoilKickBack.y, RecoilKickBack.y), Random.Range(-RecoilKickBack.z, RecoilKickBack.z));
            }
        }
    }
}
