using System.Text.Json;
using System.Text;
using Option_Organizer;

class Program
{


    const string DEEPSEEK_API_URL = "http://192.168.1.117:11434/api/generate"; //"https://api.deepseek.com";
    const Boolean DEEPSEEK_CLOUD = false; // If using Cloud deepseek change this to True
    const string API_KEY = ""; // Replace with your actual API key

    static void Main(string[] args)
    {
        Console.WriteLine("Vehicle Data Predictor");
        Console.WriteLine("======================");

        // Specify the CSV file path
        string csvFilePath = "Data.csv";

        // Allow user to specify different path if needed
        if (args.Length > 0)
        {
            csvFilePath = args[0];
        }
     

        try
        {
            // Load vehicle data from CSV
            List<Vehicle> vehicles = LoadVehicleDataFromCSV(csvFilePath);

            Console.WriteLine($"\nSuccessfully loaded {vehicles.Count} vehicle entries!");
            Console.WriteLine();

            // Display statistics
            DisplayStatistics(vehicles);

            // Display first 10 entries as examples
            Console.WriteLine("\nFirst 10 Vehicle Entries:");
            Console.WriteLine("========================");
            for (int i = 0; i < Math.Min(10, vehicles.Count); i++)
            {
                Console.WriteLine($"{i + 1:D2}. {vehicles[i]}");
            }

            InteractiveMenu(vehicles);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Please make sure the CSV file exists and is in the correct format.");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    #region " CSV Stuff " 
    static List<Vehicle> LoadVehicleDataFromCSV(string filePath)
    {
        List<Vehicle> vehicles = new List<Vehicle>();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CSV file not found: {filePath}");
        }

        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length < 2)
        {
            throw new InvalidDataException("CSV file must contain at least a header and one data row.");
        }

        // Skip the header row (index 0) and process data rows
        for (int i = 1; i < lines.Length; i++)
        {
            try
            {
                Vehicle vehicle = ParseVehicleFromCSVLine(lines[i]);
                vehicles.Add(vehicle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error parsing line {i + 1}: {ex.Message}");
                Console.WriteLine($"Line content: {lines[i]}");
            }
        }

        return vehicles;
    }

    static Vehicle ParseVehicleFromCSVLine(string csvLine)
    {
        // Handle CSV parsing (including quoted fields)
        List<string> fields = ParseCSVLine(csvLine);

        if (fields.Count != 12)
        {
            throw new InvalidDataException($"Expected 12 fields, but found {fields.Count}");
        }

        return new Vehicle
        {
            Make = fields[0].Trim(),
            Model = fields[1].Trim(),
            Year = int.Parse(fields[2].Trim()),
            ZipCode = int.Parse(fields[3].Trim()),
            VehicleService = fields[4].Trim().ToUpper() == "Y",
            Gap = fields[5].Trim().ToUpper() == "Y",
            Maintenance = fields[6].Trim().ToUpper() == "Y",
            DentAndDing = fields[7].Trim().ToUpper() == "Y",
            Appearance = fields[8].Trim().ToUpper() == "Y",
            Windshield = fields[9].Trim().ToUpper() == "Y",
            KeyReplacement = fields[10].Trim().ToUpper() == "Y",
            Theft = fields[11].Trim().ToUpper() == "Y"
        };
    }

    static List<string> ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        // Add the last field
        fields.Add(currentField);

        return fields;
    }

    #endregion

    #region " Display Stuff "
    static void DisplayStatistics(List<Vehicle> vehicles)
    {
        Console.WriteLine("Dataset Statistics:");
        Console.WriteLine("==================");

        // Total count
        Console.WriteLine($"Total Vehicles: {vehicles.Count}");

        // Make distribution (top 10)
        var makeStats = vehicles.GroupBy(v => v.Make)
                              .OrderByDescending(g => g.Count())
                              .Take(10)
                              .ToList();

        Console.WriteLine("\nTop 10 Vehicle Makes:");
        foreach (var make in makeStats)
        {
            Console.WriteLine($"  {make.Key}: {make.Count()} vehicles");
        }

        // Year distribution
        var yearStats = vehicles.GroupBy(v => v.Year)
                              .OrderBy(g => g.Key)
                              .ToList();

        Console.WriteLine("\nYear Distribution:");
        foreach (var year in yearStats)
        {
            Console.WriteLine($"  {year.Key}: {year.Count()} vehicles");
        }

        // Service statistics
        Console.WriteLine("\nService Options (% with service):");
        Console.WriteLine($"  Vehicle Service: {vehicles.Count(v => v.VehicleService) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Gap: {vehicles.Count(v => v.Gap) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Maintenance: {vehicles.Count(v => v.Maintenance) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Dent & Ding: {vehicles.Count(v => v.DentAndDing) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Appearance: {vehicles.Count(v => v.Appearance) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Windshield: {vehicles.Count(v => v.Windshield) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Key Replacement: {vehicles.Count(v => v.KeyReplacement) * 100.0 / vehicles.Count:F1}%");
        Console.WriteLine($"  Theft: {vehicles.Count(v => v.Theft) * 100.0 / vehicles.Count:F1}%");
    }

    static async void InteractiveMenu(List<Vehicle> vehicles)
    {
        while (true)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Interactive Menu:");
            Console.WriteLine("1. Get Option Order For New Vehicle");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option (0-1): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateNewVehicle(vehicles);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static async Task CreateNewVehicle(List<Vehicle> vehicles)
    {
        Console.WriteLine("\nCreate New Vehicle");
        Console.WriteLine("==================");

        try
        {
            // Get Make
            Console.Write("Enter Make: ");
            string make = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(make))
            {
                Console.WriteLine("Make cannot be empty.");
                return;
            }

            // Get Model
            Console.Write("Enter Model: ");
            string model = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(model))
            {
                Console.WriteLine("Model cannot be empty.");
                return;
            }

            // Get Year
            Console.Write("Enter Year: ");
            if (!int.TryParse(Console.ReadLine(), out int year) || year < 1900 || year > DateTime.Now.Year + 2)
            {
                Console.WriteLine("Please enter a valid year.");
                return;
            }

            // Get Zip Code
            Console.Write("Enter Zip Code: ");
            if (!int.TryParse(Console.ReadLine(), out int zipCode) || zipCode < 10000 || zipCode > 99999)
            {
                Console.WriteLine("Please enter a valid 5-digit zip code.");
                return;
            }



            // Create the new vehicle
            TempVechicle newVehicle = new TempVechicle
            {
                Make = make.Trim(),
                Model = model.Trim(),
                Year = year,
                ZipCode = zipCode
            };


            // AI Stuff 
            Console.Write("\nAnalyze service purchase likelihood with AI? (Y/N): ");
            if (GetYesNoInput())
            {
                await AnalyzeServiceLikelihood(newVehicle, vehicles);
            }



        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating vehicle: {ex.Message}");
        }
    }

    static bool GetYesNoInput()
    {
        while (true)
        {
            string input = Console.ReadLine().Trim().ToUpper();
            if (input == "Y" || input == "YES")
                return true;
            if (input == "N" || input == "NO")
                return false;

            Console.Write("Please enter Y or N: ");
        }
    }

    #endregion

    #region " Deep Seek Integration "

    static async Task AnalyzeServiceLikelihood(TempVechicle newVehicle, List<Vehicle> allVehicles)
    {
        Console.WriteLine("\n🤖 Analyzing service purchase likelihood with AI...");

        try
        {

            // Call DeepSeek API
            var prediction = await CallDeepSeekAPI(newVehicle, allVehicles);

            if (prediction != null)
            {
                Console.WriteLine("\n🎯 AI Prediction Results:");
                Console.WriteLine("Service Purchase Likelihood (Most to Least Likely):");
                Console.WriteLine(new string('=', 50));

                for (int i = 0; i < prediction.ServiceRankings.Count; i++)
                {
                    var service = prediction.ServiceRankings[i];
                    var friendlyName = GetFriendlyServiceName(service.ServiceName);
                    Console.WriteLine($"{i + 1}. {friendlyName} - {service.Likelihood:F1}% likely");
                    if (!string.IsNullOrEmpty(service.Reasoning))
                    {
                        Console.WriteLine($"   Reasoning: {service.Reasoning}");
                    }
                }

                if (!string.IsNullOrEmpty(prediction.OverallInsight))
                {
                    Console.WriteLine($"\n💡 Overall Insight: {prediction.OverallInsight}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during AI analysis: {ex.Message}");
            Console.WriteLine("Note: Make sure you have configured the DeepSeek API credentials and endpoint.");

            // Provide a basic fallback analysis
            Console.WriteLine("\n📊 Fallback Analysis (Basic Statistical Analysis):");
            ProvideBasicAnalysis(newVehicle, allVehicles);
        }
    }

    static async Task<ServicePrediction> CallDeepSeekAPI(TempVechicle newVehicle, List<Vehicle> similarVehicles)
    {


        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;

            // Set up headers
            if (DEEPSEEK_CLOUD)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
            }
            httpClient.DefaultRequestHeaders.Add("User-Agent", "VehicleServiceAnalyzer/1.0");

            // Prepare the analysis prompt
            var prompt = BuildAnalysisPrompt(newVehicle, similarVehicles);
            String jsonContent = "";
            // Create the API request
            if (DEEPSEEK_CLOUD)
            {
                var requestBody = new
                {
                    model = "deepseek-r1:latest",
                    stream = false,
                    messages = new[]
                    {
                        new { role = "system", content = "You are an expert data analyst specializing in automotive service purchasing patterns. Analyze vehicle data and provide accurate predictions with reasoning." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 5000,
                    temperature = 0.3 // Lower temperature for more consistent analysis
                };
                jsonContent = JsonSerializer.Serialize(requestBody);
            }
            else
            {
                var requestBody = new
                {
                    model = "deepseek-r1:latest", // Your model name
                    prompt = prompt,
                    stream = false, // Get complete response at once
                    options = new
                    {
                        temperature = 0.3,
                        num_predict = 5000
                    }
                };
                jsonContent = JsonSerializer.Serialize(requestBody);
            }



            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Make the API call
            var response =  httpClient.PostAsync(DEEPSEEK_API_URL, content).Result ;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (DEEPSEEK_CLOUD)
                {
                    var apiResponse = JsonSerializer.Deserialize<DeepSeekResponse>(responseContent);
                    if (apiResponse?.choices?.Length > 0)
                    {
                        var aiAnalysis = apiResponse.choices[0].message.content;
                        return ParseAIResponse(aiAnalysis);
                    }
                }
                else
                {
                    var apiResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent);

                    if (!string.IsNullOrEmpty(apiResponse?.response))
                    {
                        return ParseAIResponse(apiResponse.response);
                    }
                    else
                    {
                        throw new Exception("Empty response from model");
                    }
                }

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed: {response.StatusCode} - {errorContent}");
            }
        }

        return null;
    }

    static string GetFriendlyServiceName(string serviceName)
    {
        return serviceName switch
        {
            "VehicleService" => "Vehicle Service",
            "Gap" => "Gap Insurance",
            "Maintenance" => "Maintenance Package",
            "DentDing" => "Dent & Ding Protection",
            "Appearance" => "Appearance Protection",
            "Windshield" => "Windshield Protection",
            "KeyReplacement" => "Key Replacement",
            "Theft" => "Theft Protection",
            _ => serviceName
        };
    }

    static void ProvideBasicAnalysis(TempVechicle newVehicle, List<Vehicle> similarVehicles)
    {
        var serviceStats = new Dictionary<string, double>
        {
            ["Vehicle Service"] = similarVehicles.Count(v => v.VehicleService) * 100.0 / similarVehicles.Count,
            ["Gap Insurance"] = similarVehicles.Count(v => v.Gap) * 100.0 / similarVehicles.Count,
            ["Maintenance Package"] = similarVehicles.Count(v => v.Maintenance) * 100.0 / similarVehicles.Count,
            ["Dent & Ding Protection"] = similarVehicles.Count(v => v.DentAndDing) * 100.0 / similarVehicles.Count,
            ["Appearance Protection"] = similarVehicles.Count(v => v.Appearance) * 100.0 / similarVehicles.Count,
            ["Windshield Protection"] = similarVehicles.Count(v => v.Windshield) * 100.0 / similarVehicles.Count,
            ["Key Replacement"] = similarVehicles.Count(v => v.KeyReplacement) * 100.0 / similarVehicles.Count,
            ["Theft Protection"] = similarVehicles.Count(v => v.Theft) * 100.0 / similarVehicles.Count
        };

        var sortedServices = serviceStats.OrderByDescending(s => s.Value).ToList();

        Console.WriteLine("Service Purchase Likelihood (Based on Similar Vehicles):");
        Console.WriteLine(new string('=', 50));

        for (int i = 0; i < sortedServices.Count; i++)
        {
            var service = sortedServices[i];
            Console.WriteLine($"{i + 1}. {service.Key} - {service.Value:F1}% of similar vehicles");
        }

        Console.WriteLine($"\n💡 Analysis based on {similarVehicles.Count} vehicles with similar characteristics");
        Console.WriteLine("   (Same make or within 2 years of the vehicle year)");
    }

    static string BuildAnalysisPrompt(TempVechicle newVehicle, List<Vehicle> similarVehicles)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("Analyze the following vehicle service data to predict the likelihood of purchasing each service option for a new vehicle.");
        prompt.AppendLine();

        prompt.AppendLine("NEW VEHICLE TO ANALYZE:");
        prompt.AppendLine($"Make: {newVehicle.Make}");
        prompt.AppendLine($"Model: {newVehicle.Model}");
        prompt.AppendLine($"Year: {newVehicle.Year}");
        prompt.AppendLine($"Zip Code: {newVehicle.ZipCode}");
        prompt.AppendLine();

        prompt.AppendLine("HISTORICAL DATA (similar vehicles):");
        prompt.AppendLine("Make,Model,Year,ZipCode,VehicleService,Gap,Maintenance,DentDing,Appearance,Windshield,KeyReplacement,Theft");

        foreach (var vehicle in similarVehicles)
        {
            prompt.AppendLine($"{vehicle.Make},{vehicle.Model},{vehicle.Year},{vehicle.ZipCode}," +
                            $"{(vehicle.VehicleService ? "Y" : "N")}," +
                            $"{(vehicle.Gap ? "Y" : "N")}," +
                            $"{(vehicle.Maintenance ? "Y" : "N")}," +
                            $"{(vehicle.DentAndDing ? "Y" : "N")}," +
                            $"{(vehicle.Appearance ? "Y" : "N")}," +
                            $"{(vehicle.Windshield ? "Y" : "N")}," +
                            $"{(vehicle.KeyReplacement ? "Y" : "N")}," +
                            $"{(vehicle.Theft ? "Y" : "N")}");
        }

        prompt.AppendLine();
        prompt.AppendLine("INSTRUCTIONS:");
        prompt.AppendLine("1. Analyze patterns in the historical data");
        prompt.AppendLine("2. Consider factors like vehicle make, model, year, and geographic location (zip code)");
        prompt.AppendLine("3. Rank the 8 service options from most likely to least likely to be purchased");
        prompt.AppendLine("4. Provide percentage likelihood for each service");
        prompt.AppendLine("5. Give brief reasoning for each ranking");
        prompt.AppendLine();
        prompt.AppendLine("RESPONSE FORMAT (JSON):");
        prompt.AppendLine("{");
        prompt.AppendLine("  \"rankings\": [");
        prompt.AppendLine("    {\"service\": \"ServiceName\", \"likelihood\": 85.5, \"reasoning\": \"explanation\"},");
        prompt.AppendLine("    ...more services...");
        prompt.AppendLine("  ],");
        prompt.AppendLine("  \"insight\": \"Overall insight about this vehicle's service purchasing pattern\"");
        prompt.AppendLine("}");
        prompt.AppendLine();
        prompt.AppendLine("Service names: VehicleService, Gap, Maintenance, DentDing, Appearance, Windshield, KeyReplacement, Theft");

        return prompt.ToString();
    }

    static ServicePrediction ParseAIResponse(string aiResponse)
    {
        try
        {
            // Extract JSON from the response (in case there's additional text)
            var jsonStart = aiResponse.IndexOf('{');
            var jsonEnd = aiResponse.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonContent = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);

                using (var document = JsonDocument.Parse(jsonContent))
                {
                    var root = document.RootElement;
                    var prediction = new ServicePrediction();

                    if (root.TryGetProperty("rankings", out var rankings))
                    {
                        foreach (var ranking in rankings.EnumerateArray())
                        {
                            var serviceRanking = new ServiceRanking
                            {
                                ServiceName = ranking.GetProperty("service").GetString(),
                                Likelihood = ranking.GetProperty("likelihood").GetDouble(),
                                Reasoning = ranking.TryGetProperty("reasoning", out var reasoningElement)
                                    ? reasoningElement.GetString() : ""
                            };
                            prediction.ServiceRankings.Add(serviceRanking);
                        }
                    }

                    if (root.TryGetProperty("insight", out var insight))
                    {
                        prediction.OverallInsight = insight.GetString();
                    }

                    return prediction;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing AI response: {ex.Message}");
            Console.WriteLine($"Raw response: {aiResponse}");
        }

        return null;
    }

    #endregion 
}