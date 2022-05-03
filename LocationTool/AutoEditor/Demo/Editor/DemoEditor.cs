using UnityEditor;


[CustomEditor(typeof(Demo))]
public class DemoEditor : Editor
{
    Demo demo = null;

    private void OnEnable()
    {
        demo = target as Demo;
    }

    public override void OnInspectorGUI()
    {
        demo.SettingsAutoEd.Build();
    }
}
