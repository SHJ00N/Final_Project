//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;

//[InitializeOnLoad]
//public class EditorCameraFollow
//{
//    static EditorCameraFollow()
//    {
//        // �����Ͱ� �ε�� ������ OnSceneGUI �޼��带 ȣ���մϴ�.
//        SceneView.duringSceneGui += OnSceneGUI;
//    }

//    static void OnSceneGUI(SceneView sceneView)
//    {
//        // ���� Ȱ��ȭ�� ���� ī�޶� �����ɴϴ�.
//        Camera mainCamera = Camera.main;

//        // ���� ī�޶� ���� ��� �����մϴ�.
//        if (mainCamera == null)
//            return;

//        // ���� ī�޶��� ��ġ�� ȸ���� �� ���� ī�޶� �����մϴ�.
//        sceneView.pivot = mainCamera.transform.position;
//    }
//}
//#endif
