using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainEditor
{
    public static class Tools
    {
        public static float FractalBrowningMotion(float x, float y, int oct, float persistence)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            for (int i = 0; i < oct; i++)
            {
                total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }

        public static float Map(float value, float originalMin, float originalMax, float targetMin, float targetMax)
        {
            return (value - originalMin) * (targetMax - targetMin) / (originalMax - originalMin) + targetMin;
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            var r = new System.Random();
            while (n > 1)
            {
                n--;
                var k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
