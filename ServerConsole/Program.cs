// See https://aka.ms/new-console-template for more information
using ClientServerLib;
using LogLib;
using System.Net;

Console.WriteLine("Server console");

ServerModule module;

module=new ServerModule(new ConsoleLogger(new DefaultLogFormatter()),IPAddress.Any,5000,10);
module.Start();

Console.ReadLine();

module.Stop();

Console.ReadLine();

