using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            await Console.Out.WriteLineAsync("user " + user + "\nmessage " + message);

            

        }

        public override  Task OnConnectedAsync()
        {
            Console.WriteLine();
            Console.Out.WriteLineAsync("OnConnectedAsync await");
            //WaitForMessage(Context.ConnectionId);
            return base.OnConnectedAsync();



            
        }

        public async Task<string> WaitForMessage(string connectionId)
        {
            //var message = await Clients.Client(connectionId).InvokeCoreAsync<string>("GetMessage", new string[1]);

            var x = new CancellationTokenSource();
            var message = await Clients.Client(connectionId).InvokeAsync<string>("GetMessage", x.Token);
            //var message = await Clients.Client(connectionId).Invoke<string>("GetMessages", "kfk", "jflkj");
            //var message = await Clients.All.Invoke("GetMessage", "kfk", "jflkj");
            //=> await Clients.All.SendAsync("ReceiveMessage", user, message);
            await Console.Out.WriteLineAsync("\n wait for message message " + message);


            return connectionId;
        }


        //public async Task Re


    }
}