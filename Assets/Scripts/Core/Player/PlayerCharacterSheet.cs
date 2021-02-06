using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.Core.Player
{
    public class PlayerCharacterSheet : MonoBehaviour
    {
        public PlayerData playerSaveState;
        public static PlayerCharacterSheet pcs;

        [Header("Character Sheet")]
        public string PlayerName;
        [Header("Attributes")]
        //Strength (Melee, carry weight, damage resistances)
        public int Strength = 5;
        //Dexterity (Melee, lockpicking)
        public int Dexterity = 5;
        //Constitution (Health, damage resistances)
        public int Constitution = 5;
        //Endurance (Stamina, health, damage resistances, psionic weapons)
        public int Endurance = 5;
        //Agility (Firearms, movement speed)
        public int Agility = 5;
        //Charisma (speech)
        public int Charisma = 5;
        //Luck (critical hit chance, XP gain)
        public int Luck = 5;
        public int carryWeight = 20;

        [Space(5)]
        [Header("Skills")]
        public double mWeapons = 10;
        public double fWeapons = 10;
        public double speech = 10;
        public double lockpick = 10;

        [Space(5)]
        [Header("Vitality")]
        public double regenSpeed;
        public double movementSpeed;
        public double health;
        public double stamina;
        public double maxHealth;
        public double maxStamina;
        public double damageResistance = 2;
        public double staminaJumpCost = 10;

        [Space(5)]
        [Header("Combat/XP")]
        public double xpMultiplier;
        public double weaponDamageMultiplier;
        public double critChance;


        [Space(5)]
        [Header("Multipliers")]
        public double strengthMultiplier;
        public double dexterityMultiplier;
        public double constituionMultiplier;
        public double enduranceMultiplier;
        public double agilityMultiplier;
        public double charismaMultiplier;
        public double luckMultiplier;

        [Space(5)]
        [Header("Level")]
        //Players Current Level
        public int curLevel = 1;
        public int curXP;
        public int xpNeededToLevelUp;

        void Awake()
        {
            //Singleton
            pcs = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            ReadVariables();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ReadVariables()
        {
            PlayerName = playerSaveState.PlayerName;
            Strength = playerSaveState.Strength;
            Dexterity = playerSaveState.Dexterity;
            Constitution = playerSaveState.Constitution;
            Endurance = playerSaveState.Endurance;
            Agility = playerSaveState.Agility;
            Charisma = playerSaveState.Charisma;
            Luck = playerSaveState.Luck;

            mWeapons = playerSaveState.mWeapons;
            fWeapons = playerSaveState.fWeapons;
            speech = playerSaveState.speech;
            lockpick = playerSaveState.lockpick;

            regenSpeed = playerSaveState.regenSpeed;
            movementSpeed = playerSaveState.movementSpeed;
            health = playerSaveState.maxHealth;
            stamina = playerSaveState.maxStamina;
            damageResistance = playerSaveState.damageResistance;

            xpMultiplier = playerSaveState.xpMultiplier;
            weaponDamageMultiplier = playerSaveState.weaponDamageMultiplier;
            critChance = playerSaveState.critChance;

            strengthMultiplier = playerSaveState.strengthMultiplier;
            dexterityMultiplier = playerSaveState.dexterityMultiplier;
            constituionMultiplier = playerSaveState.constituionMultiplier;
            enduranceMultiplier = playerSaveState.enduranceMultiplier;
            agilityMultiplier = playerSaveState.agilityMultiplier;
            charismaMultiplier = playerSaveState.charismaMultiplier;
            luckMultiplier = playerSaveState.luckMultiplier;

            RollStats();

        }

        public void RollStats()
        {
            #region Skill Multipliers
            //STRENGTH multiplier = (Strength * 0.3f)
            strengthMultiplier = (int)(Strength * 0.5f);
            mWeapons = mWeapons + strengthMultiplier;
            carryWeight = carryWeight * (int)strengthMultiplier;

            //Dexterity 
            dexterityMultiplier = (int)(Dexterity * 0.5f);
            mWeapons = mWeapons + dexterityMultiplier;
            fWeapons = fWeapons + dexterityMultiplier;
            lockpick = lockpick + dexterityMultiplier;

            //Constitution
            constituionMultiplier = (int)(Constitution * 0.5f);

            //Endurance
            enduranceMultiplier = (int)(Endurance * 0.5f);
            regenSpeed = regenSpeed - ((float)enduranceMultiplier / 0.8f);

            //AGILITY: multiplier = (Agility * 0.5f)
            agilityMultiplier = (int)(Agility * 0.5f);
            //Set the firearms skill to base + multiplier
            fWeapons = fWeapons + agilityMultiplier;
            //Set movement speed to base + multiplier
            movementSpeed = movementSpeed + ((float)agilityMultiplier / 4);

            //Charisma
            charismaMultiplier = (int)(Charisma * 0.5f);
            speech = speech + charismaMultiplier;

            //Luck
            luckMultiplier = (int)(Luck * 0.2f);
            fWeapons += luckMultiplier;
            mWeapons += luckMultiplier;
            lockpick += luckMultiplier;
            speech += luckMultiplier;
            xpMultiplier += luckMultiplier;
            weaponDamageMultiplier = luckMultiplier;
            critChance = 2 + luckMultiplier;
            Mathf.Clamp((float)critChance, 0, 100);

            #endregion

            #region Health, Stamina, and Damage Resistances
            int newCalculatedHealth = 10 * Constitution + ((int)enduranceMultiplier + (int)constituionMultiplier);
            maxHealth = newCalculatedHealth;
            health = maxHealth;

            //Stamina (endurance * 5 = stamina)
            int newCalculatedStamina = 5 * Endurance + ((int)enduranceMultiplier);
            maxStamina = newCalculatedStamina;
            stamina = newCalculatedStamina;

            //Damage resistance
            damageResistance = (strengthMultiplier + constituionMultiplier + enduranceMultiplier) * (curLevel * 0.2f);
            #endregion

            //The XP needed to level up is 700 * (current level / 2)
            xpNeededToLevelUp = (700 * curLevel / 2);
            Mathf.Round(xpNeededToLevelUp);
        }

        public void SaveVariables()
        {
            playerSaveState.PlayerName = PlayerName;
            playerSaveState.Strength = Strength;
            playerSaveState.Dexterity = Dexterity;
            playerSaveState.Constitution = Constitution;
            playerSaveState.Endurance = Endurance;
            playerSaveState.Agility = Agility;
            playerSaveState.Charisma = Charisma;
            playerSaveState.Luck = Luck;


            playerSaveState.mWeapons = mWeapons;
            playerSaveState.fWeapons = fWeapons;
            playerSaveState.speech = speech;
            playerSaveState.lockpick = lockpick;

            playerSaveState.regenSpeed = regenSpeed;
            playerSaveState.movementSpeed = movementSpeed;
            playerSaveState.maxHealth = maxHealth;
            playerSaveState.maxStamina = maxStamina;
            playerSaveState.damageResistance = damageResistance;

            xpMultiplier = playerSaveState.xpMultiplier;
            weaponDamageMultiplier = playerSaveState.weaponDamageMultiplier;
            critChance = playerSaveState.critChance;

            strengthMultiplier = playerSaveState.strengthMultiplier;
            dexterityMultiplier = playerSaveState.dexterityMultiplier;
            constituionMultiplier = playerSaveState.constituionMultiplier;
            enduranceMultiplier = playerSaveState.enduranceMultiplier;
            agilityMultiplier = playerSaveState.agilityMultiplier;
            charismaMultiplier = playerSaveState.charismaMultiplier;
            luckMultiplier = playerSaveState.luckMultiplier;
        }

        public void GainXP(int xpToGain)
        {
            curXP += xpToGain * (int)xpMultiplier;
        }
    }
}
