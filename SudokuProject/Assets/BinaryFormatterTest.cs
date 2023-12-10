using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class BoolListContainer
{
    public List<bool> bools = new List<bool>();

    public int boolCount => bools.Count;

    public BoolListContainer(List<bool> bools)
    {
        this.bools = bools;
    }
    
    public void SaveToBinary()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();

        BoolListContainer test = new BoolListContainer(bools);
        
        formatter.Serialize(memoryStream, test); 

        // Get the serialized data as a byte array
        byte[] serializedData = memoryStream.ToArray();
        
        FileManager.WriteAllBytes("byteTest", serializedData);
        
        // To deserialize, you would do the reverse
        // Deserialize from byte array to object
        memoryStream = new MemoryStream(serializedData);
    }
    
    public void LoadFromBinary()
    {
        byte[] bytes = FileManager.ReadAllBytes("byteTest");
        
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(bytes);
        
        // todo: fix this
        //this = (BoolListContainer)formatter.Deserialize(memoryStream);
        BoolListContainer deserializedObject = (BoolListContainer)formatter.Deserialize(memoryStream);

        Debug.Log($"ints counts: {deserializedObject.boolCount}");
        

    }
}

[System.Serializable]
public class BinaryFormatterTest : MonoBehaviour
{
    private List<bool> bools = new List<bool>();
    // Start is called before the first frame update

    private BoolListContainer boolList;
    void Start() 
    {
        for (int i = 0; i < 100; i++)
        {
            bools.Add(false);
        }

        boolList = new BoolListContainer(bools);
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
        boolList.SaveToBinary();
    }
    
    private void LoadFromBinary()
    {
        boolList.LoadFromBinary();
    }
}
