using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using System.Collections.Generic;
using IntentModels;
using UnityEngine.Networking;

public class IntentRestClient : MonoBehaviour {

	private readonly string basePath = "http://localhost:1701";
	private RequestHelper currentRequest;

	private void LogMessage(string title, string message) {
		Debug.Log(message);
	}
	
	public void Instruct(string sents){

		RestClient.DefaultRequestParams["lang"] = "en";

		currentRequest = new RequestHelper {
			Uri = basePath + "/info/behave/en",
			Params = new Dictionary<string, string> {
				{ "param1", "value 1" },
				{ "param2", "value 2" }
			},
			Body = new InstructMessage {
				sents = sents
			},
			EnableDebug = true
		};
		RestClient.PostArray<IntentResponse>(currentRequest)
			.Then(res => {

				// And later we can clear the default query string params for all requests
				RestClient.ClearDefaultParams();

				// this.LogMessage("Success", JsonUtility.ToJson(res, true));
				this.LogMessage("Success", JsonHelper.ArrayToJsonString<IntentResponse>(res, true));
				this.ProcessMessages(res);
			})
			.Catch(err => this.LogMessage("Error", err.Message));
	}

	void ProcessMessages(IntentResponse[] responses)
	{
		var intent = responses[0];
		if (intent.intent == "injured")
		{
			gameObject.BroadcastMessage("ChangeHealth", -intent.value);
		}else if (intent.intent == "dead")
		{
			gameObject.BroadcastMessage("Die");
		}
		else
		{
			Debug.LogWarning("Cannot handle intent type "+intent.intent);
		}
	}


	public void AbortRequest(){
		if (currentRequest != null) {
			currentRequest.Abort();
			currentRequest = null;
		}
	}

	public void DownloadFile(){

		var fileUrl = "https://raw.githubusercontent.com/IonDen/ion.sound/master/sounds/bell_ring.ogg";
		var fileType = AudioType.OGGVORBIS;

		RestClient.Get(new RequestHelper {
			Uri = fileUrl,
			DownloadHandler = new DownloadHandlerAudioClip(fileUrl, fileType)
		}).Then(res => {
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = ((DownloadHandlerAudioClip)res.Request.downloadHandler).audioClip;
			audio.Play();
		}).Catch(err => {
			this.LogMessage("Error", err.Message);
		});
	}
}