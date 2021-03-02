using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

public class EventService : MonoBehaviour
{

    float cooldownBeforeSend = 10.0f;


    string serverURL = "file:///E:/events";


    List<Event> events = new List<Event>();
    public void Start()
    {
        InvokeRepeating("SendEvents", 0, cooldownBeforeSend);
    }
    // Add event to list
    public void TrackEvent(string type, string data)
    {
        events.Add(new Event(type, data));
        SendEvents();
    }
    // Send To server
    void SendEvents()
    {
        //Добовляет блок events в конвертированный запрос, иначе он выглядел бы не как в таске
        Events ev = new Events();
        ev.events = events;
        //
        //convert to json
        DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Events));

        using (FileStream fs = new FileStream("1.json", FileMode.OpenOrCreate))
        {
            formatter.WriteObject(fs, ev);
        }
        string json = File.ReadAllText("1.json");

        Debug.Log(json);

        //Запрос - ответ
        WebRequest request = WebRequest.Create(serverURL);
        request.Method = "POST";
        byte[] byte1 = System.Text.Encoding.UTF8.GetBytes(json);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byte1.Length;

        using (Stream dataStream = request.GetRequestStream())
        {
            dataStream.Write(byte1, 0, byte1.Length);
        }

        WebResponse response = request.GetResponse();
        using (Stream stream = response.GetResponseStream())
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string str = reader.ReadToEnd();
                if (str.Contains("200 OK"))
                {
                    events.Clear();
                    File.Delete("1.json");
                }
            }
        }
        response.Close();
    }

}
[Serializable]
public class Events
{
    public List<Event> events = new List<Event>();
}
[Serializable]
public class Event
{

    public string type;
    public string data;
    public Event(string type, string data)
    {
        this.data = data;
        this.type = type;
    }
}

public class GameInfo : MonoBehaviour
{
    private int coinsCount = 0;
    private int currentLevel = 1;
    private int beautyPoints = 0;

    public delegate void EventHandler(string type, string data);
    public event EventHandler Notify;
    public GameInfo(int coins, int level, int points)
    {
        coinsCount = coins;
        currentLevel = level;
        beautyPoints = points;
        Notify += CreateEvent;
    }
    //Send events 
    void CreateEvent(string type, string data)
    {
        GameObject.Find("Services").GetComponent<EventService>().TrackEvent(type, data);
        Debug.Log("Send!");
    }
    //Exemple methods
    public void IncreaseCoinsCount(int coins)
    {
        coinsCount += coins;
        Notify?.Invoke("IncreaseCoins", $"{coins}");
    }

    public void DecreaseCoinsCount(int coins)
    {
        coinsCount -= coins;
        Notify?.Invoke("DecreaseCoins", $"{coins}");
    }
    public void NewLevelStart()
    {
        currentLevel += 1;
        Notify?.Invoke("LevelUp", $"{currentLevel}");
    }
    public void GetRewards()
    {
        beautyPoints += 1;
        Notify?.Invoke("TaskComplete", $"{beautyPoints}");
    }
    //
}
