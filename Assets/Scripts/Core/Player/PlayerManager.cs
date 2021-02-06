using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using PixelCrushers.DialogueSystem;

namespace RPGSystem.Core.Player
{
    //Disable a warning message that means nothing
#pragma warning disable 0414
    [System.Serializable]
    public class PlayerManager : MonoBehaviour
    {
        public PlayerCharacterSheet playerCharacterSheet;
        public GameStateManager gameStateManager;
        //public PlayerData playerDataInstance;

        [Space(5)]
        [Header("Stats")]
        private double health;
        public float currentHealth;
        private double maxHealth;

        public double stamina;
        private double maxStamina;

        public double sanity;
        private double maxSanity;

        public double damageResistance;
        public double staminaJumpCost;

        [Space(5)]
        [Header("First Person Controller")]
        public float moveSpeed = 5;
        public float moveSpeedRun = 7;
        public float moveSpeedCrouch = 2;
        public float jumpSpeed = 7;
        public float mouseSensitivity = 3;
        public float gravityScale = 10;
        bool isCrouching;
        float currentSpeed;
        bool isWalking = false;
        public float clampAngle = 80;
        CharacterController cc;
        public GameObject camHolder;
        [SerializeField] public bool canMove = true;
        float newCalcMoveSpeed;
        Vector3 moveDirection = Vector3.zero;
        float rotationX = 0;

        [Header("UI")]
        public Slider staminaBar;
        public Slider healthBar;

        [Space(5)]
        [Header("Regeneration")]
        double staminaRegenDelay = 2;
        private Coroutine regen;
        double staminaRegenAmount = 0.1f;
        public double staminaRegenSpeed = 2;
        private WaitForSeconds regenTick;

        //[Header("DevStats")]

        void Awake()
        {
            //playerData = ScriptableObject.CreateInstance<PlayerData>();
            cc = GetComponent<CharacterController>();
            gameStateManager.manager = this;

        }

        // Start is called before the first frame update
        void Start()
        {
            regenTick = new WaitForSeconds(0.01f);
            SetVariables();
            gameStateManager.HideMouse();
            gameStateManager.ResumeGame();
        }

        //Initialize all variables from the SO
        public void SetVariables()
        {
            playerCharacterSheet.ReadVariables();
            health = playerCharacterSheet.health;
            maxHealth = playerCharacterSheet.maxHealth;
            currentHealth = Mathf.Clamp(currentHealth, 0, 54);
            currentHealth += (float)playerCharacterSheet.health;
            maxStamina = playerCharacterSheet.maxStamina;
            staminaRegenSpeed = playerCharacterSheet.regenSpeed;
            damageResistance = playerCharacterSheet.damageResistance;
            staminaJumpCost = playerCharacterSheet.staminaJumpCost;
            stamina = playerCharacterSheet.stamina;
            moveSpeed = (float)playerCharacterSheet.movementSpeed;
            staminaBar.maxValue = (float)playerCharacterSheet.maxStamina;
            staminaBar.value = (float)playerCharacterSheet.maxStamina;
            //This will be replaced with a playerData value once the stat is implemented fully
            newCalcMoveSpeed = moveSpeed;
            moveSpeedRun = newCalcMoveSpeed + 2;
            moveSpeedCrouch = newCalcMoveSpeed / 2.5f;
            healthBar.maxValue = (float)playerCharacterSheet.maxHealth;
            healthBar.value = (float)playerCharacterSheet.health;
            //StartCoroutine(RegenStamina());
        }

        // Update is called once per frame
        void Update()
        {
            float mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseInputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
            Vector3 right = transform.TransformDirection(Vector3.right).normalized;

            bool isMoving = Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") > 0;
            bool isRunning = Input.GetButton("Sprint") && stamina >= 1;

            float curSpeedX = currentSpeed * Input.GetAxis("Vertical");
            float curSpeedY = currentSpeed * Input.GetAxis("Horizontal");

            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && cc.isGrounded && stamina >= staminaJumpCost)
            {
                moveDirection.y = jumpSpeed;
                UseStaminaJump();
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!cc.isGrounded)
            {
                moveDirection.y -= gravityScale * Time.deltaTime;
                float inAirSpeed = moveSpeed / 2;
                currentSpeed = inAirSpeed;
            }

            cc.Move(moveDirection * Time.deltaTime);

            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity;
                rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);
                camHolder.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);

                if (Input.GetButton("Crouch"))
                {
                    Crouch();
                }
                else
                {
                    Standup();
                }

                if (Input.GetButton("Sprint") && isMoving)
                {
                    Standup();
                    if (stamina >= 0 && !isCrouching)
                    {
                        currentSpeed = moveSpeedRun;
                        stamina -= Time.deltaTime * 10;
                        staminaBar.value = (float)stamina;
                        Mathf.Clamp((int)stamina, 0, 999);

                        if (regen != null)
                        {
                            //StopCoroutine(RegenStamina());
                            regen = null;
                        }
                    }
                    else
                    {
                        currentSpeed = moveSpeed;
                    }
                }

                if (!isRunning && stamina < maxStamina)
                {
                    if (regen != null)
                    {
                        StopCoroutine(RegenStamina());
                    }
                    else if (regen == null)
                    {
                        regen = StartCoroutine(RegenStamina());
                    }
                }
            }

        }

        void Crouch()
        {
            cc.height = 0;
            isCrouching = true;
            currentSpeed = moveSpeedCrouch;
        }

        void Standup()
        {
            cc.height = 2;
            isCrouching = false;
            currentSpeed = newCalcMoveSpeed;
        }

        public void UseStaminaJump()
        {
            if (stamina - staminaJumpCost >= 1)
            {
                stamina -= staminaJumpCost;
                staminaBar.value = (float)stamina;

                if (regen != null)
                {
                    StopCoroutine(regen);
                }

                regen = StartCoroutine(RegenStamina());
            }
            else
            {
                Debug.Log("Not enough stamina");
            }
        }

        private IEnumerator RegenStamina()
        {
            yield return new WaitForSeconds((float)staminaRegenDelay);

            while (stamina < maxStamina && regen != null)
            {
                stamina += staminaRegenAmount;
                staminaBar.value = (float)stamina;
                stamina = Mathf.Clamp((float)stamina, 0, (float)maxStamina);
                yield return regenTick;
            }
            regen = null;

            //StopCoroutine(RegenStamina());

        }

        public void TakeSpeedPenalty(bool penalty)
        {
            if (penalty)
            {
                currentSpeed -= (currentSpeed * 0.50f);
            }
            else if (!penalty)
            {
                currentSpeed = newCalcMoveSpeed;
            }
        }

        public void TakeDamage(double damageToTake)
        {
            double calcDamage = damageToTake - Mathf.Round((float)damageResistance * 100f) / 100f;
            currentHealth -= (float)calcDamage;
            Debug.Log("Player took " + damageToTake.ToString());

            healthBar.value = currentHealth;
        }

        private IEnumerator Sprint()
        {
            stamina--;
            yield return new WaitForSeconds(5f);
        }

        public void DisableMovement()
        {
            canMove = false;
        }

        public void EnableMovement()
        {
            canMove = true;
        }
    }
}
