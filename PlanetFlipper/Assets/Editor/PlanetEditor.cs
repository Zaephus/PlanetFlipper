using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {

    Planet planet;

    Editor shapeEditor;
    Editor colourEditor;

    public override void OnInspectorGUI() {

        using(var check = new EditorGUI.ChangeCheckScope()) {

            base.OnInspectorGUI();

            if(check.changed) {
                planet.GeneratePlanet();
            }

        }

        if(GUILayout.Button("Generate Planet")) {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);

    }

    private  void DrawSettingsEditor(Object _settings, System.Action _onSettingsUpdated, ref bool _foldout, ref Editor _editor) {

        if(_settings != null) {

            _foldout = EditorGUILayout.InspectorTitlebar(_foldout, _settings);

            using(var check = new EditorGUI.ChangeCheckScope()) {

                if(_foldout) {

                    CreateCachedEditor(_settings, null, ref _editor);
                    _editor.OnInspectorGUI();

                    if(check.changed) {
                        if(_onSettingsUpdated != null) {
                            _onSettingsUpdated();
                        }
                    }

                }

            }

        }

    }

    private void OnEnable() {
        planet = (Planet)target;
    }
}
