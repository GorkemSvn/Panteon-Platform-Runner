using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artificial_Intelligence;
using UnityEngine.UI;

public class NEATGraph : MonoBehaviour
{
    public static NEATGraph instance;
    public bool autoUpdate;
    public Image nodeImage, LineImage;
    float width, height;
    [Range(0.1f,10)]
    public float UpdatePeriod;
    DNA.Operator dnop;
    public Text title;
    private void Start()
    {
        instance = this;
        if(autoUpdate)
            StartCoroutine(update());

    }

    List<NodeImage> nodes = new List<NodeImage>();
    List<AxonsImage> axons = new List<AxonsImage>();

    public void DrawNodes(DNA dna,int inputcount,int outputcount)
    {
        CleanAll();

        for (int i = 0; i < dna.nodes.Count; i++)
        {
            nodes.Add(new NodeImage(transform, nodeImage, dna.dnaOperator.Cores[i]));
        }

        PositionNodes(inputcount,outputcount);
        //position Nodes before this;
        for (int i = 0; i < dna.axons.Count; i++)
        {
            int x = dna.axons[i].i;
            int y = dna.axons[i].j;
            axons.Add(new AxonsImage(LineImage, transform, nodes[x].image, nodes[y].image,dna.axons[i]));
        }
    }
    public void WriteInfo(string text)
    {
        title.text = text;
    }
    public void UpdateGraph()
    {
        foreach(NodeImage n in nodes)
        {
            n.UpdateColor();
        }
    }

    void PositionNodes(int inputcount,int outputcount)
    {
        width = transform.GetComponent<RectTransform>().rect.width;
        height = transform.GetComponent<RectTransform>().rect.height;

        Vector2 pivot = new Vector2(-width/2f, -height/2f);
        for (int i = 0; i < inputcount; i++)  //left side
        {
            nodes[i].image.transform.localPosition = pivot + new Vector2(0,i* height/inputcount );
        }

        pivot = new Vector2(width/2f,-height/2f);
        for (int i = inputcount; i < outputcount+inputcount; i++)//right side
        {
            nodes[i].image.transform.localPosition = pivot + new Vector2(0, (i - inputcount)*height /Mathf.Max(1, outputcount-1));
        }

        for (int i = inputcount+outputcount; i < nodes.Count; i++) // middle
        {
            nodes[i].image.transform.localPosition= CircularPosition(i - inputcount - outputcount,  (nodes.Count - inputcount - outputcount), width / 2f, height / 2f);
        }
    }
    static Vector2 CircularPosition(int index,int nodeCount, float width,float height)
    {
        Vector2 p = Vector3.zero;

        float angle = 2f * Mathf.PI / nodeCount;

        p.x = width * Mathf.Sin(angle * index)/2f;
        p.y = height * Mathf.Cos(angle * index)/2f;

        return p;
    }
    void CleanAll()
    {
        nodes.Clear();
        axons.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    IEnumerator update()
    {
        while (gameObject)
        {
            yield return new WaitForSeconds(UpdatePeriod);
            if(dnop!=null)
                UpdateGraph();
        }
    }
    class NodeImage
    {
        DNA.Operator.NodeOperator node;
        public Image image { get; private set; }

        public NodeImage(Transform panel,Image imagePrefab,DNA.Operator.NodeOperator operatorNode)
        {
            image =Instantiate(imagePrefab, panel);
            node = operatorNode;
        }

        public void UpdateColor()
        {
            if (node.Activation > 0)
            {
                byte p = (byte)(node.Activation * 255);

                image.color = new Color32(p, p, p, 255);
            }
            else
            {
                byte p = (byte)(-node.Activation * 255);

                image.color = new Color32(p, 0, 0, 255);
            }
        }
    }
    class AxonsImage
    {
        Image line;

        public AxonsImage(Image linePrefab,Transform panel, Image node1, Image node2,DNA.Axon axon)
        {
            line = Instantiate(linePrefab, panel);
            Connectimage(line, node1.transform.localPosition, node2.transform.localPosition);

            line.color = new Color(axon.Weight, axon.Weight, axon.Weight);
            line.GetComponentInChildren<Text>().text = System.Math.Round(axon.Weight, 2) + ""; 
            line.transform.SetAsFirstSibling();
        }

       public static void Connectimage(Image Line, Vector2 PosA, Vector2 PosB)
        {
            Vector2 position = (PosA + PosB) / 2;
            Line.rectTransform.localPosition = position;

            Vector2 Direction = PosB - PosA;
            Line.rectTransform.sizeDelta = new Vector2(Direction.magnitude, 10);

            Line.rectTransform.localRotation = Quaternion.FromToRotation(Vector3.right, Direction);// Quaternion.Euler(0, 0, angle);
        }
    }
}

