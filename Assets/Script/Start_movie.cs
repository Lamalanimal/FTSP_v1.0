using UnityEngine;
using System.Collections;
using System.IO;

public class Start_movie : MonoBehaviour {

   
    public Transform mainCamera;

	// Use this for initialization
	void Start () {
        // Start the movie
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();

        byte[] gyroTableau = File.ReadAllBytes(".\\Assets\\Movies\\gyro_data.rot");
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
            gyroT[i] = System.BitConverter.ToInt64(tempByte2, 0);
        }
        Debug.Log(sizeof(long));
        Debug.Log(gyroT[nb_captures - 1] - gyroT[0]);
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        /*if (Time.fixedTime>gyroT[i])
        {



        }
        */

    }

}
