using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Configuration;
using System.Diagnostics;

namespace SignalRCosmosDbSample
{
	public class CosmosDbHelper
	{
		static readonly string cosmosDatabaseId;
		static readonly string cosmosCollectionId;
		static readonly string endpointUrl;
		static readonly string authorizationKey;

		static CosmosDbHelper()
		{
			cosmosDatabaseId = ConfigurationManager.AppSettings["CosmosDatabaseId"];
			cosmosCollectionId = ConfigurationManager.AppSettings["CosmosCollectionId"];
			endpointUrl = ConfigurationManager.AppSettings["CosmosEndpointUrl"];

			// Set the auth key from the app setting, but it may be overridden if we are using MSI in Azure
			authorizationKey = ConfigurationManager.AppSettings["CosmosAuthorizationKey"];

			// If the MSI_ENDPOINT environmental variable is set, we are running in Azure with a Managed Service Identity
			// Learn more about this here: https://docs.microsoft.com/en-us/azure/app-service/app-service-managed-service-identity

			if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MSI_ENDPOINT")))
			{
				// Use settings from MSI - first get a trusted KeyVault Client
				var azureServiceTokenProvider = new AzureServiceTokenProvider();
				var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

				// Now use the client to get the auth key secret from KeyVault. 
				// The identifier for the key is in an app variable called AuthorizationKeySecretIdentifier
				// TODO: handle errors if the web app cannot connect to the keyvault or get the secret, cache and handle if expired 
				string authorizationKeySecretIdentifier = ConfigurationManager.AppSettings["AuthorizationKeySecretIdentifier"];
				var secret = keyVaultClient.GetSecretAsync(authorizationKeySecretIdentifier).Result;
				authorizationKey = secret.Value;
			}
		}

		public static void Init()
		{
			using (DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
			{
				Trace.TraceInformation("Creating DB if not exists.");
				var db = client.CreateDatabaseIfNotExistsAsync(new Database { Id = cosmosDatabaseId }).Result;

				Trace.TraceInformation("Creating Collection if not exists.");
				var coll = client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(cosmosDatabaseId), 
					new DocumentCollection { Id = cosmosCollectionId },
					new RequestOptions { OfferThroughput = 400 }).Result;
			}
		}

		public static void AddOrderData(int orderId, string userId)
		{
			using (DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
			{
				Trace.TraceInformation("Adding orderid " + orderId + " for userid " + userId);
				 Document doc = client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(cosmosDatabaseId, cosmosCollectionId), 
					 new { OrderId = orderId, UserId = userId } ).Result;
			}
		}
	}
}