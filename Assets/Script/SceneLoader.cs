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
        //���� ����
        StartCoroutine(LoadSceneAsync(1));
    }

    public void QuitGame()
    {
        // ���� ����
        Application.Quit();
    }

    public void GameRetry()    
    {
        //���� �����
        SceneManager.LoadScene(1);
    }

    public void LoadTitleScene()
    {
        //���� �Ŵ�
        StartCoroutine(LoadSceneAsync(0));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)      //�ش��ϴ� �� �ε����� �޾Ƽ� �񵿱� ó��
    {
        loadingScreen.SetActive(true);      //�ε�â ȭ�鿡 ǥ��

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);     //�񵿱� �ε� ���� ���� ����

        while (!operation.isDone)       //�ε��� �Ϸ���� �ʾҴٸ�
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);      //0~0.9���� ������ ����ȭ, ������ 10�۴� �� Ȱ��ȭ �۾��� ���
            loadingBar.value = progress;
            yield return null;
        }

        loadingScreen.SetActive(false);     //�ε�â ����
    }
}
