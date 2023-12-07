
[System.Serializable]
public struct UserIdentifier
{
    public UserIdentifier(string name, string id)
    {
        this.name = name;
        this.id = id;
    }
        
    public string name;
    public string id;
}