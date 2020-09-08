using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter.Sample;

public class Finish : MonoBehaviour
{
    public GameObject doneButton;
    public CameraController.Transition wallTransition;
    public MousePainter painter;
    public void OnTriggerEnter(Collider other)
    {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        AICharacter ai = other.GetComponent<AICharacter>();

        if(player)
        {
            StartCoroutine(LateTransate());
            UiManager.manager.Finish();
            player.Dance();
        }
        if (ai)
        {
            ai.SetMovenet(false);
            ai.Dance();
        }
    }
    IEnumerator LateTransate()
    {
        yield return new WaitForSeconds(6f);
        wallTransition.MakeTransition();
        yield return new WaitForSeconds(1f);
        painter.enabled = true;
        doneButton.SetActive(true);
    }
}
