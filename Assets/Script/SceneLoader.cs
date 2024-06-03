using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadingBar;
    public void LoadGameScene()
    {
        //게임 시작
        StartCoroutine(LoadSceneAsync(1));
    }

    public void QuitGame()
    {
        // 게임 종료
        Application.Quit();
    }

    public void GameRetry()    
    {
        //게임 재시작
        SceneManager.LoadScene(1);
    }

    public void LoadTitleScene()
    {
        //메인 매뉴
        StartCoroutine(LoadSceneAsync(0));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)      //해당하는 씬 인덱스를 받아서 비동기 처리
    {
        loadingScreen.SetActive(true);      //로딩창 화면에 표시

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);     //비동기 로딩 진행 상태 추적

        while (!operation.isDone)       //로딩이 완료되지 않았다면
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);      //0~0.9사이 값으로 정규화, 나머지 10퍼는 씬 활성화 작업에 사용
            loadingBar.value = progress;
            yield return null;
        }

        loadingScreen.SetActive(false);     //로딩창 끄기
    }
}
