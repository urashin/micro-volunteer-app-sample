using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


/// <summary>
/// Utility
/// </summary>
///

public class Utility : MonoBehaviour
{
    public class ParsedDeepLinkDto
    {
        public string Url { get; set; }
        public string Endpoint { get; set; }
        public Dictionary<string, string> QueryDictionary { get; set; }
    }

    static public ParsedDeepLinkDto ParseDeepLink(string url)
    {
        /**
         * http://example.com/command?k1=v1&k2=v2
         *                    ^^^^^^^ ^^^^^^^^^^^
         *                     |        |
         *           response.Endpoint  |
         *           response.QueryDictionary
         * 
         */
        var endpoint = url.Split('/').Last();
        var endpoint2 = endpoint.Split('?');
        var command = endpoint2[0];
        var queryStr = endpoint2[1];
        var query = new Dictionary<string, string>();
        foreach (var keyVal in queryStr.Split('&'))
        {
            var kv = keyVal.Split('=');
            query.Add(kv[0], kv[1]);
        }
        var response = new ParsedDeepLinkDto();
        response.Url = url;
        response.Endpoint = command;
        response.QueryDictionary = query;
        return response;
    }
}
