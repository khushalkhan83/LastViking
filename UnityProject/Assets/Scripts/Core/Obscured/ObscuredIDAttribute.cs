using System;
using UnityEngine;

public class ObscuredIDAttribute : PropertyAttribute
{
    public Type Type { get; private set; }

    public ObscuredIDAttribute(Type type)
    {
        Type = type;
    }

}
