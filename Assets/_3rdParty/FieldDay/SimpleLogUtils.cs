﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FieldDay
{
    public class SimpleLogUtils
    {
        private const int ISOEncodingId = 28591;
        private static StringBuilder stringBuilder = new StringBuilder();

        public static long UUIDint()
        {
            DateTime dt = DateTime.Now;
            string id = ("" + dt.Year).Substring(2);

            if (dt.Month < 10) id += "0";
            id += dt.Month;

            if (dt.Day < 10) id += "0";
            id += dt.Day;

            if (dt.Hour < 10) id += "0";
            id += dt.Hour;

            if (dt.Minute < 10) id += "0";
            id += dt.Minute;

            if (dt.Second < 10) id += "0";
            id += dt.Second;

            System.Random rand = new System.Random();

            for (int i = 0; i < 5; ++i)
            {
                id += Math.Floor(rand.NextDouble() * 10);
            }

            return Int64.Parse(id);
        }

        public static string GetCookie(string cookie, string name)
        {
            if (cookie ==  null)
            {
                return "";
            }

            string full_cookie = Uri.EscapeDataString(cookie);

            if (full_cookie == "")
            {
                return "";
            }

            string[] cookies = full_cookie.Split(';');
            string full_name = name + "=";

            for (int i = 0; i < cookies.Length; ++i)
            {
                string c = cookies[i];

                while (c[0] == ' ')
                {
                    c = c.Substring(1);
                }

                if (c.IndexOf(full_name) == 0)
                {
                    return c.Substring(full_name.Length, c.Length);
                }
            }

            return "";
        }

        public static string SetCookie(string name, long val, int days)
        {
            DateTime newDate = DateTime.Now.AddDays(days);
            return name + "=" + val + "; expires=" + newDate + "; path=/";
        }

        public static string BuildDataString(List<ILogEvent> log)
        {
            foreach (ILogEvent logEvent in log)
            {
                foreach (KeyValuePair<string, string> kvp in logEvent.Data)
                {
                    stringBuilder.AppendFormat("{{\"{0}\":\"{1}\"}},", kvp.Key, kvp.Value);
                }
            }

            // Remove trailing comma
            stringBuilder.Length--;
            string jsonString = stringBuilder.ToString();
            stringBuilder.Clear();

            return btoa(jsonString);
        }

        // https://stackoverflow.com/questions/39111586/stringbuilder-appendformat-ienumarble
        // TODO: Wrote this function to clean up lots of duplicated lines for string building,
        // but I'm not sure if it makes things slower or not
        public static string BuildUrlString(string formatString, object[] args)
        {
            stringBuilder.AppendFormat(formatString, args);
            string urlString = stringBuilder.ToString();
            stringBuilder.Clear();
            
            return urlString;
        }

        // https://stackoverflow.com/questions/46093210/c-sharp-version-of-the-javascript-function-btoa
        public static string btoa(string str)
        {
            byte[] bytes = Encoding.GetEncoding(ISOEncodingId).GetBytes(str);
            return System.Convert.ToBase64String(bytes);
        }
    }
}
