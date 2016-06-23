﻿using UnityEngine;
using System.IO;

public class rotation_master : MonoBehaviour {

    private Quaternion rot;
    private float rotX;
    private float rotY;
    private float rotZ;

    private float[] gyroX;
    private float[] gyroY;
    private float[] gyroZ;
    private long[] gyroTlong;
    private float[] gyroT;
    private int t = 0;

    public Transform leftEye;
    public Transform rightEye;


    // Use this for initialization
    void Start () {
        //--------------- R E A D I N G   G Y R O   I N P U T---------------------//
        //----------------------------- B E L O W---------------------------------//

        string fullPath = Path.Combine(Path.Combine(".", "Assets"), Path.Combine("Movies", "gyro_data.rot"));
        byte[] gyroTableau = File.ReadAllBytes(fullPath);
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
            byte[] tempByte2 = System.BitConverter.GetBytes(System.BitConverter.ToInt64(gyroTableau, (i * capture_size) + 3 * sizeof(float)));
            System.Array.Reverse(tempByte2);
            gyroTlong[i] = System.BitConverter.ToInt64(tempByte2, 0);
            gyroT[i] = (gyroTlong[i] - gyroTlong[0]) / 1E9f;
        }
        //----------------------------- A B O V E---------------------------------//
        //--------------- R E A D I N G   G Y R O   I N P U T---------------------//
    }

    // Update is called once per frame
    void Update () {
        // Updating the rotation
        while (Time.time > gyroT[t] && t < gyroT.Length - 2)
        {
            t++;
        }

        float coeff = Time.deltaTime * 180f / Mathf.PI / (gyroT[t + 1] - gyroT[t]);
        rotX = coeff * ((gyroT[t + 1] - Time.time) * gyroY[t] + (Time.time - gyroT[t]) * gyroY[t + 1]);
        rotY = coeff * ((gyroT[t + 1] - Time.time) * gyroX[t] + (Time.time - gyroT[t]) * gyroX[t + 1]);
        rotZ = coeff * ((gyroT[t + 1] - Time.time) * gyroZ[t] + (Time.time - gyroT[t]) * gyroZ[t + 1]);

        leftEye.rotation = leftEye.rotation * Quaternion.Euler(new Vector3(rotX, -rotY, rotZ));
        rightEye.rotation = rightEye.rotation * Quaternion.Euler(new Vector3(rotX, -rotY, rotZ));
    }

    // Normally, update head pose now.
    void LateUpdate()
    {
        UpdateEyes();
    }

    // Compute new head pose.
    private void UpdateEyes()
    {
        /*
        if (updated)
        {  // Only one update per frame, please.
            return;
        }
        updated = true;
        */
        GvrViewer.Instance.UpdateState();

        var rot = GvrViewer.Instance.HeadPose.Orientation;

        if (leftEye != null || rightEye!=null)
        {
            leftEye.rotation = leftEye.rotation * rot;
            rightEye.rotation = rightEye.rotation * rot;
        }

        /*
        if (OnHeadUpdated != null)
        {
            OnHeadUpdated(gameObject);
        }
        */
    }
}
