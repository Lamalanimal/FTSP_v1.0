using UnityEngine;
using System.Collections;
using System.IO;

public class Start_movie : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();


        byte[] gyroTableau = File.ReadAllBytes("\\Testing\\Assets\\Movies\\gyro_data");
        int capture_size = ((3 * sizeof(float)) + sizeof(long));
        if (gyroTableau.Length % capture_size != 0)
        {
            Debug.Log("Error : size of byte array not a multiple of capture size!!!");
        }   
        int nb_captures = gyroTableau.Length / capture_size;
        float[] gyroX = new float[nb_captures];
        float[] gyroY = new float[nb_captures];
        float[] gyroZ = new float[nb_captures];
        long[] gyroT = new long[nb_captures];

        for (int i = 0; i < nb_captures; i++)
        {
            gyroX[i] = System.BitConverter.ToSingle(gyroTableau, i * capture_size);
            gyroY[i] = System.BitConverter.ToSingle(gyroTableau, (i * capture_size) + sizeof(float));
            gyroZ[i] = System.BitConverter.ToSingle(gyroTableau, (i * capture_size) + 2 * sizeof(float));
            gyroT[i] = System.BitConverter.ToInt64(gyroTableau, (i * capture_size) + 3 * sizeof(float));
        }

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Felix est tres con");
    }
}
