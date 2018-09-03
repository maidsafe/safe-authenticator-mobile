#addin nuget:?package=Cake.FileHelpers
using System.Net;
using System.Net.Sockets;

Func<IPAddress, int, string, Task> DownloadTcpTextAsync = (IPAddress TCP_LISTEN_HOST, int TCP_LISTEN_PORT, string RESULTS_PATH) => System.Threading.Tasks.Task.Run(() =>
{
    TcpListener server = null;
    try
    {
        server = new TcpListener(TCP_LISTEN_HOST, TCP_LISTEN_PORT);
        server.Start();
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            StreamReader data_in = new StreamReader(client.GetStream());
            var result = data_in.ReadToEnd();
            System.IO.File.AppendAllText(RESULTS_PATH, result);
            client.Close();
            break;
        }
    }
    catch (SocketException e)
    {
        Information("SocketException: {0}", e);
    }
    finally
    {
        server.Stop();
    }
});

void AnalyseResultFile(string FilePath)
{
    string line;
    var file = new StreamReader(FilePath);
    while ((line = file.ReadLine()) != null)
    {
        foreach (var word in line.Split(' '))
        {
            if (word == @"result=""Failed""")
            {
                throw new Exception("Tests Failed");
            }
        }
    }
    file.Close();
}
