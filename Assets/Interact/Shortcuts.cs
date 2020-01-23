using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using CreatorKitCodeInternal;
using Models;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Shortcuts : MonoBehaviour
{
    public GameObject character;
    public GameObject textInput;
    public GameObject prompt;
    
    private CharacterData m_CharacterData;
    private CharacterAudio m_CharacterAudio;
    
    private readonly string basePath = "https://jsonplaceholder.typicode.com";
    private RequestHelper currentRequest;
    private IntentRestClient restClient;
    
    // Start is called before the first frame update
    void Start()
    {
        m_CharacterData = character.GetComponent<CharacterData>();
        m_CharacterAudio = character.GetComponent<CharacterAudio>();
        restClient = GetComponent<IntentRestClient>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyUp(KeyCode.I))
        //    UISystem.Instance.ToggleInventory();
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Debug.Log(".. press alpha 1");
            Die();
        }else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Debug.Log(".. press alpha 2");
            m_CharacterAudio.Injured(transform.position);
            m_CharacterData.Stats.ChangeHealth(-5);
        }else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Debug.Log(".. press alpha 3");
            Post();
        }
    } 

    public void Die()
    {
        m_CharacterData.Stats.ChangeHealth(-1000);
    }

    public void Display()
    {
        var input=textInput.GetComponent<InputField>();
        var promptText = prompt.GetComponent<Text>();
        Debug.Log(".. call display proc: "+input.text);
        promptText.text = "You say: " + input.text;
        
        restClient.Instruct(input.text);
        // clear the input text
        input.text = "";
    }
    
    public void Post(){

        // We can add default query string params for all requests
        RestClient.DefaultRequestParams["param1"] = "My first param";
        RestClient.DefaultRequestParams["param3"] = "My other param";

        currentRequest = new RequestHelper {
            Uri = basePath + "/posts",
            Params = new Dictionary<string, string> {
                { "param1", "value 1" },
                { "param2", "value 2" }
            },
            Body = new Post {
                title = "foo",
                body = "bar",
                userId = 1
            },
            EnableDebug = true
        };
        RestClient.Post<Post>(currentRequest)
            .Then(res => {

                // And later we can clear the default query string params for all requests
                RestClient.ClearDefaultParams();

                this.LogMessage("Success", JsonUtility.ToJson(res, true));
            })
            .Catch(err => this.LogMessage("Error", err.Message));
    }

    public void ChangeHealth(int amount)
    {
        m_CharacterAudio.Injured(transform.position);
        m_CharacterData.Stats.ChangeHealth(amount);
    }
    
    private void LogMessage(string title, string message) {
		Debug.Log(message);
        gameObject.BroadcastMessage("ChangeHealth", -10);
    }
}
