using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();
        if (player)
            if (player.enabled)
                player.Die();

        AICharacter ai = collision.collider.GetComponent<AICharacter>();
        if (ai)
            if (ai.enabled)
                ai.Die();
    }
}
