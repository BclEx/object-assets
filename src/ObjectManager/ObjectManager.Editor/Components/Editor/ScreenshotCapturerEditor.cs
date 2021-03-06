﻿using UnityEditor;
using UnityEngine;

namespace OA.Components
{
    [CustomEditor(typeof(ScreenshotCapturer))]
    public class ScreenshotCapturerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var script = (ScreenshotCapturer)target;
            if (GUILayout.Button("Capture Screenshot"))
                script.CaptureScreenshot();
        }
    }
}
