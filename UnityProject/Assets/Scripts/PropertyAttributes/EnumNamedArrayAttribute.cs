using Core.Providers;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class EnumNamedArrayAttribute : PropertyAttribute
{
    public string[] names;
    public bool handleFirstAsNone;

    public EnumNamedArrayAttribute(System.Type names_enum_type,bool handleFirstAsNone = true)
    {
        this.names = System.Enum.GetNames(names_enum_type);
        this.handleFirstAsNone = handleFirstAsNone;
    }
}