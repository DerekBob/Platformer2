using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class ConsoleLogger : MonoBehaviour
{
    public TextMeshProUGUI messageLogText;
    public int maxLines = 100;

    private Queue<string> lines = new Queue<string>();
    private StringBuilder builder = new StringBuilder();

    public enum MessageType { Info, Warning, Error }

    public void Message(string msg, MessageType type = MessageType.Info)
    {
        string color = type switch
        {
            MessageType.Warning => "#ffcc00",
            MessageType.Error => "#ff4444",
            _ => "#ffffff"
        };

        string formatted = $"<color={color}>{msg}</color>";
        lines.Enqueue(formatted);

        if (lines.Count > maxLines)
            lines.Dequeue();

        builder.Clear();
        foreach (var line in lines)
            builder.AppendLine(line);

        messageLogText.text = builder.ToString();
    }

    public void ClearLog()
    {
        lines.Clear();
        messageLogText.text = "";
    }
}
