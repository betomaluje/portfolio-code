using UnityEditor;
using UnityEditor.Callbacks;

public class UpdateProjectVersion {
    // PostProcessBuildAttribute has an option to provide an order index in the callback, starting at 0. 
    // This is useful if you have more than one PostProcessBuildAttribute callback, and you would like them to be called in a certain order. 
    //Callbacks are called in order, starting at zero.
    [PostProcessBuildAttribute(0)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        string monthAndDay = System.DateTime.Now.ToString("MMdd");
        PlayerSettings.bundleVersion = $"0.2.{monthAndDay}-beta";
    }
}