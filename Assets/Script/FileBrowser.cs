
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
    public MediaPlayerCtrl leftVideoPlayer;
    public MediaPlayerCtrl rightVideoPlayer;
    public rotation_master rotationMaster;

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
            GUI.skin.label.fontSize = 50;
            path = DirectoryPanel();
        }
        else
        {
            if (File.Exists(path))
            {
                activatePlayback(path);
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
        GUIStyle customButtonStyle = new GUIStyle("button");
        customButtonStyle.fontSize = 50;

        //arriere plan. | background of the browser
        GUI.Box(new Rect(10, 20, Screen.width/2, Screen.height - 200), "Repertoire");

        if (dI.FullName != "/"){
            if (GUI.Button(new Rect(Screen.width / 4, 40, Screen.width/2, Screen.height / 9), "Dossier parent", customButtonStyle))
            {
                dI = dI.Parent;
            }
        }

        GUILayout.BeginArea(new Rect(Screen.width / 4, 50 + Screen.height / 9, Screen.width / 2, Screen.height - (100 + Screen.height / 9)));
        GUILayout.Label(dI.FullName + " :");
        scrollMax = GUILayout.BeginScrollView(scrollPosition);
        // Repertoire | Directory
        foreach (DirectoryInfo item in dI.GetDirectories())
        {
            if (GUILayout.Button(new GUIContent(item.Name, (Texture2D)Resources.Load("folder-icon")), customButtonStyle))
            {
                dI = item;
            }
        }
        // Fichier | File
        foreach (FileInfo item in dI.GetFiles())
        {
            if (item.Extension == ".rot")
            {
                if (GUILayout.Button(new GUIContent(item.Name), customButtonStyle))
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

    //allow media control and rotation master to start their job, according to the chosen files.
    //This could petentially be inside another class : MasterController.
    //This file browser could then just be used to retreive the path and send it to MasterController.
    private void activatePlayback(string file_path)
    {
        //TODO give the different players the path for their media.
        leftVideoPlayer.Play();
        rightVideoPlayer.Play();
        //rotationMaster.Play();

    }
}

