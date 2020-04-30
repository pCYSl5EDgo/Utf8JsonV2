// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;

public class CsprojFileEdit : AssetPostprocessor
{
    // ReSharper disable once UnusedMember.Local
    private static string OnGeneratedCSProject(string path, string content)
    {
        var document = XDocument.Parse(content);
        document
            .Descendants()
            .Where(x => x.Name.LocalName == "Reference")
            .Where(x => ((string)x.Attribute("Include")).Equals("Boo.Lang"))
            .Remove();
        foreach (var xElement in document
            .Descendants()
            .Where(x => x.Name.LocalName == "Reference")
            .Where(x =>
            {
                var attribute = (string)x.Attribute("Include");
                switch (attribute)
                {
                    case "System.Reflection.Emit":
                        return x.Value.EndsWith("Assets/Plugins/System.Reflection.Emit.dll");
                    case "System.Reflection.Emit.ILGeneration":
                        return x.Value.EndsWith("Assets/Plugins/System.Reflection.Emit.ILGeneration.dll");
                    case "System.Reflection.Emit.Lightweight":
                        return x.Value.EndsWith("Assets/Plugins/System.Reflection.Emit.Lightweight.dll");
                    default: return false;
                }
            }))
        {
            xElement.SetAttributeValue("Condition", "$(DefineConstants.Contains('NET_STANDARD_2_0'))");
        }
        return document.Declaration + Environment.NewLine + document.Root;
    }
}
#endif
