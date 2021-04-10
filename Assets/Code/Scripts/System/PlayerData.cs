using System;
using UnityEngine;

public enum State { NONE, BURNED, ASLEEP, PARALIZED, POISONED, SCARED }

public class PlayerData : MonoBehaviour {

    [SerializeField] public int baseEnergy;
    [NonSerialized]  public int currentEnergy;
    
    [SerializeField] public float baseHealth;
    [NonSerialized]  public float currentHealth;

    [SerializeField] public float baseAttack;
    [NonSerialized]  public float currentAttack;
    
    [SerializeField] public float baseDefense;
    [NonSerialized]  public float currentDefense;
    
    [SerializeField] public float baseSpeed;
    [NonSerialized]  public float currentSpeed;

    [SerializeField] public State healthState;
}