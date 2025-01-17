// See https://aka.ms/new-console-template for more information
using ClientServerLib;
using LogLib;
using System.Net;

Console.WriteLine("Client console");

ClientModule module;

module = new ClientModule(new ConsoleLogger(new DefaultLogFormatter()), IPAddress.Loopback, 5000, 10);
module.Start();

Console.ReadLine();

module.Stop();
