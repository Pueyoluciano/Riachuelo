using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(fileName = "NewSceneDatabase", menuName = "ScriptableObjects/Scenes/SceneDatabase", order = 1)]
public class Scenes : ScriptableObject
{
    [System.Serializable]
    public class SceneReference
    {
        [SerializeField] private SceneAsset sceneAsset; // Solo en Editor
        [SerializeField] private string sceneName; // Usado en runtime

        public string SceneName { get => sceneName; set => sceneName = value; }

#if UNITY_EDITOR
        public void UpdateSceneName()
        {
            if (sceneAsset != null)
            {
                SceneName = sceneAsset.name;
            }
            else
            {
                SceneName = "";
            }
        }
#endif
    }
    [field: SerializeField] public SceneReference Location { get; set; }
    [field: SerializeField] public SceneReference MainMenu { get; set; }
    private void OnValidate()
    {
        PropertyInfo[] properties = typeof(Scenes).GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(SceneReference))
            {
                var obj = property.GetValue(this);

                if (obj != null)
                    ((SceneReference)obj).UpdateSceneName();
            }
        }
    }
}
