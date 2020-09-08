using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageText : MonoBehaviour
{
    static Queue<string> messageQue = new Queue<string>();
    static Queue<float> timeQue = new Queue<float>();

    public void ShowMessage(string message, float timelenght)
    {
        messageQue.Enqueue(message);
        timeQue.Enqueue(timelenght);

        if (showCo == null)
            showCo = StartCoroutine(ShowMessages());
    }

    static Coroutine showCo;
    IEnumerator ShowMessages()
    {
        while (messageQue.Count > 0)
        {
            GetComponent<Text>().text = messageQue.Dequeue();
            yield return new WaitForSeconds(timeQue.Dequeue());
        }
        GetComponent<Text>().text ="";
        showCo = null;
    }
    public void OnDestroy()
    {
        showCo = null;
    }
}
