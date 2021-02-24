using System;
using UnityEngine;

public class FrequencyTimer : MonoBehaviour
{
    string ID;
    float frequencyInSeconds;

    float timeElapsedWhileMute = 0f;
    float frequencyCounter = 0f;

    bool hasStarted = false;
    bool hasEnded = false;

    public static FrequencyTimer getInstance(string ID, GameObject gameObject)
    {
        String prefixID = gameObject.name + "_";

        FrequencyTimer ft = Array.Find(gameObject.GetComponents<FrequencyTimer>(), instance => instance.ID == prefixID+ID);

        if (ft == null)
        {
            ft = gameObject.AddComponent<FrequencyTimer>();
            ft.ID = prefixID + ID;
        }
        return ft;
    }

    void Update()
    {
        if (hasStarted)
        {
            timeElapsedWhileMute = 0;
            countFreq();
        }
        else
        {
            timeElapsedWhileMute += Time.deltaTime;
        }

        if(timeElapsedWhileMute > frequencyInSeconds)
        {
            Component.Destroy(this);
        }
    }

    void countFreq()
    {
        if(frequencyCounter < frequencyInSeconds)
        {
            frequencyCounter += Time.deltaTime * 0.9f;
        }
        else
        {
            frequencyCounter = 0f;
            hasStarted = false;
            hasEnded = true;
        }
    }

    public bool isStartPhase(float frequencyInSeconds)
    {
        if (!hasStarted)
        {
            this.frequencyInSeconds = frequencyInSeconds;
            hasStarted = true;
            hasEnded = false;

            return true;
        }
        else 
        {
            return false; 
        }
    }

    public bool isEndPhase(float frequencyInSeconds)
    {
        if (!hasStarted)
        {
            this.frequencyInSeconds = frequencyInSeconds;
            hasStarted = true;
        }

        if (hasEnded)
        {
            hasEnded = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
