using SkillsboxAssessment;
using Microsoft.Azure.Cosmos;
using System.Configuration;
using System.Net;
using Lunar.Domain;
using Lunar.Domain.CandidateManager;
using Lunar.Domain.RegionManager;

namespace NetCore6WithProgram
{
    //internal class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Console.WriteLine("Hello, World!");
    //    }
    //}
    internal class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

    // The primary key for the Azure Cosmos account.
    private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

    // The Cosmos client instance
    private CosmosClient cosmosClient;

    // The database we will create
    private Database database;

    private Database regionDatabase;

    // The container we will create.
    private Container container;

    private Container regionContainer;

        // The name of the database and container we will create
        // You can change this for the assessment task
    private readonly string databaseId = "CandidateManager";
    private string containerId = "Candidates";


    private readonly string regionDatabaseId = "RegionManager";
    private readonly string regionContainerId = "RegionWithCountry";

    // <Main>
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Beginning operations...\n");
            Program p = new Program();
            await p.GetStartedDemoAsync();

        }
        catch (CosmosException de)
        {
            Exception baseException = de.GetBaseException();
            Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e);
        }
        finally
        {
            Console.WriteLine("End of demo, press any key to exit.");
            Console.ReadKey();
        }
    }
    // </Main>

    // <GetStartedDemoAsync>
    /// <summary>
    /// Entry point to call methods that operate on Azure Cosmos DB resources in this sample
    /// </summary>
    public async Task GetStartedDemoAsync()
    {
        // Create a new instance of the Cosmos Client
        this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
        await this.CreateDatabaseAsync();
        await this.CreateContainerAsync();
        await this.AddItemsToContainerAsync();
        }
    // </GetStartedDemoAsync>

    // <CreateDatabaseAsync>
    /// <summary>
    /// Create the database if it does not exist
    /// </summary>
    private async Task CreateDatabaseAsync()
    {
        // Create a new database
        this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        Console.WriteLine("Created Database: {0}\n", this.database.Id);

        this.regionDatabase = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(regionDatabaseId);
        Console.WriteLine("Created Database: {0}\n", this.regionDatabase.Id);
    }
    // </CreateDatabaseAsync>

    // <CreateContainerAsync>
    /// <summary>
    /// Create the container if it does not exist. 
    /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
    /// </summary>
    /// <returns></returns>
    private async Task CreateContainerAsync()
    {
        // Create a new container
        this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Last", 400);
        Console.WriteLine("Created Container: {0}\n", this.container.Id);

            this.regionContainer = await this.regionDatabase.CreateContainerIfNotExistsAsync(regionContainerId, "/Name", 400);
            Console.WriteLine("Created Container: {0}\n", this.regionContainer.Id);
    }
    // </CreateContainerAsync>

    // <AddItemsToContainerAsync>
    /// <summary>
    /// Add Candidate and Region items to the respective containers
    /// </summary>
    private async Task AddItemsToContainerAsync()
    {
            // Create a family object for the Andersen family
        Candidate candidate = new()
        {
            Id = Guid.NewGuid().ToString(),
            Last = "Murphy",
            Name = "Antonio",
            Email = "Antonio.Murphy@gmx.com",
            Status = new Status
            {
                CountCreditsAssigned = 203,
                HasConsumedCredits = true,
                IsRegistered = true,
                HasCredit = true,
            }
        };

        try
        {
            // Read the item to see if it exists.  
            ItemResponse<Candidate> candidateResponse = await this.container.ReadItemAsync<Candidate>(candidate.Id, new PartitionKey(candidate.Last));
            Console.WriteLine("Item in database with id: {0} already exists\n", candidateResponse.Resource.Id);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<Candidate> candidateResponse = await this.container.CreateItemAsync<Candidate>(candidate, new PartitionKey(candidate.Last));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", candidateResponse.Resource.Id, candidateResponse.RequestCharge);
        }

        RegionWithCountry region = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ICDL Africa",
            Discriminator = "CatalogRegion",
            Type= "region",
            Countries = new[]
            {
                new Country
                {
                    Name = "Rwanda",
                    ShortCode = "RW",
                    Type = "country"
                },
                new Country
                {
                    Name = "Uganda",
                    ShortCode = "UG",
                    Type = "country"
                },
                new Country
                {
                    Name = "Nigeria",
                    ShortCode = "NG",
                    Type = "country"
                }
            }
        };

        try
        {
            // Read the item to see if it exists.  
            ItemResponse<RegionWithCountry> regionResponse = await this.regionContainer.ReadItemAsync<RegionWithCountry>(region.Id, new PartitionKey(region.Name));
            Console.WriteLine("Item in database with id: {0} already exists\n", regionResponse.Resource.Id);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<RegionWithCountry> regionResponse = await this.regionContainer.CreateItemAsync<RegionWithCountry>(region, new PartitionKey(region.Name));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", regionResponse.Resource.Id, regionResponse.RequestCharge);
        }

         
    }
        // </AddItemsToContainerAsync>
}
}