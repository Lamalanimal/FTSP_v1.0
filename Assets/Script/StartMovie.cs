using UnityEngine;
using System.IO;

public class StartMovie : MonoBehaviour {
    // Use this for initialization
    void Start() {
        // Start the movie
        #if UNITY_EDITOR && UNITY_ANDROID
        string fullPath = Path.Combine(Path.Combine("Assets", "StreamingAssets"), Path.Combine("Movies", "videoTemp1.mp4"));

            ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
        #endif
        
    }


   
}
