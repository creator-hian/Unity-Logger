using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

/// <summary>
/// Unity 테스트에서 스크립팅 심볼을 동적으로 관리하기 위한 기본 테스트 클래스입니다.
/// </summary>
public abstract class ScriptingSymbolTestBase
{
    protected const string UNITY_EDITOR_SYMBOL = "UNITY_EDITOR";
    protected const string DEVELOPMENT_BUILD_SYMBOL = "DEVELOPMENT_BUILD";

    private string[] _originalSymbols;
    private BuildTargetGroup _buildTargetGroup;

    [OneTimeSetUp]
    public virtual void BaseSetUp()
    {
        _buildTargetGroup = BuildPipeline.GetBuildTargetGroup(
            EditorUserBuildSettings.activeBuildTarget
        );
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            _buildTargetGroup
        );
        _originalSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';');
    }

    [OneTimeTearDown]
    public virtual void BaseTearDown()
    {
        // 원래의 심볼들 복원
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            _buildTargetGroup
        );
        PlayerSettings.SetScriptingDefineSymbols(
            namedBuildTarget,
            string.Join(";", _originalSymbols)
        );
    }

    /// <summary>
    /// 스크립팅 심볼들을 설정합니다. 기존 심볼들은 모두 제거됩니다.
    /// </summary>
    /// <param name="symbols">설정할 심볼 배열</param>
    protected void SetSymbols(params string[] symbols)
    {
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            _buildTargetGroup
        );
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", symbols));
    }

    /// <summary>
    /// 기존 심볼들을 유지하면서 새로운 심볼을 추가합니다.
    /// </summary>
    /// <param name="symbol">추가할 심볼</param>
    protected void AddSymbol(string symbol)
    {
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            _buildTargetGroup
        );
        string[] currentSymbols = PlayerSettings
            .GetScriptingDefineSymbols(namedBuildTarget)
            .Split(';');
        if (!System.Array.Exists(currentSymbols, s => s == symbol))
        {
            string[] newSymbols = new string[currentSymbols.Length + 1];
            System.Array.Copy(currentSymbols, newSymbols, currentSymbols.Length);
            newSymbols[currentSymbols.Length] = symbol;
            PlayerSettings.SetScriptingDefineSymbols(
                namedBuildTarget,
                string.Join(";", newSymbols)
            );
        }
    }

    /// <summary>
    /// 특정 심볼을 제거합니다.
    /// </summary>
    /// <param name="symbol">제거할 심볼</param>
    protected void RemoveSymbol(string symbol)
    {
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            _buildTargetGroup
        );
        string[] currentSymbols = PlayerSettings
            .GetScriptingDefineSymbols(namedBuildTarget)
            .Split(';');
        string[] newSymbols = System.Array.FindAll(currentSymbols, s => s != symbol);
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", newSymbols));
    }

    protected void AddDevelopmentBuildSymbol()
    {
        AddSymbol(DEVELOPMENT_BUILD_SYMBOL);
    }

    protected void RemoveDevelopmentBuildSymbol()
    {
        RemoveSymbol(DEVELOPMENT_BUILD_SYMBOL);
    }

    protected void RemoveEditorAndDevelopmentSymbols()
    {
        RemoveSymbol(UNITY_EDITOR_SYMBOL);
        RemoveSymbol(DEVELOPMENT_BUILD_SYMBOL);
    }

    protected void RestoreEditorSymbol()
    {
        AddSymbol(UNITY_EDITOR_SYMBOL);
    }
}
