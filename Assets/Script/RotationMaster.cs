using UnityEngine;
using System.IO;

// States
public enum state
{
	NOT_READY,
	READY,
	PLAYING,
	DONE
}

public class RotationMaster : MonoBehaviour {
   
	public state currentState; 

    // Cameras
    public Transform leftEye;
	public Transform rightEye;

	// EyeBoxes
	public Transform leftEyeBox;
	public Transform rightEyeBox;

	// Si on veut faire motifier les rotations dans Update() plutot que LateUpdate()
	// Potentiellement a utiliser avec un ray cast
	public bool updateEarly = false;
	private bool updated;

	// Donnes des rotations
    private float[] gyroX;
    private float[] gyroY;
    private float[] gyroZ;
	private float[] gyroT;
    private int t = 0;

	// Delais
	public float clapGoPro; 	// Moment du clap dans les GoPros
	public float clapCell; 		// Moment du clap dans le video du cell
	public float retard; 		// Avance du gyro_data sur les videos
	private float delai;        // Delai combine

    
    // rotation_master lit les donnees de rotations enregistrees (gyro_data) et les applique aux eyeBoxes
    // En plus, rotation_master applique les rotations du cell et les applique aux cameras en utilisant le code de GvrHead
    // Il faut donc que trackPosition et trackRotation de GvrHead soit set a false.

    // Use this for initialization
    void Start () {
        currentState = state.NOT_READY;
    }

    // Update is called once per frame
    void Update () {

		// L'initialisation a reussie, on commence a faire jouer
        if (currentState == state.READY)
        {
            currentState = state.PLAYING;

			// Ajustement du delai selon le temps qu'a dure l'initialisation
			delai -= Time.time;
        }
            
		// On applique les rotations des EyeBoxes si le video est en train de jouer
        if (currentState == state.PLAYING)
        {
            rotateEyeBox(); 
        }
        
        // Update la rotation des yeux maintenant si earlyUpdate
        updated = false;  // OK to recompute head pose.
        if (updateEarly)
        {
            UpdateEyes();
        }

    }

	// Rotation des EyeBoxes
    private void rotateEyeBox()
    {
        // Mise a jour de la position du pointeur t
        while (Time.time + delai > gyroT[t] && t < gyroT.Length - 2)
        {
            t++;
        }

		// On applique la rotation des EyeBoxes
        if (t < gyroT.Length - 2)
        {
            float coeff = Time.deltaTime * 180f / Mathf.PI / (gyroT[t + 1] - gyroT[t]);
            float rotX = coeff * ((gyroT[t + 1] - Time.time) * gyroY[t] + (Time.time - gyroT[t]) * gyroY[t + 1]);
            float rotY = coeff * ((gyroT[t + 1] - Time.time) * gyroX[t] + (Time.time - gyroT[t]) * gyroX[t + 1]);
            float rotZ = coeff * ((gyroT[t + 1] - Time.time) * gyroZ[t] + (Time.time - gyroT[t]) * gyroZ[t + 1]);

            leftEyeBox.rotation = leftEyeBox.rotation * Quaternion.Euler(new Vector3(rotX, -rotY, rotZ));
            rightEyeBox.rotation = rightEyeBox.rotation * Quaternion.Euler(new Vector3(rotX, -rotY, rotZ));
        }
        else
        {
			// On arrete les rotations
            // Debug.Log("Stop the rotation!");
            leftEyeBox.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            rightEyeBox.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            currentState = state.DONE;
        }
    }

	// Preparation des donnees de rotation
    public void initializeGyroData(string path)
    {
        if (currentState == state.NOT_READY)
        {
            // Initialisation des delais
			delai = retard + clapCell - clapGoPro;

            //--------------- R E A D I N G   G Y R O   I N P U T---------------------//
            //----------------------------- B E L O W---------------------------------//

            //http://docs.unity3d.com/Manual/StreamingAssets.html
            //string fullPath = "jar:file://" + Application.dataPath + "!/assets/gyro_data.rot";
            //#string fullPath = "";
            //fullPath = "/storage/emulated/0/Android/data/gyro_data.rot";
            //		fullPath = Path.Combine(Path.Combine(".", "Assets"), Path.Combine("Movies", "gyro_data.rot"));
            string fullPath = path;
            Debug.Log(fullPath);


            // Les donnes sont sous forme d'un tableau de byte
            byte[] gyroTableau = File.ReadAllBytes(fullPath);
            int capture_size = ((3 * sizeof(float)) + sizeof(long));
            if (gyroTableau.Length % capture_size != 0)
            {
                Debug.Log("Error : size of byte array not a multiple of capture size!!!");
            }

            // Initialisation des tableaux de donnes
            int nb_captures = gyroTableau.Length / capture_size;
            gyroX = new float[nb_captures];
            gyroY = new float[nb_captures];
            gyroZ = new float[nb_captures];

            long[] gyroTlong;
            gyroTlong = new long[nb_captures];
            gyroT = new float[nb_captures];

            // Lecture des donnes et conversion des bytes (Little Endian / Big Endian)
            for (int i = 0; i < nb_captures; i++)
            {

                byte[] tempByte = System.BitConverter.GetBytes(System.BitConverter.ToSingle(gyroTableau, (i * capture_size)));
                System.Array.Reverse(tempByte);
                gyroX[i] = System.BitConverter.ToSingle(tempByte, 0);

                tempByte = System.BitConverter.GetBytes(System.BitConverter.ToSingle(gyroTableau, (i * capture_size) + sizeof(float)));
                System.Array.Reverse(tempByte);
                gyroY[i] = System.BitConverter.ToSingle(tempByte, 0);

                tempByte = System.BitConverter.GetBytes(System.BitConverter.ToSingle(gyroTableau, (i * capture_size) + 2 * sizeof(float)));
                System.Array.Reverse(tempByte);
                gyroZ[i] = System.BitConverter.ToSingle(tempByte, 0);

                byte[] tempByte2 = System.BitConverter.GetBytes(System.BitConverter.ToInt64(gyroTableau, (i * capture_size) + 3 * sizeof(float)));
                System.Array.Reverse(tempByte2);
                gyroTlong[i] = System.BitConverter.ToInt64(tempByte2, 0);

                // Conversion du temps de nansecondes a secondes
                gyroT[i] = (gyroTlong[i] - gyroTlong[0]) / 1E9f;
            }
            //----------------------------- A B O V E---------------------------------//
            //--------------- R E A D I N G   G Y R O   I N P U T---------------------//
            currentState = state.READY;
        }     
    }

    // Normally, update head pose now.
    void LateUpdate()
    {

        UpdateEyes();
        
    }

    // Compute new head pose.
    private void UpdateEyes()
    {
		if (updated) {  // Only one update per frame, please.
			return;
		}
		updated = true;

        GvrViewer.Instance.UpdateState();

        var rot = GvrViewer.Instance.HeadPose.Orientation;

        if (leftEye != null || rightEye!=null)
        {
            leftEye.rotation = rot;
            rightEye.rotation = rot;
        }

    }
}
