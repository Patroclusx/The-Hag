using System;
using UnityEngine;

public class FrequencyTimer : MonoBehaviour
{
    string ID;
    float frequencyInSeconds;

    float timeElapsedWhileMute = 0f;
    float frequencyCounter = 0f;
    bool hasStarted = false;

    public static FrequencyTimer getInstance(string ID, GameObject gameObject)
    {
        FrequencyTimer ft = Array.Find(gameObject.GetComponents<FrequencyTimer>(), instance => instance.ID == ID);

        if (ft == null)
        {
            ft = gameObject.AddComponent<FrequencyTimer>();
            ft.ID = ID;
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

        timeElapsedWhileMute += Time.deltaTime * 0.85f;

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
        }
    }

    public bool isWaiting(float frequencyInSeconds)
    {
        if (!hasStarted)
        {
            this.frequencyInSeconds = frequencyInSeconds;
            hasStarted = true;

            return false;
        }
        else 
        {
            return true; 
        }
    }
}
