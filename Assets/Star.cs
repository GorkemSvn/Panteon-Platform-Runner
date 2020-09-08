using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Star : MonoBehaviour
{
    public static int point;
    public Star nextStar;
    public float speed = 5f;
    public Transform meshT;
    bool counted = false;
    public void FixedUpdate()
    {
        transform.Rotate(0, 180 * Time.fixedDeltaTime, 0);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharacter>()&&!counted)
        {
            StartCoroutine(Efx(0.5f));
            counted = true;
        }
        else if (other.GetComponent<AICharacter>())
        {
            if (nextStar != null)
            {
                other.GetComponent<AICharacter>().targetStar = nextStar;
                other.GetComponent<AICharacter>().speed = nextStar.speed;
            }
            else
                other.GetComponent<AICharacter>().speed = 0;
        }
    }

    IEnumerator Efx(float lenght)
    {
        Vector3 startscale = meshT.localScale;
        GetComponentInChildren<ParticleSystem>().Play();
        for (float i = 0; i <= lenght; i+=Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
            meshT.localScale = Vector3.Lerp(startscale, Vector3.zero, i/ lenght);
        }
        GameObject.Find("StarText").GetComponentInChildren<Text>().text = ++point + "";
        //Destroy(gameObject);
    }
}
