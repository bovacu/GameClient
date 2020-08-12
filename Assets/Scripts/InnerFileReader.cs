using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using UnityEngine;

public class InnerFileReader {

    private static Dictionary<string, string> properties;
    private static string _filePath;

    public static void initProperties() {

        _filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "/inGame.txt";

        if (!File.Exists(_filePath)) {
            var _file = File.Open(_filePath, FileMode.OpenOrCreate);
            _file.Close();
            writeBasicData();
        }

        properties = new Dictionary<string, string>();

        foreach (var row in File.ReadAllLines(_filePath))
            properties.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
    }

    public static Dictionary<string, string> getAllProperties() {
        return properties;
    }

    public static string getProperty(string _property) {
        return properties[_property];
    }

    public static void setProperty(string _property, string _value) {
        properties[_property] = _value;

        System.IO.StreamWriter _file = new System.IO.StreamWriter(_filePath);

        foreach (string prop in properties.Keys.ToArray())
            if (!string.IsNullOrWhiteSpace(properties[prop]))
                _file.WriteLine(prop + "=" + properties[prop]);

        _file.Close();
    }

    private static void writeBasicData() {
        StreamWriter _file = new System.IO.StreamWriter(_filePath);

        _file.WriteLine("remember=|");
        _file.WriteLine("password=|");
        _file.WriteLine("appVersion=1.0");
        _file.WriteLine("userName=|");

        _file.Close();
    }

}
