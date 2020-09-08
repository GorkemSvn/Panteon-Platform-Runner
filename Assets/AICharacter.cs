using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artificial_Intelligence;
public class AICharacter : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float[] feed;
    public Star targetStar;
    public bool StarAi;
    public bool Monitor;
    public ParticleSystem poof;
    DNA dna;
    Vector2 decision;
    bool dead = false;
    Star firstStar;
    public void Start()
    {
        firstStar = targetStar;
        if (StarAi)
            speed = targetStar.speed;
        SetMovenet(true);
        StartCoroutine(Thinking());
    }

    public void Die(float timelenght = 5f)
    {
        if (FallCo == null)
            FallCo = StartCoroutine(Fall(timelenght));
       /* else
        {
            StopCoroutine(FallCo);
            FallCo = StartCoroutine(Fall(timelenght, false));
        }*/
        SetMovenet(false);
        dead = true;
        if (!StarAi)
            dna.Fitness = transform.position.z;

    }
    public void Dance()
    {
        SetMovenet(false);
        GetComponentInChildren<Animator>().SetTrigger("Win");
    }
    public void SetMovenet(bool active)
    {
        if (active)
        {
            if (movementCo == null)
                movementCo = StartCoroutine(Movement());
            else
            {
                StopCoroutine(movementCo);
                movementCo = StartCoroutine(Movement());
            }
        }
        else
        {
            GetComponentInChildren<Animator>().SetFloat("MoveSpeed", 0);
            StopCoroutine(movementCo);
        }
    }

    #region Coroutines
    Coroutine FallCo;
    IEnumerator Fall(float timelenght)
    {
        Debug.Log(gameObject.name + " is fallen");
        GetComponentInChildren<RagdollController>().SetRagDollActive(true);
        this.enabled = false;

        yield return new WaitForSeconds(timelenght);

        poof.Play();

        yield return new WaitForSeconds(0.2f);

        GetComponentInChildren<RagdollController>().SetRagDollActive(false); 
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().gameObject.SetActive(true);
        this.enabled = true;
        transform.position = new Vector3(0, 0, -20);
        SetMovenet(true);
        targetStar = firstStar;
        FallCo = null;
    }

    Coroutine movementCo;
    IEnumerator Movement()
    {
        //move in update if jitters
        CameraController.camera.AutoTakePosition = false;
        Animator animator = GetComponentInChildren<Animator>();
        while (gameObject)
        {

            if (decision.sqrMagnitude > 0.1)
            {
                float dp = Time.fixedDeltaTime;

                Vector3 direction = new Vector3(decision.x, 0, decision.y);
                transform.Translate(direction * speed * dp, Space.World);
                animator.SetFloat("MoveSpeed", 1);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), dp * turnSpeed);
            }
            else
                animator.SetFloat("MoveSpeed", 0);

            yield return new WaitForFixedUpdate();
            CameraController.camera.ManualUpdate();
        }
    }

    IEnumerator Thinking()
    {
        yield return new WaitForSeconds(1f+Random.Range(-0.1f,0.1f));

        if(!StarAi)
            dna = NeuroEvolution.trainQ.Dequeue();
        if (Monitor)
            NEATGraph.instance.DrawNodes(dna, 6, 2);

        while (gameObject)
        {
            yield return new WaitForSeconds(0.1f);

            if (StarAi)
                StarControl();
            else
                NeuralControl();

            if (Monitor)
                NEATGraph.instance.UpdateGraph(); ;
        }
    }
    void StarControl()
    {
        if (targetStar != null)
        {
            Vector3 dir = (targetStar.transform.position - transform.position).normalized;
            decision.y = dir.z;
            decision.x = dir.x;
        }
    }
    void NeuralControl()
    {
        float dist = 5f;
        feed = new float[6];
        feed[0] = transform.position.x;

        if (Physics.Raycast(transform.position + transform.up, Vector3.forward, out RaycastHit forwardHit, dist))
            feed[1] = 1f - forwardHit.distance / dist;

        if (Physics.Raycast(transform.position + transform.up, Vector3.forward + Vector3.right / 4, out RaycastHit rightHit1, dist))
            feed[2] = 1f - rightHit1.distance / dist;
        if (Physics.Raycast(transform.position + transform.up, Vector3.forward + Vector3.right / 2, out RaycastHit rightHit2, dist))
            feed[3] = 1f - rightHit2.distance / dist;

        if (Physics.Raycast(transform.position + transform.up, Vector3.forward - Vector3.right / 4, out RaycastHit leftHit1, dist))
            feed[4] = 1f - leftHit1.distance / dist;

        if (Physics.Raycast(transform.position + transform.up, Vector3.forward - Vector3.right / 2, out RaycastHit leftHit2, dist))
            feed[5] = 1f - leftHit2.distance / dist;

        dna.dnaOperator.FeedForward(feed, 2);

        decision.x = dna.dnaOperator.output[0];
        decision.y = dna.dnaOperator.output[1];
    }
    #endregion
    /*
    void OnDestroy()
    {
     //   dna.Fitness = transform.position.z;
        if (dead)
            dna.Fitness -= 10;
    }*/
}
