# Document Information System (DIS)

DIS is a robust .NET application designed for document analysis. Utilizing regular expressions (regex), this tool extracts information from documents and converts it into JSON format. Additionally, the system is integrated with a vector database for efficient data storage and retrieval.

## Features

- **Document Analysis:** Extract data from textual documents using powerful regular expressions.
- **Conversion to JSON:** Convert the extracted information into JSON format, facilitating data manipulation and integration.
- **Vector Database Support:** Supports storage in a vector database, optimizing the management and querying of large volumes of information.
- **Data Storage:** Ability to store extracted data securely and efficiently.

## Getting Started

### Prerequisites

- .NET 8.0
- A valid **OpenAI API Key** for OpenAI services.
- **Redis Connection String** for caching context data.
- **Qdrant Host** for the Qdrant vector database.
- **Qdrant API Key** for interacting with the Qdrant database.

### Installation

Clone the repository:

```bash
git clone https://github.com/ThiagoAndradeF/document-information-searcher
```

## Environment Variables

Before running the application, make sure to set the necessary environment variables:

- **OpenAiKey:** Your API key for OpenAI services.
- **RedisConnectionString:** The connection string for Redis, used for caching context data.
- **QdrantHost:** The host for the Qdrant vector database.
- **QdrantKey:** The API key required to interact with the Qdrant database.

## Project Objective

The goal of this project is to optimize the management of dynamic contexts and data, employing advanced techniques to ensure efficient storage and retrieval of information. The application integrates file uploads into the Qdrant vector database and manages contexts intelligently using strategies such as context caching and integration with the NoSQL Redis database. This enables efficient real-time management of document-related data, focusing on performance, scalability, and rapid information retrieval.

## Demonstrative Controller

For testing the methods implemented by the main class `TextAnalysisService.cs`, a demonstrative controller is provided. This controller allows you to interact with the service and test its functionality before integrating it into your application. You can call the methods from `TextAnalysisService.cs` directly through this controller for quick experimentation and validation of the implemented features.

## Future Availability

Once the project reaches a stable version, it will be made available on **NuGet** for easier integration into your applications.
