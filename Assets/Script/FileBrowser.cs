
using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class FileBrowser : MonoBehaviour
{
    private string path = "";
    private string location = "";
    private DirectoryInfo dI = null;
    //private obj monObjet = null;
    public Shader shader = null;
    private Vector2 scrollPosition;
    private Vector2 scrollMax;

    // Use this for initialization
    void Start()
    {
        location = Application.dataPath;
        location = "/storage/emulated/0/Android/";
        dI = new DirectoryInfo(location);

        scrollPosition = new Vector2();
        scrollMax = new Vector2();
    }

    void OnGUI()
    {
        if (path == "")
        {
            path = DirectoryPanel();
        }
        else
        {
            if (File.Exists(path))
            {
                //if (monObjet == null)
                //    monObjet = new obj(path, shader);
                ;
            }
        }
    }


    void LateUpdate()
    {
        // commande pour l'interface du Browser. | Input for the scrollbar
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (scrollPosition.y < 0) scrollPosition.y = 0;
            if (scrollPosition.y != scrollMax.y) scrollPosition.y = scrollMax.y;
            scrollPosition.y -= Input.GetAxis("Mouse ScrollWheel") * 100;
        }
    }

    private string DirectoryPanel()
    {
        //arriere plan. | background of the browser
        GUI.Box(new Rect(10, 20, Screen.width/2, Screen.height - 200), "Repertoire");

        if (dI.FullName != "/"){
            if (GUI.Button(new Rect(20, 40, Screen.width/2, Screen.height / 9), "Dossier parent"))
            {
                dI = dI.Parent;
            }
        }

        GUILayout.BeginArea(new Rect(30, 50 + Screen.height / 9, Screen.width / 2, Screen.height - (100 + Screen.height / 9)));
        GUILayout.Label(dI.FullName + " :");
        scrollMax = GUILayout.BeginScrollView(scrollPosition);
        // Repertoire | Directory
        foreach (DirectoryInfo item in dI.GetDirectories())
        {
            if (GUILayout.Button(new GUIContent(item.Name, (Texture2D)Resources.Load("folder-icon"))))
            {
                dI = item;
            }
        }
        // Fichier | File
        foreach (FileInfo item in dI.GetFiles())
        {
            if (GUILayout.Button(new GUIContent(item.Name)))
            {
                if (item.Extension == ".rot")
                {
                    return item.FullName;
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        //si rien arrive. | if nothing happens
        return "";
    }
}

