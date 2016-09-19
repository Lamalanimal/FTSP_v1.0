using UnityEngine;
using System.Collections;

public class MovieMaster : MonoBehaviour {

	// LOOP !
	public bool loop;

	// Duree du film
	public float duration;

	// Les object qui servent d'ecrans
	// UNITY_EDITOR
	public GameObject leftScreenUnity;
	public GameObject rightScreenUnity;
	// ANDROID
	public GameObject leftScreenAndroid;
	public GameObject rightScreenAndroid;

	// Components qui controle les movies
	// UNITY_EDITOR
	private MovieTexture leftMovieUnity;
	private MovieTexture rightMovieUnity;
	// ANDROID
	private MediaPlayerCtrl leftMovieAndroid;
	private MediaPlayerCtrl rightMovieAndroid;

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		// Activation et desactivation des bons ecrans
		leftScreenUnity.SetActive( true );
		rightScreenUnity.SetActive( true );
		leftScreenAndroid.SetActive( false );
		rightScreenAndroid.SetActive( false );

		// Recuperation des movie components
		Renderer rendererMovie = leftScreenUnity.GetComponent<Renderer>();
		leftMovieUnity = (MovieTexture) rendererMovie.material.mainTexture;
		rendererMovie = rightScreenUnity.GetComponent<Renderer>();
		rightMovieUnity = (MovieTexture) rendererMovie.material.mainTexture;

		// Set de l'option loop
		leftMovieUnity.loop = loop;
		rightMovieUnity.loop = loop;

		// Recuperation de la duree
		duration = leftMovieUnity.duration;

		#elif UNITY_ANDROID
		// Activation et desactivation des bons ecrans
		leftScreenUnity.SetActive( false );
		rightScreenUnity.SetActive( false );
		leftScreenAndroid.SetActive( true );
		rightScreenAndroid.SetActive( true );

		// Recuperation des movie components
		leftMovieAndroid = leftScreenAndroid.GetComponent<MediaPlayerCtrl>();
		rightMovieAndroid = rightScreenAndroid.GetComponent<MediaPlayerCtrl>();

		// Set de l'option loop
		leftMovieAndroid.m_bLoop = loop;
		rightMovieAndroid.m_bLoop = loop;

		// Recuperation de la duree (en ms dans MediaPlayerCtrl)
		duration = ( (float)leftMovieAndroid.GetDuration() ) * 0.001f;

		#endif
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// On start le shit
	public void Play () {
		#if UNITY_EDITOR
			leftMovieUnity.Play();
			rightMovieUnity.Play();
		#elif UNITY_ANDROID
			leftMovieAndroid.Play();
			rightMovieAndroid.Play();
		#endif
	}
}
