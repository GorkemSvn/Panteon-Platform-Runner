using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Transform cuttingPlane;

    public void OnTriggerEnter(Collider other)
    {

        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player)
            if (player.enabled)
                player.Die();

        AICharacter ai = other.GetComponent<AICharacter>();
        if (ai)
            if (ai.enabled)
                ai.Die();
        if (other.transform.tag == "Slicable" && !other.isTrigger)
        {
            //planeDummy.eulerAngles = Vector3.zero;
            SliceObject(other.GetComponentInChildren<Renderer>().gameObject, cuttingPlane);
          /*  if ( )
            {
                if (other.transform.parent != null)
                    other.transform.parent.gameObject.SetActive(false);
            }*/
        }
    }



    public bool SliceObject(GameObject objWithMesh, Transform plane)
    {
        Mesh mesh = GetMesh(objWithMesh, out Material[] mats);

        if (mesh == null)
            return false;

        GorkemMeshManipulator.DigestMesh(mesh, objWithMesh.transform);
        Mesh[] meshes = GorkemMeshManipulator.SplitWithPlane(plane);

        if (meshes == null)
            return false;

        TurnMeshToObject(meshes[0], mats, objWithMesh.transform, plane.forward + plane.up);
        TurnMeshToObject(meshes[1], mats, objWithMesh.transform, plane.forward + plane.right);
        objWithMesh.SetActive(false);
        return true;
    }


    static Mesh GetMesh(GameObject objWithMeshComp, out Material[] mats)
    {
        if (objWithMeshComp.GetComponent<MeshFilter>())
        {
            mats = objWithMeshComp.GetComponent<MeshRenderer>().materials;
            return objWithMeshComp.GetComponent<MeshFilter>().mesh;
        }
        else if (objWithMeshComp.GetComponent<SkinnedMeshRenderer>())
        {
            SkinnedMeshRenderer smr = objWithMeshComp.GetComponent<SkinnedMeshRenderer>();
            mats = objWithMeshComp.GetComponent<SkinnedMeshRenderer>().materials;
            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);
            return mesh;
        }
        mats = null;
        return null;
    }
    void TurnMeshToObject(Mesh mesh, Material[] mats, Transform target, Vector3 velocity)
    {
        GameObject A = new GameObject();
        A.transform.position = target.position;
        A.transform.rotation = target.rotation;
        A.transform.localScale = target.localScale;
        A.AddComponent<MeshFilter>().mesh = mesh;
        A.AddComponent<MeshRenderer>().materials = mats;
        A.AddComponent<Rigidbody>().velocity = velocity;
        A.AddComponent<MeshCollider>().convex = true;

        StartCoroutine(destroy(A));
    }
    IEnumerator destroy(GameObject a)
    {
        yield return new WaitForSeconds(5f);
        Destroy(a);
    }
}
