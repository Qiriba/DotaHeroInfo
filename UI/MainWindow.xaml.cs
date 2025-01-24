using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
namespace HeroDisplay
{
    public partial class MainWindow : Window
    {
        private HashSet<int> selectedHeroes = new(); // Store selected hero IDs
        private List<bool> cores = new List<bool>(128);
        private List<Hero> allHeroes = new(); // Store all heroes
        private List<Hero> filteredHeroes = new(); // Store filtered heroes

        public MainWindow()
        {
            InitializeComponent();
            LoadHeroData();
        }

        private static void OnGameEvent(DotaGameEvent game_event)
        {
            if (game_event is ProviderUpdated provider)
            {
                Console.WriteLine($"Current Game version: {provider.New.Version}");
                Console.WriteLine($"Current Game time stamp: {provider.New.TimeStamp}");
            }
            else if (game_event is PlayerDetailsChanged player_details)
            {
                Console.WriteLine($"Player Name: {player_details.New.Name}");
                Console.WriteLine($"Player Account ID: {player_details.New.AccountID}");
            }
            else if (game_event is HeroDetailsChanged hero_details)
            {
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero ID: " + hero_details.New.ID);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero XP: " + hero_details.New.Experience);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero has Aghanims Shard upgrade: " + hero_details.New.HasAghanimsShardUpgrade);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero Health: " + hero_details.New.Health);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero Mana: " + hero_details.New.Mana);
                Console.WriteLine($"Player {hero_details.Player.Details.Name} Hero Location: " + hero_details.New.Location);
            }
            else if (game_event is AbilityUpdated ability)
            {
                Console.WriteLine($"Player {ability.Player.Details.Name} updated their ability: " + ability.New);
            }
            else if (game_event is TowerUpdated tower_updated)
            {
                if (tower_updated.New.Health < tower_updated.Previous.Health)
                {
                    Console.WriteLine($"{tower_updated.Team} {tower_updated.Location} tower is under attack! Health: " + tower_updated.New.Health);
                }
                else if (tower_updated.New.Health > tower_updated.Previous.Health)
                {
                    Console.WriteLine($"{tower_updated.Team} {tower_updated.Location} tower is being healed! Health: " + tower_updated.New.Health);
                }
            }
            else if (game_event is TowerDestroyed tower_destroyed)
            {
                Console.WriteLine($"{tower_destroyed.Team} {tower_destroyed.Location} tower is destroyed!");
            }
            else if (game_event is RacksUpdated racks_updated)
            {
                if (racks_updated.New.Health < racks_updated.Previous.Health)
                {
                    Console.WriteLine($"{racks_updated.Team} {racks_updated.Location} {racks_updated.RacksType} racks are under attack! Health: " + racks_updated.New.Health);
                }
                else if (racks_updated.New.Health > racks_updated.Previous.Health)
                {
                    Console.WriteLine($"{racks_updated.Team} {racks_updated.Location} {racks_updated.RacksType} tower are being healed! Health: " + racks_updated.New.Health);
                }
            }
            else if (game_event is RacksDestroyed racks_destroyed)
            {
                Console.WriteLine($"{racks_destroyed.Team} {racks_destroyed.Location} {racks_destroyed.RacksType} racks is destroyed!");
            }
            else if (game_event is AncientUpdated ancient_updated)
            {
                if (ancient_updated.New.Health < ancient_updated.Previous.Health)
                {
                    Console.WriteLine($"{ancient_updated.Team} ancient is under attack! Health: " + ancient_updated.New.Health);
                }
                else if (ancient_updated.New.Health > ancient_updated.Previous.Health)
                {
                    Console.WriteLine($"{ancient_updated.Team} ancient is being healed! Health: " + ancient_updated.New.Health);
                }
            }
            else if (game_event is TeamNeutralItemsUpdated team_neutral_items_updated)
            {
                Console.WriteLine($"{team_neutral_items_updated.Team} neutral items updated: {team_neutral_items_updated.New}");
            }
            else if (game_event is CourierUpdated courier_updated)
            {
                Console.WriteLine($"Player {courier_updated.Player.Details.Name} courier updated: {courier_updated.New}");
            }
            else if (game_event is TeamDraftDetailsUpdated draft_details_updated)
            {
                Console.WriteLine($"{draft_details_updated.Team} draft details updated: {draft_details_updated.New}");
            }
            else if (game_event is TeamDefeat team_defeat)
            {
                Console.WriteLine($"{team_defeat.Team} lost the game.");
            }
            else if (game_event is TeamVictory team_victory)
            {
                Console.WriteLine($"{team_victory.Team} won the game!");
            }
        }
        private void LoadHeroData()
        {
            string jsonFilePath = "C:\\Users\\Tobias\\source\\repos\\DotaHeroInfo\\DotaHeroInfo\\bin\\Debug\\net8.0\\HeroData.json";

            if (!File.Exists(jsonFilePath))
            {
                MessageBox.Show("HeroData.json file not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                JObject heroData = JObject.Parse(jsonContent);

                foreach (var heroEntry in heroData)
                {
                    var hero = heroEntry.Value["result"]?["data"]?["heroes"]?.FirstOrDefault();
                    if (hero != null)
                    {
                        int heroId = hero["id"]?.Value<int>() ?? 0;
                        string heroName = hero["name_loc"]?.ToString() ?? "Unknown Hero";

                        var newHero = new Hero { Id = heroId, Name = heroName };
                        allHeroes.Add(newHero);
                        filteredHeroes.Add(newHero); // Initially show all heroes

                        var heroCard = CreateHeroCard(heroId, heroName);
                        HeroPanel.Children.Add(heroCard);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading hero data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateHeroCard(int heroId, string heroName)
        {
            for (int i = 0; i < 128; i++)
            {
                cores.Add(false);
            }
            // Create a selectable hero card
            var border = new Border
            {
                Width = 150,
                Height = 100,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Tag = heroId // Store hero ID for selection
            };

            // Create a Grid to hold the TextBlock and CheckBox
            var grid = new Grid();

            // Add two rows to the Grid: one for the CheckBox, one for the TextBlock
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // For CheckBox
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // For TextBlock

            // Create and configure the CheckBox
            var checkBox = new CheckBox
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5)
            };
            checkBox.Visibility = Visibility.Collapsed; // Initially hidden

            checkBox.Checked += (s, e) =>
            {
                if (checkBox.IsChecked == true)
                {
                    var list = selectedHeroes.ToList();
                    cores[list.IndexOf(heroId)] = true;
                }
                else
                {
                    var list = selectedHeroes.ToList();
                    cores[list.IndexOf(heroId)] = false;
                }
            };

            // Create and configure the TextBlock
            var textBlock = new TextBlock
            {
                Text = heroName,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };

            // Place the CheckBox and TextBlock in the Grid
            Grid.SetRow(checkBox, 0); // Place CheckBox in the first row
            Grid.SetRow(textBlock, 1); // Place TextBlock in the second row

            grid.Children.Add(checkBox);
            grid.Children.Add(textBlock);

            // Add the Grid to the Border
            border.Child = grid;

            // Add click event for selection
            border.MouseDown += (s, e) =>
            {
                if (selectedHeroes.Contains(heroId))
                {
                    selectedHeroes.Remove(heroId);
                    border.Background = new SolidColorBrush(Colors.White);
                    checkBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    selectedHeroes.Add(heroId);
                    border.Background = new SolidColorBrush(Colors.LightBlue);
                    checkBox.Visibility = Visibility.Visible;
                }
            };

            // Reapply the selection highlight if this hero is selected
            if (selectedHeroes.Contains(heroId))
            {
                border.Background = new SolidColorBrush(Colors.LightBlue);
                checkBox.Visibility = Visibility.Visible;
            }

            return border;
        }

        // Handle the OK button click to go to the next page
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHeroes.Count == 0)
            {
                MessageBox.Show("Please select at least one hero.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Navigate to new page
            var detailsPage = new HeroDetailsWindow(selectedHeroes.ToList(), cores);
            detailsPage.Show();
            this.Close();
        }

        // Handle search box text change to filter heroes
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            // Filter the heroes based on the search text
            filteredHeroes = string.IsNullOrWhiteSpace(searchText)
                ? new List<Hero>(allHeroes) // Show all heroes if search is empty
                : allHeroes.Where(hero => hero.Name.ToLower().Contains(searchText)).ToList();

            // Clear current cards and display filtered heroes
            HeroPanel.Children.Clear();
            foreach (var hero in filteredHeroes)
            {
                var heroCard = CreateHeroCard(hero.Id, hero.Name);
                HeroPanel.Children.Add(heroCard);
            }
        }
    }

    // Hero model (simplified)
    public class Hero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // You can add more properties here like abilities, health, etc.
    }
}
