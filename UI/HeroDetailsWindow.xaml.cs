using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace HeroDisplay
{
    public partial class HeroDetailsWindow : Window
    {
        private readonly List<int> selectedHeroIds;

        // Diagnostic counters
        private int magicDamageCount = 0;
        private int physicalDamageCount = 0;
        private int pureDamageCount = 0;
        private int disableCount = 0;

        // Auto-attack counters
        private int autoAttackMagicCount = 0;
        private int autoAttackPhysicalCount = 0;
        private int autoAttackPureCount = 0;
        private List<bool> isCore;
        public HeroDetailsWindow(List<int> heroIds, List<bool> core)
        {
            InitializeComponent();
            selectedHeroIds = heroIds;
            isCore = core;
            LoadHeroDetails();
            DisplayDiagnostics(); // Display diagnostics at the bottom
        }

        private void LoadHeroDetails()
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
                int counter = 0;
                foreach (var heroId in selectedHeroIds)
                {
                    var hero = heroData[heroId.ToString()]?["result"]?["data"]?["heroes"]?.FirstOrDefault();
                    if (hero != null)
                    {
                        string heroName = hero["name_loc"]?.ToString() ?? "Unknown Hero";
                        var abilitiesToken = hero["abilities"] ?? new JArray();
                        var abilities = abilitiesToken as JArray; // Explicitly cast to JArray

                        if (abilities == null || abilities.Count == 0)
                        {
                            DetailsPanel.Children.Add(new TextBlock
                            {
                                Text = "No abilities found for this hero.",
                                Margin = new Thickness(10, 0, 0, 5)
                            });
                            continue;
                        }

                        var heroHeader = new TextBlock
                        {
                            Text = $"Hero: {heroName}",
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5)
                        };
                        DetailsPanel.Children.Add(heroHeader);

                        int attackCapability = hero["attack_capability"]?.Value<int>() ?? 1;
                        string autoAttackDamageType = attackCapability switch
                        {
                            1 => "Physical",
                            2 => "Magical",
                            4 => "Pure",
                            _ => "Unknown"
                        };

                        if (isCore[counter])
                        {
                            switch (autoAttackDamageType)
                            {
                                case "Magical":
                                    autoAttackMagicCount++;
                                    autoAttackMagicCount++;
                                    autoAttackMagicCount++;
                                    autoAttackMagicCount++;
                                    break;
                                case "Physical":
                                    autoAttackPhysicalCount++;
                                    autoAttackPhysicalCount++;
                                    autoAttackPhysicalCount++;
                                    autoAttackPhysicalCount++;
                                    break;
                                case "Pure":
                                    autoAttackPureCount++;
                                    autoAttackPureCount++;
                                    autoAttackPureCount++;
                                    autoAttackPureCount++;
                                    break;
                            }
                        }
                        // Update auto-attack counters
                        switch (autoAttackDamageType)
                        {
                            case "Magical":
                                autoAttackMagicCount++;
                                break;
                            case "Physical":
                                autoAttackPhysicalCount++;
                                break;
                            case "Pure":
                                autoAttackPureCount++;
                                break;
                        }

                        var autoAttackDamage = new TextBlock
                        {
                            Text = $"Autoattack Damage Type: {autoAttackDamageType}",
                            Margin = new Thickness(10, 0, 0, 5)
                        };
                        switch (autoAttackDamageType)
                        {
                            case "Magical":
                                autoAttackDamage.Foreground = new SolidColorBrush(Colors.Blue);
                                break;
                            case "Physical":
                                autoAttackDamage.Foreground = new SolidColorBrush(Colors.Red);
                                break;
                            case "Pure":
                                autoAttackDamage.Foreground = new SolidColorBrush(Colors.Gold);
                                break;
                        }
                        DetailsPanel.Children.Add(autoAttackDamage);

                        for (int i = 0; i < abilities.Count; i++)
                        {
                            var ability = abilities[i];
                            string abilityName = ability["name_loc"]?.ToString() ?? "Unknown Ability";
                            string damageType = ability["damage"]?.Value<int>() switch
                            {
                                0 => "None",
                                1 => "Physical",
                                2 => "Magical",
                                4 => "Pure",
                                _ => "Unknown"
                            };

                            // Determine ability tags
                            string abilityLabel;
                            if (ability["ability_is_innate"]?.Value<bool>() == true)
                            {
                                abilityLabel = "(Innate)";
                            }
                            else if (ability["ability_is_granted_by_scepter"]?.Value<bool>() == true)
                            {
                                abilityLabel = "(Granted by Scepter)";
                            }
                            else if (ability["ability_has_scepter"]?.Value<bool>() == true)
                            {
                                abilityLabel = "(Upgraded by Scepter)";
                            }
                            else if (ability["ability_is_granted_by_shard"]?.Value<bool>() == true)
                            {
                                abilityLabel = "(Granted by Shard)";
                            }
                            else if (ability["ability_has_shard"]?.Value<bool>() == true)
                            {
                                abilityLabel = "(Upgraded by Shard)";
                            }
                            else if (i == abilities.Count - 1) // Fallback for the last ability if not marked explicitly
                            {
                                abilityLabel = "(Ultimate)";
                            }
                            else
                            {
                                abilityLabel = ""; // No label
                            }

                            // Check for disables
                            bool hasDisable = AbilityHasDisable(ability);
                            string disableInfo = hasDisable ? " (Disables)" : "";


                            if (isCore[counter])
                            {
                                // Update diagnostics counters
                                switch (damageType)
                                {
                                    case "Magical":
                                        magicDamageCount++;
                                        magicDamageCount++;
                                        magicDamageCount++;
                                        magicDamageCount++;
                                        break;
                                    case "Physical":
                                        physicalDamageCount++;
                                        physicalDamageCount++;
                                        physicalDamageCount++;
                                        physicalDamageCount++;
                                        break;
                                    case "Pure":
                                        pureDamageCount++;
                                        pureDamageCount++;
                                        pureDamageCount++;
                                        pureDamageCount++;
                                        break;
                                }
                            }
                            // Update diagnostics counters
                            switch (damageType)
                            {
                                case "Magical":
                                    magicDamageCount++;
                                    break;
                                case "Physical":
                                    physicalDamageCount++;
                                    break;
                                case "Pure":
                                    pureDamageCount++;
                                    break;
                            }

                            if (hasDisable)
                            {
                                disableCount++;
                                if (isCore[counter])
                                {
                                    disableCount++; disableCount++; disableCount++; disableCount++;
                                }
                            }

                            var abilityDetails = new TextBlock
                            {
                                Text = $"{abilityName} {abilityLabel} - Damage Type: {damageType}{disableInfo}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            switch (damageType)
                            {
                                case "Magical":
                                    abilityDetails.Foreground = new SolidColorBrush(Colors.Blue);
                                    break;
                                case "Physical":
                                    abilityDetails.Foreground = new SolidColorBrush(Colors.Red);
                                    break;
                                case "Pure":
                                    abilityDetails.Foreground = new SolidColorBrush(Colors.Gold);
                                    break;
                            }
                            DetailsPanel.Children.Add(abilityDetails);
                        }
                    }

                    counter++;

                }
            }
            catch
            {
                MessageBox.Show("Failed to load hero details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayDiagnostics()
        {
            var diagnosticsHeader = new TextBlock
            {
                Text = "Diagnostics Summary",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };
            DetailsPanel.Children.Add(diagnosticsHeader);

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Magic Damage Abilities: {magicDamageCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Blue)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Physical Damage Abilities: {physicalDamageCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Red)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Pure Damage Abilities: {pureDamageCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Gold)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Disabling Abilities: {disableCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Green)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Autoattack (Physical): {autoAttackPhysicalCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Red)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Autoattack (Magical): {autoAttackMagicCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Blue)
            });

            DetailsPanel.Children.Add(new TextBlock
            {
                Text = $"Autoattack (Pure): {autoAttackPureCount}",
                Margin = new Thickness(10, 0, 0, 5),
                Foreground = new SolidColorBrush(Colors.Gold)
            });
            if(autoAttackMagicCount + magicDamageCount > autoAttackPhysicalCount + physicalDamageCount)
            {

            }



        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close the current details window
        }

        private bool AbilityHasDisable(JToken ability)
        {
            var facets = ability["facets_loc"];
            if (facets != null)
            {
                foreach (var facet in facets)
                {
                    string facetText = facet.ToString().ToLower();
                    if (facetText.Contains("slow") ||
                        facetText.Contains("stun") ||
                        facetText.Contains("silence") ||
                        facetText.Contains("reduce") ||
                        facetText.Contains("root") ||
                        facetText.Contains("disarm") ||
                        facetText.Contains("fear") ||
                        facetText.Contains("hex"))
                    {
                        return true;
                    }
                }
            }

            var specialValues = ability["special_values"];
            if (specialValues != null)
            {
                foreach (var special in specialValues)
                {
                    string name = special["name"]?.ToString() ?? "";
                    if (name.Contains("stun_duration") ||
                        name.Contains("silence_duration") ||
                        name.Contains("root_duration") ||
                        name.Contains("armor_reduction") ||
                        name.Contains("move_speed_slow") ||
                        name.Contains("attack_speed_slow") ||
                        name.Contains("healing_reduction"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
