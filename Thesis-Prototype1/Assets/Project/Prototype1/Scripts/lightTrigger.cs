using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class lightTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject VirtualEnvironment;
    public GameObject VirtualLight;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        
            //If the GameObject's name matches the one you suggest, output this message in the console
        Debug.Log("Enter");

        triggerLight("https://maker.ifttt.com/trigger/Light/with/key/mSl1498NtCACZrYh8eAbtz9ZgfAdFtowYUZPsDmyPhb");
        //Disable Environment
        VirtualEnvironment.SetActive(false);
        VirtualLight.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        VirtualEnvironment.SetActive(true);
        VirtualLight.SetActive(true);
        triggerLight("https://maker.ifttt.com/trigger/turnOffLight/with/key/mSl1498NtCACZrYh8eAbtz9ZgfAdFtowYUZPsDmyPhb");
    }

    void triggerLight(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SendWebRequest();
    }
}
