using UnityEngine;
using System.Collections;
using System.IO;

public class Start_movie : MonoBehaviour {

    private Quaternion rot;
    private Vector3 oldLookingVector;           // Where the camera is the frame before looking
    private float rotX;
    private float rotY;
    private float rotZ;
    private Vector3 newLookingVector;           // Where the camera needs to point

    private float[] gyroX;
    private float[] gyroY;
    private float[] gyroZ;
    private long[] gyroTlong;
    private float[] gyroT;
    private int t = 0;

    public Transform mainCamera;

	// Use this for initialization
	void Start () {
        // Start the movie
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();

        // Set default variables
        oldLookingVector =  transform.rotation.eulerAngles;

        //--------------- R E A D I N G   G Y R O   I N P U T---------------------//
        //----------------------------- B E L O W---------------------------------//
<<<<<<< HEAD
        byte[] gyroTableau = File.ReadAllBytes("./Assets/Movies/gyro_data.rot");
=======

        string fullPath = Path.Combine(Path.Combine(".", "Assets"),Path.Combine("Movies", "gyro_data.rot"));

        byte[] gyroTableau = File.ReadAllBytes(fullPath);
>>>>>>> 60ed62029713a9f9a5d5a9fbc58a856b734c4245
        int capture_size = ((3 * sizeof(float)) + sizeof(long));
        if (gyroTableau.Length % capture_size != 0)
        {
            Debug.Log("Error : size of byte array not a multiple of capture size!!!");
        }   

        int nb_captures = gyroTableau.Length / capture_size;
        gyroX = new float[nb_captures];
        gyroY = new float[nb_captures];
        gyroZ = new float[nb_captures];
        gyroTlong = new long[nb_captures];
        gyroT = new float[nb_captures];


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

            //change
            byte[] tempByte2 = System.BitConverter.GetBytes(System.BitConverter.ToInt64(gyroTableau, (i * capture_size) + 3* sizeof(float)));
            System.Array.Reverse(tempByte2);
            gyroTlong[i] = System.BitConverter.ToInt64(tempByte2, 0);
            gyroT[i] = (gyroTlong[i] - gyroTlong[0])/1E9f;
        }
        //----------------------------- A B O V E---------------------------------//
        //--------------- R E A D I N G   G Y R O   I N P U T---------------------//

    }

    // Update is called once per frame
    void Update() {

        // Updating the rotation
        while (Time.time > gyroT[t] && t < gyroT.Length - 2)
        {
            t++;
        }

        float coeff = Time.deltaTime * 180f / Mathf.PI /(gyroT[t+1]-gyroT[t]);
        rotX = coeff * ( (gyroT[t + 1] - Time.time) * gyroY[t] + (Time.time - gyroT[t]) * gyroY[t + 1] );
        rotY = coeff * ((gyroT[t + 1] - Time.time) * gyroX[t] + (Time.time - gyroT[t]) * gyroX[t + 1]);
        rotZ = coeff * ((gyroT[t + 1] - Time.time) * gyroZ[t] + (Time.time - gyroT[t]) * gyroZ[t + 1]);
      

        rot = transform.rotation * Quaternion.Euler(new Vector3(rotX, -rotY, rotZ));
        mainCamera.rotation = rot;        
    }

}
