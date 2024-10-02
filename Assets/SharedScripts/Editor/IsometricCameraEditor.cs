using UnityEditor;
using UnityEngine;

namespace GameMath.Cameras.Editor
{
    [CustomEditor(typeof(IsometricCamera))]
    public class IsometricCameraEditor : UnityEditor.Editor
    {
        private IsometricCamera isometricCamera;
        
        private SerializedProperty anchorProperty;
        private SerializedProperty updateMethodProperty;
        private SerializedProperty panningSpeedProperty;
        private SerializedProperty initialAngleProperty;
        private SerializedProperty rotateSmoothTimeProperty;
        private SerializedProperty rotateMotionProperty;
        private SerializedProperty zoomMinDistanceProperty;
        private SerializedProperty zoomMaxDistanceProperty;
        private SerializedProperty zoomDiscreteDistanceProperty;
        private SerializedProperty zoomSensitiveProperty;
        private SerializedProperty zoomMotionProperty;
        private SerializedProperty zoomSmoothTimeProperty;

        private void OnEnable()
        {
            isometricCamera = (IsometricCamera)target;
            
            anchorProperty = serializedObject.FindProperty("anchor");
            panningSpeedProperty = serializedObject.FindProperty("panningSpeed");
            
            updateMethodProperty = serializedObject.FindProperty("updateMethod");
            
            rotateMotionProperty = serializedObject.FindProperty("rotateMotion");
            initialAngleProperty = serializedObject.FindProperty("initialAngle");
            rotateSmoothTimeProperty = serializedObject.FindProperty("rotateSmoothTime");
            
            zoomMinDistanceProperty = serializedObject.FindProperty("zoomMinDistance");
            zoomMaxDistanceProperty = serializedObject.FindProperty("zoomMaxDistance");
            zoomDiscreteDistanceProperty = serializedObject.FindProperty("zoomDiscreteDistance");
            zoomSensitiveProperty = serializedObject.FindProperty("zoomSensitive");
            zoomMotionProperty = serializedObject.FindProperty("zoomMotion");
            zoomSmoothTimeProperty = serializedObject.FindProperty("zoomSmoothTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Motion", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(anchorProperty);
            EditorGUILayout.PropertyField(panningSpeedProperty);
            
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(initialAngleProperty);
            EditorGUILayout.PropertyField(rotateMotionProperty);

            if (isometricCamera.RotateMotion == IsometricCamera.CameraMotion.Smooth)
                EditorGUILayout.PropertyField(rotateSmoothTimeProperty);
            
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Zoom", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(zoomMinDistanceProperty);
            EditorGUILayout.PropertyField(zoomMaxDistanceProperty);
            EditorGUILayout.PropertyField(zoomMotionProperty);
            
            if (isometricCamera.ZoomMotion == IsometricCamera.CameraMotion.Smooth)
            {
                EditorGUILayout.PropertyField(zoomSensitiveProperty);
                EditorGUILayout.PropertyField(zoomSmoothTimeProperty);
            }
            else
            {
                EditorGUILayout.PropertyField(zoomDiscreteDistanceProperty);
            }

            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Update", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(updateMethodProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}