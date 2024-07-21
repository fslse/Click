using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        DelayLog().Forget();
        Observable.EveryUpdate().First(_ => Input.GetMouseButton(0)).Subscribe(_ => tmp.text = "UniRx");
        Debug.LogError("SRDebugger");
    }

    private async UniTaskVoid DelayLog()
    {
        await UniTask.Delay(3000);
        tmp.text = "UniTask";
    }
}