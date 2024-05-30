//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;

//[InitializeOnLoad]
//public class EditorCameraFollow
//{
//    static EditorCameraFollow()
//    {
//        // 에디터가 로드될 때마다 OnSceneGUI 메서드를 호출합니다.
//        SceneView.duringSceneGui += OnSceneGUI;
//    }

//    static void OnSceneGUI(SceneView sceneView)
//    {
//        // 현재 활성화된 메인 카메라를 가져옵니다.
//        Camera mainCamera = Camera.main;

//        // 메인 카메라가 없는 경우 리턴합니다.
//        if (mainCamera == null)
//            return;

//        // 메인 카메라의 위치와 회전을 씬 뷰의 카메라에 적용합니다.
//        sceneView.pivot = mainCamera.transform.position;
//    }
//}
//#endif
