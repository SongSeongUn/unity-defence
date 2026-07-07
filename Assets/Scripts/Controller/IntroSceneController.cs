using UnityEngine;
using Cysharp.Threading.Tasks;
using Managers;
using Table.TBL;
using UI.UIWindow;
using UnityEngine.UI;

public class IntroSceneController : MonoBehaviour 
{
    private void Awake()
    {
        // 카메라 가져오가나 등

        AsyncAwaitLoad().Forget();
    }

    private async UniTask AsyncAwaitLoad()
    {
        await DataTableManager.LoadDataTable();

        await UIManager.Create();
        UI_IntroScene_Page ui = await UIManager.Instance.OpenUI<UI_IntroScene_Page>("UI_IntroScene_Page");
        ui.AddButtonsEvents(StartGameScene, ExitGame);
    }

    public async void StartGameScene()
    {
        //TODO 게임로비로 바꿔야함
        DebugUtils.Log("시작");
        await SceneControllManager.Instance.LoadScene("StageScene");
    }


    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}