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

    private void Start()
    {
        InvokeRepeating("TrackEvent", 0, cooldownBeforeSend);
    }
    public void AddEvent(Event newEvent)
    {
        events.Add(newEvent);
    }
    public void ShowEvents()
    {
        foreach (Event ev in events)
        {
            Debug.Log(ev.type + ev.data);
        }
    }

    public void TrackEvent()
    {
        Events ev = new Events();
        ev.events = events;

        DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Events));
        using (FileStream fs = new FileStream("1.json", FileMode.OpenOrCreate))
        {
            formatter.WriteObject(fs, ev);
        }
        string json = File.ReadAllText("1.json");

        Debug.Log(json);

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
                string str = "200 OK";
                //Debug.Log(reader.ReadToEnd());
                //Если запрос успешно прошёл то очищаем лист ивентов
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

public class GameInfo
{
    private int coinsCount = 0;
    private int currentLevel = 1;
    private int beautyPoints = 0;

    public delegate void EventHandler(string type, string data);
    public event EventHandler Notify;

    public EventService events = new EventService();


    public GameInfo(int coins, int level, int points)
    {
        coinsCount = coins;
        currentLevel = level;
        beautyPoints = points;
        Notify += CreateEvent;
    }
    void CreateEvent(string type, string data)
    {
        events.AddEvent(new Event(type, data));
        Debug.Log($"Событие {type} данные {data}.");
    }
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
}

