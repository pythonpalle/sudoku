using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class jsontestintern
{
    public List<bool> ints = new List<bool>();

    public int intCount => ints.Count;

    public jsontestintern(List<bool> bools)
    {
        ints = bools;
    }
}

[System.Serializable]
public class jsontest : MonoBehaviour
{
    public List<bool> ints = new List<bool>();

    

    // Start is called before the first frame update
    void Start() 
    {
        for (int i = 0; i < 100; i++)
        {
            ints.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveToBinary();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFromBinary();
        }
    }

    void SaveToBinary()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();

        jsontestintern test = new jsontestintern(ints);
        
        formatter.Serialize(memoryStream, test); 

        // Get the serialized data as a byte array
        byte[] serializedData = memoryStream.ToArray();
        
        FileManager.WriteAllBytes("byteTest", serializedData);
        
        // To deserialize, you would do the reverse
        // Deserialize from byte array to object
        memoryStream = new MemoryStream(serializedData);
    }
    
    private void LoadFromBinary()
    {
        byte[] bytes = FileManager.ReadAllBytes("byteTest");
        
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(bytes);
        
        jsontestintern deserializedObject = (jsontestintern)formatter.Deserialize(memoryStream);

        Debug.Log($"ints counts: {deserializedObject.intCount}");
        

    }
}
