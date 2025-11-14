
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MentorHitboxScript : MonoBehaviour
{
    public Mentor mentor;
    public Player player;

    FightingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Punch
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack") && player.isAttacking)
        {
            mentor.TakeDamage();
        }

        // Water
        if (other.gameObject.layer == LayerMask.NameToLayer("WaterAttack"))
        {
            mentor.TakeDamage();
        }

        // Melee Weapons - Only apply damage if player is holding the weapon
        if (IsHeldByPlayer())
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Lollipop"))
            {
                audioManager.playSFX(audioManager.lollipop);
                mentor.TakeDamage();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Banana"))
            {
                audioManager.playSFX(audioManager.banana);
                mentor.TakeDamage();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("RubberChicken"))
            {
                audioManager.playSFX(audioManager.rubberChicken);
                mentor.TakeDamage();
            }
        }
    }

    private bool IsHeldByPlayer()
    {
        if (player == null) return false;

        bool rightHolding = !player.IsHandEmpty(player.rightController);
        bool leftHolding = !player.IsHandEmpty(player.leftController);

        return rightHolding || leftHolding;
    }
}

