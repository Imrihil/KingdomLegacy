// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;

Console.WriteLine("Kingdom Legacy!");
Console.WriteLine("");

const string BaseUrl = "https://www.kingdomlegacygame.com";
const string ExpansionsPath = "expansions";
const string CardsPath = "cards";
const string BaseExpansion = "FeudalKingdom";

var expansionCards = new Dictionary<string, int>
{
    { "FeudalKingdom", 140 }
};

var client = new HttpClient();
client.DefaultRequestHeaders.Add("User-Agent",
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
    "AppleWebKit/537.36 (KHTML, like Gecko) " +
    "Chrome/127.0.0.0 Safari/537.36");

client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

//var expansions = await GetExpansions();

await DownloadExpansion(BaseExpansion);

async Task<IEnumerable<string>> GetExpansions()
{
    var url = BaseUrl + "/" + ExpansionsPath + "/" + BaseExpansion;
    var html = await client.GetStringAsync(url);

    var document = new HtmlDocument();
    document.LoadHtml(html);

    var expansionsSelectWrapper = document.DocumentNode.SelectSingleNode("//*[@id=\"search\"]/div[contains(@class, 'select-expansion-wrapper')]");
    var expansionsList = expansionsSelectWrapper.SelectSingleNode("//div[contains(@class, 'select-expansion')]/ul");
    var expansions = expansionsList.SelectNodes("./li").Select(node => node.Attributes["data-url"].Value);

    Console.WriteLine("Expansions:");
    foreach (var expansion in expansions)
        Console.WriteLine($" - {expansion}");

    return expansions;
}


async Task DownloadExpansion(string expansionPath)
{
    //var html = await client.GetStringAsync(BaseUrl + "/" + ExpansionsPath + "/" + expansionPath);

    //var document = new HtmlDocument();
    //document.LoadHtml(html);

    //var cardsSelect = document.DocumentNode.SelectSingleNode("//*[@id=\"search\"]/div[contains(@class, 'select-cardnumber-wrapper')]/div[contains(@class, 'select-box')]/select");
    //var cardsIds = cardsSelect.SelectNodes("./option").Select(node => node.Attributes["value"].Value);

    //foreach (var cardId in cardsIds)
    //    await DownloadCard(expansionPath, cardId);

    var cards = expansionCards[expansionPath];
    for (var cardId = 0; cardId < cards; cardId++)
        await DownloadCard(expansionPath, cardId.ToString());

    Console.WriteLine($"Downloaded expansion {expansionPath}.");
}

async Task DownloadCard(string expansionPath, string cardId)
{
    Console.WriteLine($"Downloading card {expansionPath} #{cardId}...");

    string url = BaseUrl + "/" + CardsPath + "/" + expansionPath + "/" + cardId;
    var cardHtml = await client.GetStringAsync(url);
    var cardDocument = new HtmlDocument();
    cardDocument.LoadHtml(cardHtml);

    Directory.CreateDirectory(expansionPath);

    await DownloadImage("card-front", $"{expansionPath}/{expansionPath}_{cardId}_A", cardDocument);
    await DownloadImage("card-back", $"{expansionPath}/{expansionPath}_{cardId}_B", cardDocument);

    Console.WriteLine($"Downloaded card {expansionPath} #{cardId}.");

    await Task.Delay(Random.Shared.Next(700, 1300));
}

async Task DownloadImage(string @class, string name, HtmlDocument cardDocument)
{
    Console.WriteLine($"  Downloading image {name}...");

    var imageNode = cardDocument.DocumentNode.SelectSingleNode($"//*[@id=\"main-view\"]/div[contains(@class, 'main-section')]/div[contains(@class, 'card-view')]/div[contains(@class, 'card-full-image-view')]/div[contains(@class, '{@class}')]/img");
    var url = imageNode.Attributes["src"].Value;
    var imageBytes = await client.GetByteArrayAsync(BaseUrl + "/" + url);
    var fileExtension = Path.GetExtension(url).Split('?')[0];
    var fileName = $"{name}{fileExtension}";
    await File.WriteAllBytesAsync(fileName, imageBytes);

    Console.WriteLine($"  Saved: {fileName}.");
}