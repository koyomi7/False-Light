using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ReverseAnimation : Editor
{
    public static AnimationClip GetSelectedClip()
    {
        var clips = Selection.GetFiltered(typeof(AnimationClip),SelectionMode.Assets);
        if (clips.Length > 0)
        {
            return clips[0] as AnimationClip;
        }
        return null;
    }

    [MenuItem("Tools/ReverseAnimation")]
    public static void Reverse()
    {
        var clip = GetSelectedClip();
        if (clip == null)
            return;
        float clipLength = clip.length;

        List<AnimationCurve> curves = new List<AnimationCurve>();
        EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(clip);
        foreach (EditorCurveBinding i in editorCurveBindings)
        {
            var curve = AnimationUtility.GetEditorCurve(clip, i);
            curves.Add(curve);
        }

        clip.ClearCurves();
        for (int i = 0; i < curves.Count; i++)
        {
            var curve = curves[i];
            var binding = editorCurveBindings[i];

            var keys = curve.keys;
            int keyCount = keys.Length;
            var postWrapmode = curve.postWrapMode;
            curve.postWrapMode = curve.preWrapMode;
            curve.preWrapMode = postWrapmode;
            for (int j = 0; j < keyCount; j++ )
            {
                Keyframe K = keys[j];
                K.time = clipLength - K.time;
                var tmp = -K.inTangent;
                K.inTangent = -K.outTangent;
                K.outTangent = tmp;
                keys[j] = K;
            }
            curve.keys = keys;
            clip.SetCurve(binding.path, binding.type, binding.propertyName, curve);
        }

        var events = AnimationUtility.GetAnimationEvents(clip);
        if (events.Length > 0)
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i].time = clipLength - events[i].time;
            }
            AnimationUtility.SetAnimationEvents(clip,events);
        }
        Debug.Log("Animation reversed!");
    }
}