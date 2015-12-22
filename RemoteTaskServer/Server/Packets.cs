﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using RemoteTaskServer.Utilities;
using RemoteTaskServer.WebSocketAPI;

namespace RemoteTaskServer.Server

{
    [Serializable]
    public class Packets
    {
        public string command;
        public string action;
        public PacketType packetType;
        public string paramaters;
        public Uri query;
        public string apiKey;
        public string senderID;
        private Settings settings = new Settings();
        
        public Packets(byte[] packetBytes, int readBytes)
        {
            
            var decodedQuery = WebSocketFunctions.DecodeMessage(packetBytes, readBytes);
            Uri myUri = null;
            try
            {
                myUri = new Uri(decodedQuery);
            }
            catch (Exception)
            {
                return;
            }
            query = myUri;
            command = myUri.Host;
            action = Tools.GetQueryString(decodedQuery, "command");
            apiKey = Tools.GetQueryString(decodedQuery, "oauth");
            Console.WriteLine(apiKey);
            senderID = "server";
            var key = settings.Read("ApiKey", "TaskServer");
            if (!String.IsNullOrEmpty(key))
            {
                if (key.Equals(apiKey))
                {
                    switch (command)
                    {
                        case "requestprocessinformation":
                            Console.WriteLine("Request Process Information");
                            packetType = PacketType.RequestProcess;
                            break;
                        case "requestcpuinformation":
                            Console.WriteLine("Request CPU Information");
                            packetType = PacketType.RequestCpuInformation;
                            break;
                        case "requestosinformation":
                            Console.WriteLine("Request OS Information");
                            packetType = PacketType.RequestOsInformation;
                            break;
                        case "requestnetworkinformation":
                            Console.WriteLine("Request Network Information");
                            packetType = PacketType.RequestNetworkInformation;
                            break;
                        case "requestsysteminformation":
                            Console.WriteLine("Request System Information");
                            packetType = PacketType.RequestSystemInformation;
                            break;
                        case "startprocess":
                            Console.WriteLine("Starting Process " + action);
                            packetType = PacketType.StartProcess;
                            break;
                        case "killprocess":
                            Console.WriteLine("Killing Process " + action);
                            packetType = PacketType.KillProcess;
                            break;
                        default:
                            packetType = PacketType.InvalidPacket;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid OAuth Key" + key);
                    packetType = PacketType.InvalidApiKey;
                }
            }
            else
            {
                Console.WriteLine("Please Generate a OAuth Key.");
                packetType = PacketType.EmptyApiKey;
            }
        }
    }

    public enum PacketType
    {
        RequestProcess,
        RequestCpuInformation,
        RequestOsInformation,
        RequestNetworkInformation,
        RequestSystemInformation,
        StartProcess,
        KillProcess,
        EmptyApiKey,
        InvalidApiKey,
        InvalidPacket
    }
}