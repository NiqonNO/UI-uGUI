using System;
using UnityEngine;

namespace NiqonNO.UGUI
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class NOMVVMBindAttribute : PropertyAttribute
    {
        public string Key;

        public NOMVVMBindAttribute(string key = null)
        {
            Key = key;
        }
    }
}