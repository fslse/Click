using UnityEditor;

/// <summary>
/// 游戏框架 Inspector 抽象类。
/// </summary>
public abstract class GameFrameworkInspector : Editor
{
    private bool isCompiling;

    /// <summary>
    /// 绘制事件。
    /// </summary>
    public override void OnInspectorGUI()
    {
        switch (isCompiling)
        {
            case true when !EditorApplication.isCompiling:
                isCompiling = false;
                OnCompileComplete();
                break;
            case false when EditorApplication.isCompiling:
                isCompiling = true;
                OnCompileStart();
                break;
        }
    }

    /// <summary>
    /// 编译开始事件。
    /// </summary>
    protected virtual void OnCompileStart()
    {
    }

    /// <summary>
    /// 编译完成事件。
    /// </summary>
    protected virtual void OnCompileComplete()
    {
    }

    protected bool IsPrefabInHierarchy(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return false;
        }

        return PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.Regular;
    }
}