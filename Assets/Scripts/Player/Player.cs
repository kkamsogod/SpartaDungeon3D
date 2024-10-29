using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public ItemEffectHandler effectHandler;
    public Equipment equip;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        effectHandler = GetComponent<ItemEffectHandler>();
        equip = GetComponent<Equipment>();
    }
}