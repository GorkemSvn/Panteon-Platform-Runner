using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Ranker : MonoBehaviour
{
    public List<Transform> competitors;
    public Transform player;

    public static int playerRank;

    Coroutine rankerCo;
    public void Start()
    {
        rankerCo= StartCoroutine(Rank());
    }

    public void StopRanking()
    {
        StopCoroutine(rankerCo);
    }
    IEnumerator Rank()
    {
        while (gameObject)
        {
            yield return new WaitForSeconds(0.5f);
            competitors.Sort((x, y) => x.position.z.CompareTo(y.position.z));
            competitors.Reverse();
            playerRank = competitors.IndexOf(player);
            GetComponent<Text>().text = "Rank :" + (playerRank + 1);
        }
    }


}
