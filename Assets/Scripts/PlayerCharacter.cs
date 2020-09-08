using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float speed;
    public float turnSpeed;



    public void Die(float timelenght=1f,bool temporary=false)
    {
        if (FallCo == null)
            FallCo = StartCoroutine(Fall(timelenght,!temporary));
        else
        {
            StopCoroutine(FallCo);
            FallCo = StartCoroutine(Fall(timelenght, !temporary));
        }

        if (!temporary)
            UiManager.manager.LevelFail();
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
            StopCoroutine(movementCo);
            GetComponentInChildren<Animator>().SetFloat("MoveSpeed", 0);
        }
    }

    #region Coroutines
    Coroutine FallCo;
    IEnumerator Fall(float timelenght, bool permanent=false)
    {
        GetComponentInChildren<RagdollController>().SetRagDollActive(true);
        this.enabled = false;
        yield return new WaitForSeconds(timelenght);
        GetComponentInChildren<RagdollController>().SetRagDollActive(permanent);
        this.enabled = permanent;
    }

    Coroutine movementCo;
    IEnumerator Movement()
    {
        //move in update if jitters
        CameraController.camera.AutoTakePosition = false;
        Animator animator = GetComponentInChildren<Animator>();
        while (gameObject)
        {

            if (InputManager.sqrdInputPower > 0.1 && InputManager.swiping)
            {
                float dp = Time.fixedDeltaTime;

                Vector3 direction = new Vector3(InputManager.normalizedSwipe.x, 0, InputManager.normalizedSwipe.y);
                transform.Translate(direction * speed * dp, Space.World);
                animator.SetFloat("MoveSpeed", InputManager.sqrdInputPower);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), dp * turnSpeed);
            }
            else
                animator.SetFloat("MoveSpeed", 0);

            yield return new WaitForFixedUpdate();
            CameraController.camera.ManualUpdate();
        }
    }
    #endregion
}
