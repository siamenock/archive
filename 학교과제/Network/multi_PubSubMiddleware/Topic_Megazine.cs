using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;




// publishable topic
public class Topic
{
    public string name      { get; private set; }
    public string ipHost    { get; private set; }
    public int portNum      { get; private set; }
    public int period       { get; private set; }
    public bool readPast    { get; private set; }

    private Topic() { }
    public Topic(string name, string ipHost, int portNum, int period = 0, bool readPastLogsWhenSubscribe = false)
    {
        this.name = name;        // 이거 복사문제 있을것같은데
        this.ipHost = ipHost;
        this.portNum = portNum;
        this.period = period;
        this.readPast = readPastLogsWhenSubscribe;
    }
}

public class Megazine
{
    public string name { get; private set; }  // == topic.name
    public string ipHost { get; private set; }  // == topic.ipHost
    public int portNum { get; private set; }  // == topic.portNum
    public string timeLog { get; private set; }
    public string data { get; private set; }


    private Megazine() { }
    public Megazine(string name, string ipHost, int portNum, string data, string timeLog = "default: curTime")
    {
        bool timeLogDefault = string.Compare(timeLog, "") == 0;
        this.name = name;
        this.ipHost = ipHost;
        this.portNum = portNum;
        this.data = data;
        this.timeLog = timeLog == "default: curTime" ? CurDateTime() : timeLog;
    }

    public static string CurDateTime()
    {
        DateTime localDate = DateTime.Now;
        var culture = new CultureInfo("en-KR");
        return localDate.ToString(culture);
    }
}





