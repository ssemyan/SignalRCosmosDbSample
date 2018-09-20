# Sample App: Signal R with CosmosDB Integration via Managed Service Identity 

This is a simple app to show how to use Signal R and CosmosDB. The Signal R piece allows users to broadcast messages to all other users, and to simulate an long running operation that the originator needs to be notified about.
An example is an order mechanism where the user places an order and then is notified later when the order is accepted. When a user submits an order by clicking the top button, the 
order is written to CosmosDB and then 5 seconds later, the submitter will be notified via Signal R that the order has been processed. 

This sample can be run locally by using the CosmosDB emulator: (https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)

The code will create a new database and collection using the values from the web.config file. The URI and auth key for the CosmosDB can be stored in the web.config, or better, 
in KeyVault. If using Managed Service Identity in Azure (determined by the existance of an environment variable 'MSI_ENDPOINT'), then it will be assumed the key is located in KeyVault 
and MSI will be used to access the secret.

To deploy to Azure, create a CosmosDB instance in SQL mode and an Azure Web App. Then, either inject the CosmosDB URI and auth key into the web.config during the deploy process, 
or add the app settings to the Web App directly, or enable Managed Service Identity, add the setting to KeyVault, give the Web App MSI access to GET the secrets, and store the
ID of the secret in an app setting. 

