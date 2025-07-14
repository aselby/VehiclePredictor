# Vehicle Option Predictor with AI Service Prediction

A C# console application that loads vehicle data from CSV files and uses AI to predict the likelihood of customers purchasing various automotive services.

## Features

- **CSV Data Loading**: Automatically loads and parses vehicle data from CSV files
- **Smart Vehicle Creation**: Interactive prompts to create new vehicle entries with validation
- **AI Service Prediction**: Integrates with DeepSeek AI to predict service purchase likelihood
- **Statistical Analysis**: Provides fallback analysis when AI is unavailable
- **Data Insights**: Displays comprehensive statistics about your vehicle dataset

## Quick Start

### Prerequisites

- **.NET 5.0 or later**
- **DeepSeek API access** (optional, for AI predictions)
- **Vehicle data CSV file** (sample generator included)


## CSV Data Format

Your CSV file should include the following columns:

| Column | Type | Description |
|--------|------|-------------|
| Make | String | Vehicle manufacturer (Toyota, Ford, etc.) |
| Model | String | Vehicle model (Camry, F-150, etc.) |
| Year | Integer | Manufacturing year |
| Zip Code | Integer | 5-digit zip code |
| Vehicle Service | Y/N | Vehicle service package |
| Gap | Y/N | Gap insurance |
| Maintenance | Y/N | Maintenance package |
| Dent & Ding | Y/N | Dent and ding protection |
| Appearance | Y/N | Appearance protection |
| Windshield | Y/N | Windshield protection |
| Key Replacement | Y/N | Key replacement service |
| Theft | Y/N | Theft protection |

### Sample CSV Header
```csv
Make,Model,Year,Zip Code,Vehicle Service,Gap,Maintenance,Dent & Ding,Appearance,Windshield,Key Replacement,Theft
Toyota,Camry,2023,12345,Y,N,Y,N,Y,Y,N,N
```

## Usage

### Main Menu Options

1. **Enter New Vehicle**
   - Interactive prompts for all vehicle details
   - Input validation for years and zip codes
   - Optional AI analysis of purchase likelihood

2. **Exit**
   - Safely exit the application

### Example Workflow

```
Vehicle Data Predictor
======================
Successfully loaded 250 vehicle entries!

Dataset Statistics:
==================
Total Vehicles: 250

Top 10 Vehicle Makes:
  Volkswagen: 13 vehicles
  Cadillac: 10 vehicles
  Chrysler: 10 vehicles
  ...

Interactive Menu:
1. Create New Vehicle
0. Exit
Choose an option (0-1): 1

Create New Vehicle
==================
Enter Make: GMC
Enter Model: pickup
Enter Year: 2020
Enter Zip Code: 46815

...

Analyze service purchase likelihood with AI? (Y/N): Y

Analyzing service purchase likelihood with AI...
Analyzing based on 47 similar vehicles...

AI Prediction Results:
Service Purchase Likelihood (Most to Least Likely):
==================================================
1. Key Replacement - 90.0% likely
   Reasoning: High likelihood due to frequent key loss or breakage.
2. Maintenance Package - 85.0% likely
   Reasoning: Commonly needed and regularly performed services.
3. Vehicle Service - 80.0% likely
   Reasoning: Broad category covering regular maintenance, likely purchased frequently.
4. Appearance Protection - 75.0% likely
   Reasoning: Depends on vehicle aesthetics and personal preference.
5. Dent & Ding Protection - 65.0% likely
   Reasoning: Less frequent unless vehicles are involved in accidents or have significant damage.
6. Gap Insurance - 60.0% likely
   Reasoning: Depends on vehicle value after loss, less commonly purchased.
7. Windshield Protection - 50.0% likely
   Reasoning: Moderate likelihood based on usage conditions and frequency of issues.

## AI Integration

### DeepSeek API Configuration

1. **Get API Access**
   - Visit [DeepSeek Platform](https://platform.deepseek.com)
   - Create an account and obtain API key
   - Note the API endpoint URL

2. **Update Configuration**
   ```csharp
   const string DEEPSEEK_API_URL = "https://api.deepseek.com/v1/chat/completions";
   const Boolean DEEPSEEK_CLOUD = false; // If using Cloud deepseek change this to True
   const string API_KEY = "your-actual-api-key-here";
   ```

3. **Local AI Alternative (Ollama)**
   If using local Ollama instance:
   ```csharp
   const string OLLAMA_API_URL = "http://localhost:11434/api/generate";
   // Update request format for Ollama compatibility
   ```

### AI Analysis Features

- **Smart Data Filtering**: Uses similar vehicles for better predictions
- **Comprehensive Prompts**: Sends structured data with clear instructions
- **Percentage Predictions**: Returns likelihood percentages for each service
- **Reasoning**: AI explains why each service ranks where it does
- **Fallback Analysis**: Statistical analysis when AI is unavailable

## Dependencies

### Required NuGet Packages
```xml
<PackageReference Include="System.Text.Json" Version="7.0.0" />
<PackageReference Include="System.Net.Http" Version="4.3.4" />
```

## Data Generator

Included CSV file generates 1000 sample vehicle entries:

### Features
- **Realistic models** for each manufacturer
- **Years 2015-2024**
- **US zip codes** from major metropolitan areas
- **Random service selections**

## Configuration

### Timeout Settings
```csharp
// Unlimited timeout for slow AI responses
httpClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;

// Alternative: Set specific timeout
httpClient.Timeout = TimeSpan.FromMinutes(30);
```

### Model Parameters
```csharp
temperature = 0.3,      // Lower = more consistent
max_tokens = 5000,      // Response length limit
```

## Troubleshooting

### Common Issues

**"API call failed"**
- Verify API key is correct
- Check API endpoint URL
- Ensure internet connectivity

**"CSV file not found"**
- Verify file path and name
- Ensure CSV file is in project directory
- Check file permissions

**"Empty response from AI"**
- AI model may need more time
- Try reducing data complexity
- Check API rate limits

**HttpClient timeout**
- Use unlimited timeout for local AI
- Increase timeout duration
- Check model processing capacity

### Debug Tips

1. **Enable detailed logging**
   ```csharp
   Console.WriteLine($"Request: {jsonContent}");
   Console.WriteLine($"Response: {responseContent}");
   ```

2. **Test with smaller datasets**
   - Start with 10-20 vehicles
   - Gradually increase data size

3. **Validate CSV format**
   - Check for missing headers
   - Verify Y/N values
   - Ensure proper comma separation
