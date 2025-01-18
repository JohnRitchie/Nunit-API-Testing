# README

## Project Overview
This project contains API tests written in C# using NUnit to validate the RESTful API of [JSONPlaceholder](https://jsonplaceholder.typicode.com). The tests cover basic HTTP methods (`GET`, `POST`, `PUT`, `DELETE`) to ensure the API's endpoints behave as expected.

### Features:
- Comprehensive testing of JSONPlaceholder API endpoints.
- Automated test execution with NUnit.
- Detailed request and response logging integrated with Allure for enhanced reporting.

## Prerequisites
Before running the tests, ensure the following tools and dependencies are installed:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [NUnit](https://nunit.org/) test framework
- [Allure NUnit Adapter](https://github.com/allure-framework/allure-nunit)
- [Newtonsoft.Json](https://www.newtonsoft.com/json) for JSON handling

## Test Scenarios
The project implements the following test scenarios:

### 1. GET Request
- **Endpoint**: `/posts/1`
- **Validations**:
    - Verify the response status code is `200`.
    - Verify the response body contains expected fields (`userId`, `id`, `title`, `body`).

### 2. POST Request
- **Endpoint**: `/posts`
- **Payload**:
  ```json
  {
    "title": "foo",
    "body": "bar",
    "userId": 1
  }
  ```
- **Validations**:
    - Verify the response status code is `201`.
    - Verify the response body contains details of the newly created post.

### 3. PUT Request
- **Endpoint**: `/posts/1`
- **Payload**:
  ```json
  {
    "id": 1,
    "title": "new_foo",
    "body": "new_bar",
    "userId": 1
  }
  ```
- **Validations**:
    - Verify the response status code is `200`.
    - Verify the response body contains updated post details.

### 4. DELETE Request
- **Endpoint**: `/posts/1`
- **Validations**:
    - Verify the response status code is `200` or `204`.
    - Verify the response body is either empty or contains `{}`.

## How to Run the Tests

### 1. Clone the Repository
```bash
git clone <repository_url>
cd <repository_directory>
```

### 2. Install Dependencies
Ensure that the required NuGet packages are installed:
```bash
dotnet restore
```

### 3. Execute Tests
Run the tests using the `dotnet` CLI:
```bash
dotnet test
```

### 4. Generate Allure Reports
To view detailed reports:
1. Install the Allure command-line tool: [Allure Installation Guide](https://docs.qameta.io/allure/#_installing_a_commandline).
2. Generate the Allure report:
   ```bash
   allure serve <output_directory>
   ```
3. Example:
   ```bash
    allure serve .\bin\Debug\net9.0\allure-results
   ```

## File Structure
```
├── ApiTests.cs             # Main test class containing all test cases
├── allureConfig.json       # Allure configuration file
└── README.md               # Project documentation
```

## Contribution
Feel free to fork the repository and create pull requests to improve the project. If you encounter any issues, please submit them in the "Issues" section.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.

---

Happy Testing! 🚀

