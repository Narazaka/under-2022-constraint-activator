using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Narazaka.Unity.Under2022ConstraintActivator
{
#if !UNITY_2022_1_OR_NEWER
    public class Under2022ConstraintActivator : AssetPostprocessor
    {
        static HashSet<string> _constraintMark = new HashSet<string>
        {
            "RotationConstraint:",
            "PositionConstraint:",
            "ScaleConstraint:",
            "AimConstraint:",
            "LookAtConstraint:",
            "ParentConstraint:",
        };
        static Regex _activeProp = new Regex(@"m_Active:\s*1");
        static Regex _isConstraintActiveProp = new Regex(@"m_IsContraintActive:\s*1");

        private void OnPostprocessPrefab(GameObject root)
        {
            var lines = File.ReadAllLines(assetPath);
            var outLines = new List<string>();
            var modified = false;
            var isConstraintRegion = false;
            string activeLine = "";
            var hasIsConstraintActive = false;
            foreach (var line in lines)
            {
                if (_constraintMark.Contains(line))
                {
                    isConstraintRegion = true;
                    activeLine = string.Empty;
                    hasIsConstraintActive = false;
                }
                else if (isConstraintRegion)
                {
                    if (line.StartsWith("---"))
                    {
                        isConstraintRegion = false;

                        if (activeLine != string.Empty && !hasIsConstraintActive)
                        {
                            var index = activeLine.IndexOf("m_Active:");
                            var newLine = activeLine.Substring(0, index) + "m_IsContraintActive:" + activeLine.Substring(index + 9);
                            outLines.Add(newLine);
                            modified = true;
                        }
                    }
                    else if (_activeProp.IsMatch(line))
                    {
                        activeLine = line;
                    }
                    else if (_isConstraintActiveProp.IsMatch(line))
                    {
                        hasIsConstraintActive = true;
                    }
                }
                outLines.Add(line);
            }
            if (modified)
            {
                File.WriteAllLines(assetPath, outLines);
                Debug.Log($"Constraint Activated {assetPath}");
            }
        }
    }
#endif
}
