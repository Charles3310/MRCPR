using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCharts.Runtime;

public class ShowDataChart : MonoBehaviour
{
    public Transform canvasTransform;

    public void show(List<(float time, float value)> dataList)
    {
        var chart = gameObject.GetComponent<LineChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<LineChart>();
            chart.Init();
        }

        chart.transform.SetParent(canvasTransform);
        chart.transform.localPosition = Vector3.zero;
        chart.SetSize(580, 300);

        var title = chart.EnsureChartComponent<Title>();
        title.text = "Simple Line";

        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = false;

        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.splitNumber = 10;
        xAxis.boundaryGap = true;
        xAxis.type = Axis.AxisType.Category;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;

        chart.ClearData();

        foreach (var item in dataList)
        {
            chart.AddXAxisData(item.time.ToString("F2"));
            chart.AddData(0, item.value);
        }
    }
}
