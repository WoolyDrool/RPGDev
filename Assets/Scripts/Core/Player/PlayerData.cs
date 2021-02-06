using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Player/New Player Data")]
public class PlayerData : ScriptableObject
{

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

    
}
