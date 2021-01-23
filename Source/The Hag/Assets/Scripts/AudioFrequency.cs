using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioFrequency : MonoBehaviour
{
    string ID;
    float frequencyInSeconds;

    float timeElapsedWhileMute = 0f;
    float frequencyCounter = 0f;
    bool hasPlayed = false;

    public static AudioFrequency getInstance(string id, GameObject gameObject)
    {
        AudioFrequency af = Array.Find(gameObject.GetComponents<AudioFrequency>(), instance => instance.ID == id);

        if (af == null)
        {
            af = gameObject.AddComponent<AudioFrequency>();
            af.ID = id;
        }
        return af;
    }

    void Update()
    {
        timeElapsedWhileMute += Time.deltaTime;

        if(timeElapsedWhileMute > frequencyInSeconds)
        {
            Component.Destroy(this);
        }
    }

    void countFreq()
    {
        if(frequencyCounter < frequencyInSeconds)
        {
            frequencyCounter += Time.deltaTime / frequencyInSeconds;
        }
        else
        {
            frequencyCounter = 0f;
            hasPlayed = false;
        }
    }

    public bool isWaiting(float frequencyInSeconds)
    {
        if (!hasPlayed)
        {
            this.frequencyInSeconds = frequencyInSeconds;
            hasPlayed = true;

            return false;
        }
        else 
        {
            timeElapsedWhileMute = 0;
            countFreq();

            return true; 
        }
    }
}
