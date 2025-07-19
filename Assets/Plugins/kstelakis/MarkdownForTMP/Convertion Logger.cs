using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class ConvertionLogger : MonoBehaviour
{
    public enum LogType
    {
        Replacement,
        Addition,
        Appendment,
        Info,
        Parse
    }

    [SerializeField] private List<LogType> logsToBeShown;
    void Awake()
    {
        // If no options log all
        logsToBeShown ??= new List<LogType>
        {
            LogType.Replacement,
            LogType.Addition,
            LogType.Appendment,
            LogType.Info,
            LogType.Parse
        };

        logsToBeShown = new List<LogType>(new HashSet<LogType>(logsToBeShown)); // Remove duplicates
    }

    public void Log(LogType type, string message)
    {
        if (logsToBeShown.Contains(type))
        {
            string visible = string.Concat(message.Select(c =>
                c switch
                {
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    ' ' => "·", // Optional: visualize spaces as ·
                    _ => c.ToString()
                }));


            Debug.Log($"[{type}] {message}");
        }
    }

}
