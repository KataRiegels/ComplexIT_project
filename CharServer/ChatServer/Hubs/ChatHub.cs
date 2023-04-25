using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatSample.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Dictionary<string, WebSocket> _clients = new Dictionary<string, WebSocket>();
        public async Task Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.

            await Clients.All.SendAsync("broadcastMessage", name, message);
            Console.WriteLine(message);
            await Console.Out.WriteLineAsync(   "ffff");


        }


        private async Task OnMessage(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {message}");
            this.Clients.Group(chatRoomName).BroadcastMessageAsync(message)
            //await BroadcastMessageAsync(webSocket, message);
        }

        // Declare a dictionary to store the connected clients

        // Handler method for WebSocket connections
        // Start a new thread to listen for incoming messages from the client



    }
}