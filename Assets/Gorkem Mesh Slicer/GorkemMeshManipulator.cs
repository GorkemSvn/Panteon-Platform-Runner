using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorkemMeshManipulator : MonoBehaviour
{
    static List<Vector3> vertices;
    static List<Vector3> normals;
    static List<Vector2> uvs;
    static List<int> triangles;
    static List<MeshTriangle> meshTriangles;

    static Transform meshTransform;

    public static void DigestMesh(Mesh mesh,Transform MeshTransform)
    {
        meshTransform = MeshTransform;

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        meshTriangles = new List<MeshTriangle>();

        vertices.AddRange(mesh.vertices);
        normals.AddRange(mesh.normals);
        uvs.AddRange(mesh.uv);
        triangles.AddRange(mesh.triangles);

        for (int i = 0; i < triangles.Count; i += 3)
        {
            meshTriangles.Add(new MeshTriangle(
                vertices[triangles[i]],
                vertices[triangles[i+1]],
                vertices[triangles[i+2]],
                triangles[i],
                triangles[i+1],
                triangles[i+2]
                ));
        }
    }
    public static Mesh[] SplitWithPlane(Transform planeTransform)
    {
        Plane plane = new Plane(meshTransform.InverseTransformDirection( planeTransform.up), meshTransform.InverseTransformPoint(planeTransform.position));

        List<MeshTriangle> rightTris = new List<MeshTriangle>();
        List<MeshTriangle> leftTris = new List<MeshTriangle>();

        List<Vector3> rightVertices = new List<Vector3>();
        List<Vector3> leftVertices = new List<Vector3>();

        List<Vector2> rightUvs = new List<Vector2>();
        List<Vector2> leftUvs = new List<Vector2>();

        Vector3 middle = Vector3.zero;
        rightVertices.Add(middle);
        leftVertices.Add(middle);
        rightUvs.Add(Vector2.zero);
        leftUvs.Add(Vector2.zero);
        List<MeshTriangle> AddrightTris = new List<MeshTriangle>();
        List<MeshTriangle> AddleftTris = new List<MeshTriangle>();


        bool valid = false;

        for (int i = 0; i < meshTriangles.Count; i++)
        {
            MeshTriangle tri = meshTriangles[i];

            bool aIsRight = plane.GetSide(tri.verticeA);
            bool bIsRight = plane.GetSide(tri.verticeB);
            bool cIsRight = plane.GetSide(tri.verticeC);

            if (aIsRight && bIsRight && cIsRight)
            {
                int c = rightVertices.Count;
                rightVertices.AddRange(new Vector3[] { tri.verticeA, tri.verticeB, tri.verticeC });
                rightUvs.AddRange(new Vector2[] { uvs[tri.a], uvs[tri.b], uvs[tri.c] });
                rightTris.Add(new MeshTriangle(tri.verticeA,tri.verticeB,tri.verticeC,    c,c+1,c+2));
            }
            else if (!aIsRight && !bIsRight && !cIsRight)
            {
                int c = leftVertices.Count;
                leftVertices.AddRange(new Vector3[] { tri.verticeA, tri.verticeB, tri.verticeC });
                leftUvs.AddRange(new Vector2[] { uvs[tri.a], uvs[tri.b], uvs[tri.c] });

                leftTris.Add(new MeshTriangle(tri.verticeA, tri.verticeB, tri.verticeC, c, c + 1, c + 2));
            }
            else
            {
                if (aIsRight && bIsRight)
                {
                    valid = true;
                    Ray ray = new Ray(tri.verticeA, tri.verticeC - tri.verticeA);
                    plane.Raycast(ray, out float en);
                    Vector3 ac = tri.verticeA + (tri.verticeC - tri.verticeA).normalized * en;

                    ray = new Ray(tri.verticeB, tri.verticeC - tri.verticeB);
                    plane.Raycast(ray, out en);
                    Vector3 bc = tri.verticeB + (tri.verticeC - tri.verticeB).normalized * en;

                    int rc = rightVertices.Count;

                    rightVertices.Add(tri.verticeA); //rc
                    rightVertices.Add(tri.verticeB); //rc+1
                    rightUvs.Add(uvs[tri.b]);
                    rightUvs.Add(uvs[tri.a]);

                    rightVertices.Add(ac); //rc+2
                    rightVertices.Add(bc); //rc+3
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);
                    
                    //middle
                    AddrightTris.Add(new MeshTriangle(middle, ac, bc, 0, rc + 2, rc + 3));
                    
                    MeshTriangle acTri = new MeshTriangle(tri.verticeA, tri.verticeB, bc,     rc, rc+1, rc + 3);
                    MeshTriangle bcTri = new MeshTriangle(tri.verticeA, bc, ac,    rc, rc + 3, rc+2);
                    rightTris.Add(acTri);
                    rightTris.Add(bcTri);

                    int lc = leftVertices.Count;
                    leftVertices.Add(tri.verticeC);//c
                    leftVertices.Add(ac);//c+1
                    leftVertices.Add(bc);//c+2
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);

                    AddleftTris.Add(new MeshTriangle(middle, bc, ac, 0, lc + 2, lc + 1));
                    MeshTriangle ltri = new MeshTriangle(ac,  bc, tri.verticeC, lc +1,lc+2, lc);
                    leftTris.Add(ltri);

                }
                if (aIsRight && cIsRight)
                {
                    valid = true;
                    Ray ray = new Ray(tri.verticeA, tri.verticeB - tri.verticeA);
                    plane.Raycast(ray, out float en);
                    Vector3 ab = tri.verticeA + (tri.verticeB - tri.verticeA).normalized * en;

                    ray = new Ray(tri.verticeC, tri.verticeB - tri.verticeC);
                    plane.Raycast(ray, out en);
                    Vector3 cb = tri.verticeC + (tri.verticeB - tri.verticeC).normalized * en;

                    int rc = rightVertices.Count;

                    rightVertices.Add(tri.verticeA); //rc
                    rightUvs.Add(Vector2.zero);
                    rightVertices.Add(tri.verticeC); //rc+1
                    rightUvs.Add(Vector2.zero);
                    rightVertices.Add(ab); //rc+2
                    rightUvs.Add(Vector2.zero);
                    rightVertices.Add(cb); //rc+3
                    rightUvs.Add(Vector2.zero);

                    //middle
                    AddrightTris.Add(new MeshTriangle(middle, cb, ab, 0, rc + 3, rc + 2));
                    MeshTriangle acTri = new MeshTriangle(tri.verticeA, ab, cb, rc, rc + 2, rc + 3);
                    MeshTriangle bcTri = new MeshTriangle(tri.verticeA, cb, tri.verticeC, rc, rc + 3, rc + 1);
                    rightTris.Add(acTri);
                    rightTris.Add(bcTri);

                    int lc = leftVertices.Count;
                    leftVertices.Add(tri.verticeB);//c
                    leftVertices.Add(ab);//c+1
                    leftVertices.Add(cb);//c+2
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);

                    AddleftTris.Add(new MeshTriangle(middle, ab, cb, 0, lc + 1, lc + 2));
                    MeshTriangle ltri = new MeshTriangle(ab, tri.verticeB,cb, lc + 1, lc, lc + 2);
                    leftTris.Add(ltri);

                }
                if (bIsRight && cIsRight)
                {
                    valid = true;
                    Ray ray = new Ray(tri.verticeB, tri.verticeA - tri.verticeB);
                    plane.Raycast(ray, out float en);
                    Vector3 ba = tri.verticeB + (tri.verticeA- tri.verticeB).normalized * en;

                    ray = new Ray(tri.verticeC, tri.verticeA - tri.verticeC);
                    plane.Raycast(ray, out en);
                    Vector3 ca = tri.verticeC + (tri.verticeA - tri.verticeC).normalized * en;

                    int rc = rightVertices.Count;

                    rightVertices.Add(tri.verticeB); //rc
                    rightUvs.Add(Vector2.zero);
                    rightVertices.Add(tri.verticeC); //rc+1
                    rightUvs.Add(Vector2.zero);

                    rightVertices.Add(ba); //rc+2
                    rightUvs.Add(Vector2.zero);
                    rightVertices.Add(ca); //rc+3
                    rightUvs.Add(Vector2.zero);

                    //middle
                    AddrightTris.Add(new MeshTriangle(middle, ba, ca, 0, rc + 2, rc + 3));
                    MeshTriangle acTri = new MeshTriangle(tri.verticeB, ca, ba, rc, rc + 3, rc + 2);
                    MeshTriangle bcTri = new MeshTriangle(tri.verticeB,  tri.verticeC, ca, rc, rc + 1, rc + 3);
                    rightTris.Add(acTri);
                    rightTris.Add(bcTri);

                    int lc = leftVertices.Count;
                    leftVertices.Add(tri.verticeA);//c
                    leftVertices.Add(ba);//c+1
                    leftVertices.Add(ca);//c+2
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);

                    AddleftTris.Add(new MeshTriangle(middle, ca, ba, 0, lc + 2, lc + 1));
                    MeshTriangle ltri = new MeshTriangle(ca, tri.verticeA, ba, lc + 2, lc, lc + 1);
                    leftTris.Add(ltri);

                }
                if(aIsRight && !bIsRight && !cIsRight)
                {
                    valid = true;
                    Ray ray = new Ray(tri.verticeB, tri.verticeA - tri.verticeB);
                    plane.Raycast(ray, out float en);
                    Vector3 ba = tri.verticeB + (tri.verticeA - tri.verticeB).normalized * en;

                    ray = new Ray(tri.verticeC, tri.verticeA - tri.verticeC);
                    plane.Raycast(ray, out en);
                    Vector3 ca = tri.verticeC + (tri.verticeA - tri.verticeC).normalized * en;

                    int rc = leftVertices.Count;

                    leftVertices.Add(tri.verticeB); //rc
                    leftVertices.Add(tri.verticeC); //rc+1
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);

                    leftVertices.Add(ba); //rc+2
                    leftVertices.Add(ca); //rc+3
                    leftUvs.Add(Vector2.zero);
                    leftUvs.Add(Vector2.zero);

                    AddleftTris.Add(new MeshTriangle(middle, ba, ca, 0, rc + 2, rc + 3));
                    MeshTriangle acTri = new MeshTriangle(
                        tri.verticeB, ca, ba, 
                        rc, rc + 3, rc + 2);
                    MeshTriangle bcTri = new MeshTriangle(
                        tri.verticeB, tri.verticeC,ca,
                        rc, rc + 1, rc + 3);
                    leftTris.Add(acTri);
                    leftTris.Add(bcTri);

                    int lc = rightVertices.Count;
                    rightVertices.Add(tri.verticeA);//c
                    rightVertices.Add(ba);//c+1
                    rightVertices.Add(ca);//c+2
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);

                    AddrightTris.Add(new MeshTriangle(middle, ca, ba, 0, lc + 2, lc + 1));
                    MeshTriangle ltri = new MeshTriangle(ba, ca, tri.verticeA, lc + 1, lc+2, lc );
                    rightTris.Add(ltri);
                }
                if(bIsRight && !cIsRight && !aIsRight)
                {
                    valid = true;
                    Ray ray = new Ray(tri.verticeA, tri.verticeB - tri.verticeA);
                    plane.Raycast(ray, out float en);
                    Vector3 ab = tri.verticeA + (tri.verticeB - tri.verticeA).normalized * en;

                    ray = new Ray(tri.verticeC, tri.verticeB - tri.verticeC);
                    plane.Raycast(ray, out en);
                    Vector3 cb = tri.verticeC + (tri.verticeB - tri.verticeC).normalized * en;

                    int rc = leftVertices.Count;

                    leftVertices.Add(tri.verticeA); //rc
                    leftUvs.Add(Vector2.zero);
                    leftVertices.Add(tri.verticeC); //rc+1
                    leftUvs.Add(Vector2.zero);
                    leftVertices.Add(ab); //rc+2
                    leftUvs.Add(Vector2.zero);
                    leftVertices.Add(cb); //rc+3
                    leftUvs.Add(Vector2.zero);

                    AddleftTris.Add(new MeshTriangle(middle, cb, ab, 0, rc + 3, rc + 2));
                    MeshTriangle acTri = new MeshTriangle(tri.verticeA, ab, cb, rc, rc + 2, rc + 3);
                    MeshTriangle bcTri = new MeshTriangle(tri.verticeA, cb, tri.verticeC, rc, rc + 3, rc + 1);
                    leftTris.Add(acTri);
                    leftTris.Add(bcTri);

                    int lc = rightVertices.Count;
                    rightVertices.Add(tri.verticeB);//c
                    rightVertices.Add(ab);//c+1
                    rightVertices.Add(cb);//c+2
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);

                    AddrightTris.Add(new MeshTriangle(middle, ab, cb, 0, lc + 1, lc + 2));
                    MeshTriangle ltri = new MeshTriangle(ab,  tri.verticeB, cb, lc + 1, lc, lc+2 );
                    rightTris.Add(ltri);
                }
                if(cIsRight && !aIsRight && !bIsRight)
                {
                    valid = true;
                    Ray ray = new Ray(tri.verticeA, tri.verticeC - tri.verticeA);
                    plane.Raycast(ray, out float en);
                    Vector3 ac = tri.verticeA + (tri.verticeC - tri.verticeA).normalized * en;

                    ray = new Ray(tri.verticeB, tri.verticeC - tri.verticeB);
                    plane.Raycast(ray, out en);
                    Vector3 bc = tri.verticeB + (tri.verticeC - tri.verticeB).normalized * en;

                    int rc = leftVertices.Count;

                    leftVertices.Add(tri.verticeA); //rc
                    leftUvs.Add(uvs[tri.a]);
                    leftVertices.Add(tri.verticeB); //rc+1
                    leftUvs.Add(uvs[tri.b]);

                    leftVertices.Add(ac); //rc+2
                    leftUvs.Add(Vector2.zero);
                    leftVertices.Add(bc); //rc+3
                    leftUvs.Add(Vector2.zero);

                    AddleftTris.Add(new MeshTriangle(middle, ac, bc, 0, rc + 2, rc + 3));
                    MeshTriangle acTri = new MeshTriangle(
                        tri.verticeA, bc, ac,
                        rc, rc + 3, rc + 2);
                    MeshTriangle bcTri = new MeshTriangle(
                        tri.verticeA,  tri.verticeB, bc,
                        rc, rc + 1, rc + 3);
                    leftTris.Add(acTri);
                    leftTris.Add(bcTri);

                    int lc = rightVertices.Count;
                    rightVertices.Add(tri.verticeC);//c
                    rightVertices.Add(ac);//c+1
                    rightVertices.Add(bc);//c+2
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);
                    rightUvs.Add(Vector2.zero);

                    AddrightTris.Add(new MeshTriangle(middle,  bc,ac, 0, lc + 2, lc +1));
                    MeshTriangle ltri = new MeshTriangle(ac,  bc, tri.verticeC, lc + 1, lc+2, lc );
                    rightTris.Add(ltri);
                }
            }
        }

        for (int i = 0; i < AddrightTris.Count; i++)
        {
            middle += (AddrightTris[i].verticeB + AddrightTris[i].verticeC);
        }
        middle /= AddrightTris.Count*2;
        rightVertices[0] = middle;
        leftVertices[0] = middle;
        rightTris.AddRange(AddrightTris);
        leftTris.AddRange(AddleftTris);

        if (!valid)
        {
            Debug.Log("not valid");
            return null;
        }

        return new Mesh[] { makeMesh(rightVertices,rightUvs,rightTris), makeMesh(leftVertices,leftUvs, leftTris) };
    }
    static Mesh makeMesh(List<Vector3> _vertices, List<Vector2> _uvs, List<MeshTriangle> triangles)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = _vertices.ToArray();
        mesh.uv = _uvs.ToArray();
        mesh.normals =new Vector3[_vertices.Count];

        List<int> tris = new List<int>();

        for (int i = 0; i < triangles.Count; i++)
        {
            tris.Add(triangles[i].a);
            tris.Add(triangles[i].b);
            tris.Add(triangles[i].c);
        }
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
    public struct MeshTriangle
    {
        public Vector3 verticeA { get; private set; }
        public Vector3 verticeB { get; private set; }
        public Vector3 verticeC { get; private set; }

        public int a { get; private set; }
        public int b { get; private set; }
        public int c { get; private set; }

        public MeshTriangle(Vector3 va, Vector3 vb, Vector3 vc, int ai, int bi, int ci)
        {
            a = ai;
            b = bi;
            c = ci;

            verticeA = va;
            verticeB = vb;
            verticeC = vc;
        }
    }
}
