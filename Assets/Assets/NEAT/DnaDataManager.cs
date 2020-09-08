using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Artificial_Intelligence;

public class DnaDataManager :MonoBehaviour
{
    public string dataFileName;
    public int populationSize;
    public float lapLenght;
    public bool train;

    static DNAData data;

    public void Awake()
    {
        if (NeuroEvolution.members.Count == 0)
        {
            if (DnaDataManager.LoadToNeuroEvolution(dataFileName))
                Debug.Log("Load succesfull");
            else
            {
                NeuroEvolution.StartNewPopulation(populationSize, 8, 4, 6);
                Debug.Log("Starting New Popy");
            }

        }
        else if (NeuroEvolution.trainQ.Count == 0)
        {
            NeuroEvolution.Evolve();
            Debug.Log("Evolving");
            DnaDataManager.Save(dataFileName);
        }

        List<string> s = NeuroEvolution.GetStats(3);
        for (int i = 0; i < s.Count; i++)
        {
            Debug.Log(s[i]);
        }
        if (train)
            StartCoroutine(restart());
    }

    IEnumerator restart()
    {
        yield return new WaitForSeconds(lapLenght);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public static void Save(string fileName)
    {
        data = new DNAData();
        data.SaveDNA(NeuroEvolution.members, Application.dataPath + "/Resources/" + fileName + ".bytes");
    }
    public static bool LoadToNeuroEvolution(string resourcesFileName)
    {
        data = DnaDataManager.LoadDna(resourcesFileName);

        if (data != null)
        {
            data.SaveDNA(data.DeserializeDNA(), Application.persistentDataPath + "/temp");

            NeuroEvolution.LoadMembers(Application.persistentDataPath + "/temp");
            for (int i = 0; i < NeuroEvolution.members.Count; i++)
            {
                NeuroEvolution.trainQ.Enqueue(NeuroEvolution.members[i]);
            }
            return true;
        }
        else
            return false;
    }
    public static DNAData LoadDna(string DnafileName)
    {
        TextAsset textAsset = Resources.Load(DnafileName) as TextAsset;
        if (textAsset != null)
        {
            Stream stream = new MemoryStream(textAsset.bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            data = formatter.Deserialize(stream) as DNAData;

            return data;
        }

        return null;
    }
}
