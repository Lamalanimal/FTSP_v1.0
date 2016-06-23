using UnityEngine;
using System.IO;

public class Start_movie : MonoBehaviour {
	// Use this for initialization
	void Start () {
        // Start the movie

#if UNITY_ANDROID
        string fullPath = Path.Combine(Path.Combine("Assets", "StreamingAssets"), Path.Combine("Movies", "videoTemp1.mp4"));
        Handheld.PlayFullScreenMovie(fullPath);
#else
            ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
#endif



    }

    // Update is called once per frame
    void Update() {

          
    }

}
