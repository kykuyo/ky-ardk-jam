using System;
using UnityEngine;

public class GuidService : MonoBehaviour
{
    private const string GUID = "GUID";
    private string _guid;
    public string UserId => _guid;

    public static GuidService Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _guid = GetOrCreateGuid();
        Debug.Log("Unique ID: " + _guid);
    }

    private string GetOrCreateGuid()
    {
        if (PlayerPrefs.HasKey(GUID))
        {
            return PlayerPrefs.GetString(GUID);
        }
        else
        {
            string newID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(GUID, newID);
            PlayerPrefs.Save();
            return newID;
        }
    }
}
