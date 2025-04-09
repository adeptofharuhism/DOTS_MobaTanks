using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Utility.Extensions
{
    public static class DataExtensions
    {
        public static float4 AsFloat4(this Color color) =>
            new(color.r, color.g, color.b, color.a);
    }
}