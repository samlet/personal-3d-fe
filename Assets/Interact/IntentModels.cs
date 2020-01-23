using System;

namespace IntentModels
{
    [Serializable]
    public class IntentResponse
    {
        public string intent;

        public int value=0;

        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
    
    [Serializable]
    public class InstructMessage
    {
        public string sents;

        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}

