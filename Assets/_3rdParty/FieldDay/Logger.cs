﻿using System.Collections.Generic;
using UnityEngine;

namespace FieldDay
{
    public class Logger : MonoBehaviour
    {
        private void Awake()
        {
            SimpleLog slog = new SimpleLog("AQUALAB", 0);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["test"] = "test";

            LogEvent le = new LogEvent(data);

            slog.Log(le);
        }
    }

    public class LogEvent : ILogEvent
    {
        private Dictionary<string, string> data;

        public LogEvent(Dictionary<string, string> data)
        {
            this.data = data;
        }

        public Dictionary<string, string> Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
