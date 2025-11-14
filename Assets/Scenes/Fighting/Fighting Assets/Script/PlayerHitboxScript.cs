using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxScript : MonoBehaviour
{
    private Mentor mentor;
    public Player player;
    private bool mentorIsAttacking = false;
    FightingAudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }

    void Update()
    {
        if (mentor != null)
        {
            mentorIsAttacking = mentor.isAttacking;
        }
    }

    public void AssignMentor(Mentor activeMentor)
    {
        mentor = activeMentor;
        //Debug.Log("Mentor assigned: " + mentor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Fists") && mentorIsAttacking) //Teddy Bear
        {
            audioManager.playSFX(audioManager.teddy);
            player.takeDamage();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Sword") && mentorIsAttacking) 
        {
            audioManager.playSFX(audioManager.sword);
            player.takeDamage();
        }

        //Rescripted Melee Weapons
        if (other.gameObject.layer == LayerMask.NameToLayer("Lollipop") && mentorIsAttacking)
        {
            audioManager.playSFX(audioManager.lollipop);
            player.takeDamage();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Banana") && mentorIsAttacking)
        {
            audioManager.playSFX(audioManager.banana);
            player.takeDamage();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("RubberChicken") && mentorIsAttacking)
        {
            audioManager.playSFX(audioManager.rubberChicken);
            player.takeDamage();
        }
    }
}
