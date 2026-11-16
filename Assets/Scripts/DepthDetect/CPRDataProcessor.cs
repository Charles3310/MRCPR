using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CPRDataProcessor
{
    private bool isFalling = false;
    private float tempValue;
    private float tempTime;

    private readonly List<(float time, float value)> Buffer = new();
    private readonly List<(float time, float value)> Minima = new();

    [SerializeField] private float depthChangeThreshold = 0.75f;
    
    public event Action onDepthUp;
    public event Action onDepthDown;

    
    public void Reset()
    {
        Buffer.Clear();
        Minima.Clear();
        tempValue = 0;
        tempTime = 0;
        isFalling = false;
    }

    public bool Process(float time, float value)
    {
        bool mini = false;
        if(Math.Abs(tempValue-value) > depthChangeThreshold){
            Buffer.Add((time, value));
            if (value > tempValue && !isFalling)
            {
                mini = true;
                tempTime = time;
                isFalling = true; // 下壓

                onDepthDown?.Invoke();
            }
            else if (value < tempValue && isFalling)
            {
                mini = true;
                // 偵測到波谷
                Minima.Add((tempTime, tempValue));
                isFalling = false;

                onDepthUp?.Invoke();

            }
            tempValue = value;
        }
        return mini;
    }

    public (float avgSpeed, float stdSpeed, float avgDepth, float stdDepth) getAVGSpeedAndAVGDepth()
    {
        if (Minima.Count < 2)
            return (0f, 0f, 0f, 0f);

        // --- 計算速度 ---
        var intervals = Minima.Skip(1)
                              .Select((m, i) => m.time - Minima[i].time)
                              .Where(i => i >= 0.2f)
                              .ToList();

        var speeds = intervals.Select(i => 60f / i).ToList();
        float avgSpeed = speeds.Average();
        float stdSpeed = Mathf.Sqrt(speeds.Average(s => (s - avgSpeed) * (s - avgSpeed)));

        // --- 計算深度 ---
        var depths = Minima.Select(m => m.value).ToList();
        float avgDepth = depths.Average();
        float stdDepth = Mathf.Sqrt(depths.Average(d => (d - avgDepth) * (d - avgDepth)));

        return (avgSpeed, stdSpeed, avgDepth, stdDepth);
    }
    
    public float getInterval()
    {
        var count = Minima.Count;
        if(count==0)return 0.55f;
        float interval = tempTime - Minima[count - 1].time;            
        return interval;
    }

    

    
    public bool IsFalling() => isFalling;
    public IReadOnlyList<(float time, float value)> GetMinima() => Minima.AsReadOnly();
}
