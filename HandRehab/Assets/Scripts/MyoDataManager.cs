using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class MyoDataManager : MonoBehaviour
{

    public float strength;
    public string arm;

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update(){}

    public void SetStrength(float newStrength)
    {
        strength = newStrength;
    }

    public void SetArm(string newArm)
    {
        arm = newArm;
    }

    public IEnumerator GetMyoData(string address)
    {
        // Request GET from server
        UnityWebRequest www = UnityWebRequest.Get(address);
        yield return www.SendWebRequest();


        // Verify if response has an error
        if (www.isHttpError)
        {
            Debug.LogError(www.error);
            Debug.LogError(address);
        }
        // Proccess Response from text to a JSON
        else{
            JSONNode response = ProccessServerResponse(www.downloadHandler.text);

            if (response != null) {
                // Verify if data object is null and if strength's data is lower than 3. 
                // The reason behind this is to avoid interfearing values where makes the strength too powerfull
                if (response["strength"] != null && 
                    response["strength"] < 3)
                {
                    SetStrength(response["strength"]);
                }

                if (response["arm"] != null)
                {
                    Debug.Log("Arm: " + response["arm"]);
                    SetArm(response["arm"]);
                }
            }            
        } 


    }

    JSONNode ProccessServerResponse(string rawResponse)
    {
        // That text, is actually a JSON info, so we need to parse that into something we can navigate.
        JSONNode node = JSON.Parse(rawResponse);

        // Changes current strength value in case the server response was successfull
        if (node != null && 
            node["meta"] != null && 
            node["meta"]["success"] == true) 
        {
            return node["data"];
        } else 
        {
            return null;
        }
    }

}
