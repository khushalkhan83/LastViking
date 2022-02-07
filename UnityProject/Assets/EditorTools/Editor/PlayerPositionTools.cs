using System.Collections;
using System.Collections.Generic;
using System.IO;
using UltimateSurvival;
using UnityEditor;
using UnityEngine;

public static class PlayerPositionTools
{
    

    [MenuItem("EditorTools/Teleport/Move player to View #%t")] // shift ctrl-y on Windows, shift cmd-t on macOS
    private static void MovePlayerToView()
    {
        bool error = !TryGetPlayerAndCamera(out var playerTransform, out var sceneCameraTransform);

        if(error) return;

        if(!EditorInPlayMode())
        {
            EditorUtility.DisplayDialog("Error", "Shortcuts work only in play mode", "Ok");
            return;
        }

        var newRotation = new Vector3(0,
                                      sceneCameraTransform.transform.rotation.eulerAngles.y,
                                      0);

        playerTransform.position = sceneCameraTransform.position;
        playerTransform.rotation = Quaternion.Euler(newRotation);
    }

    [MenuItem("EditorTools/Teleport/Focus on Player #t")] // shift ctrl-f on Windows, shift cmd-t on macOS
    public static void FocustOnPlayer()
    {
         bool error = !TryGetPlayerAndCamera(out var playerTransform, out var sceneCameraTransform);

        if(error) return;

        if(!EditorInPlayMode())
        {
            EditorUtility.DisplayDialog("Error", "Shortcuts work only in play mode", "Ok");
            return;
        }

        var lastSelectedObject = Selection.activeGameObject;

        Selection.activeGameObject = playerTransform.gameObject;
        SceneView.lastActiveSceneView.FrameSelected();
        Selection.activeGameObject = lastSelectedObject;
    }

    private static bool TryGetPlayerAndCamera( out Transform playerTransform, out Transform sceneCameraTransform)
    {
        playerTransform = null;
        sceneCameraTransform = null;

        var player = GameObject.FindObjectOfType<PlayerEventHandler>();
        if(player == null) 
        {
            EditorUtility.DisplayDialog("Error", "Cant find player in scene", "Ok");
            return false;
        }
        playerTransform = player.transform;

        var sceneView = SceneView.lastActiveSceneView;

        if(sceneView == null) 
        {
            EditorUtility.DisplayDialog("Error", "Cant find scene view opened. Try to open scene view window", "Ok");
            return false;
        }

        var sceneViewCamera = sceneView.camera;

        if(sceneViewCamera == null) 
        {
            EditorUtility.DisplayDialog("Error", "Cant find opened scene view", "Ok");
            return false;
        }
        sceneCameraTransform = sceneViewCamera.transform;

        return true;
    }

    private static bool EditorInPlayMode()
    {
        return EditorApplication.isPlaying;
    }
}
