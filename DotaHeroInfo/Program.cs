using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

class Program
{

    public enum DamageType
    {
        None = 0,      // No damage
        Magical = 1,   // Magical damage
        Physical = 2,  // Physical damage
        Pure = 4       // Pure damage
    }

    static void Main(string[] args)
    {
        Console.WriteLine("tasdasd");

        string filePath = "HeroData.json";

        // Load hero data from file
        var heroData = LoadHeroDataFromFile(filePath);

        if (heroData != null)
        {
            // Display all ability names
            DisplayAllAbilityNames(heroData);
        }
    }

    // Method to load hero data from a JSON file
    static Dictionary<int, JObject> LoadHeroDataFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);

                // Parse the JSON content to a dictionary of JObject
                JObject parsedData = JObject.Parse(jsonContent);
                var heroDataDictionary = new Dictionary<int, JObject>();

                foreach (var item in parsedData)
                {
                    int heroId = int.Parse(item.Key);
                    JObject heroObject = (JObject)item.Value;
                    heroDataDictionary[heroId] = heroObject;
                }

                Console.WriteLine($"Successfully loaded hero data from {filePath}");
                return heroDataDictionary;
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading hero data: {ex.Message}");
        }

        return null;
    }

    // Method to display all ability names for all heroes
    static void DisplayAllAbilityNames(Dictionary<int, JObject> heroData)
    {
        foreach (var heroEntry in heroData)
        {
            int heroId = heroEntry.Key;
            JObject heroObject = heroEntry.Value;

            // Navigate to the "abilities" array in the JSON structure
            var abilities = heroObject["result"]?["data"]?["heroes"]?[0]?["abilities"] as JArray;

            if (abilities != null)
            {
                Console.WriteLine($"Hero ID: {heroId}");

                foreach (var ability in abilities)
                {
                    string abilityName = ability["name_loc"]?.ToString() ?? "Unknown";
                    string abilityDamage = ability["damage"]?.ToString() ?? "Unknown";
                    Console.WriteLine($"  Ability: {abilityName}");
                    Console.WriteLine($"  Damage: {abilityDamage}");
                }

                Console.WriteLine(new string('-', 50)); // Separator
            }
            else
            {
                Console.WriteLine($"No abilities found for Hero ID: {heroId}");
            }
        }
    }
}
