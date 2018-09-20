using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SignalRCosmosDbSample
{
	public class FanHub : Hub
	{
		public void Send(string name, string message)
		{
			string region = Environment.GetEnvironmentVariable("REGION_NAME");
			if (string.IsNullOrEmpty(region))
			{
				region = "LOCALHOST";
			}

			// Call the broadcastMessage method to update clients.
			Clients.All.broadcastMessage(name, message + " from " + region);
		}

		public void SendOrder(int orderId)
		{
			// Wait 5 sec and then tell user order is complete
			Task.Run(() => {

				// Record order in DB
				CosmosDbHelper.AddOrderData(orderId, Context.ConnectionId);

				Thread.Sleep(5000);
				Clients.Caller.broadcastMessage("System", "Order " + orderId + " is complete.");
			});
		}
	}
}