# 1.Introduction
This document describes the implementation of a Web API and an MVC Web Application developed using .NET 6. The purpose of these applications is to manage user profiles with functionalities like login, registration, and profile management. The Web API serves as the backend for the MVC Web Application. It is designed with security, performance, and extensibility in mind.

# 2. Overview
## Web API
The Web API was created using .NET 6 and provides three core endpoints:

   + **Login User**: Authenticates the user and returns a JWT token for authorized access.  
   + **Register a New User:** Registers a new user with required details.  
   + **Get and View User Profiles:** Retrieves the user’s profile information.
  
The API leverages JWT tokens for secure authentication, CORS (Cross-Origin Resource Sharing) for managing cross-domain requests, Versioning to support backward compatibility for different API versions, and Logging and Error Handling for better monitoring and troubleshooting.

**Technology Stack**:
+ .NET 6 Web API
+ Entity Framework for SQL Server interaction
+ JWT-based authentication
+ SQL Server database
  
## MVC Web Application
The MVC Web Application is built using .NET 6 and interfaces with the Web API for user-related operations. It provides multiple user interface pages for:

+ Login  
+ Registration
+ Viewing and updating user profiles

This web application communicates with the Web API to save and update user details, ensuring a seamless user experience.

**Technology Stack:**

+ .NET 6 MVC
+ Web API Integration (via HTTP requests)
+ Razor views for dynamic user interfaces
## Unit Testing with xUnit
To ensure the functionality of the Web API, unit tests were written using the xUnit testing framework. Specifically, a test case was created to verify the login functionality, ensuring the API works as expected for user authentication.

# 3. Detailed Architecture
## 3.1 Web API Design
The Web API is designed to expose the following endpoints:

### Login User Endpoint
+ **Endpoint**: POST /api/user/login
+ **Description**: Authenticates the user with provided credentials (username, password) and returns a JWT token if authentication is successful.
+ **Authorization**: JWT token-based authentication.
+ **Response**: JWT token on successful login; error message on failure.
### Register User Endpoint
+ **Endpoint**: POST /api/user/register
+ **Description**: Registers a new user by accepting registration details (e.g., username, password, email) and stores it in the SQL Server database.
+ **Authorization**: No authentication required for registration.
+ **Response**: Success or failure message based on the operation.
### Get/Update User Profile Endpoint
+ **Endpoint**: GET /api/user/profile and POST /api/user/profile
+ **Description**: Retrieves and updates the user’s profile details. The profile is stored and retrieved from the SQL Server database.
+ **Authorization**: JWT token required for authentication.
+ **Response**: User profile data in response to a GET request, or success/error message for update requests.

  ![image](https://github.com/user-attachments/assets/e385e03e-f9b5-44e6-9f94-32d9e9980203)
## 3.2 Security and Robustness Features
+ **JWT Token Authentication**: Ensures that only authorized users can access the API’s resources.
+ **CORS**: Enabled to allow cross-origin requests from trusted domains.
+ **API Versioning**: Supports multiple versions of the API to ensure backward compatibility.
+ **Logging**: Logs information on API requests, responses, and errors for easier debugging and monitoring.
+ **Error Handling**: Implements standardized error handling with meaningful HTTP status codes and error messages.
## 3.3 Database Layer
The application uses Entity Framework to interact with a SQL Server database for managing user data. This includes:

+ User credentials
+ Profile details
+ Registration metadata
## 3.4 MVC Web Application Design
The MVC Web Application interacts with the Web API for user authentication and profile management. The application has the following features:

+ **Login Page**: A form to allow users to log in by providing their credentials.
 ![image](https://github.com/user-attachments/assets/f0556941-17a9-46c4-8578-ce984bcb296d)

+ **Register Page**: A form for user registration.
![image](https://github.com/user-attachments/assets/c377d73e-f5e5-489a-81ca-24b0747c6e3f)

+ **Profile Page**: Displays user information, with an option to update profile details.
![image](https://github.com/user-attachments/assets/a914fc10-2f95-46d6-9ffd-e13fe75d4211)
The web application sends requests to the Web API to perform operations like saving and updating user details.

## 3.5 xUnit Testing
Unit tests were created to ensure the correct functionality of the Web API. These tests include:

+ Verifying the login functionality: Ensuring that a user can log in with correct credentials and receives a valid JWT token.
The tests are created using the xUnit framework, which is integrated into the development environment to run and validate these tests automatically.
![image](https://github.com/user-attachments/assets/d5f8cd6d-da5f-46b5-9dfe-6c16e7e08824)

# 4. Setup Instructions
The primary step is to checkout the the code repo on your local machine and following further steps;
## WEB API
+ To run the Web API locally, you first need to create an environment variable on your machine. There is a file named set-env-vars.bat in the API repository. Simply double-click on this file, and it will generate the necessary environment variables on your local machine.
+ Additionally, you will need to set the database connection string in the appsettings.json file.
+ Next, to create the database, run the following migration command:
    dotnet ef database update
## WEB API
 + To run the web application, update the Web API references in the local configuration file. Then, build and run the project.

## WEB

# 5. Implementation Details
## 5.1 API Authentication and Security
+ The Web API uses JWT for authentication. On successful login, a JWT token is issued and sent back to the client. This token is then used for subsequent API requests to verify the user’s identity.
+ CORS is enabled to allow requests from trusted origins, ensuring that cross-origin resource sharing does not pose security risks.
## 5.2 Error Handling and Logging
+ The API utilizes middleware for global error handling, ensuring that errors are returned with standardized HTTP status codes and error messages.
+ Logging is implemented using a logging framework to log important information about API requests, responses, and exceptions.
## 5.3 Entity Framework Integration
The Web API connects to a SQL Server database using Entity Framework Core, which abstracts the database interactions and allows for easy querying and data manipulation. The Entity Framework is used to handle operations like:
+ Retrieving user profile data
+ Inserting new users during registration
+ Updating user profile details
## 5.4 MVC Web Application
The MVC Web Application is structured into controllers, models, and views:
+ Controllers handle requests and responses, interacting with the Web API.
+ Models define the data structures (e.g., user credentials, profiles).
+ Views are Razor pages that present dynamic content to the user.
## 5.5 Unit Testing with xUnit
The xUnit test project was created to test the Web API’s login functionality:

+ **Test Case**: Verifies that the login API endpoint returns a valid JWT token when correct credentials are provided.
+ **Test Framework**: xUnit, with assertions to validate API responses.
# 6. Conclusion
The .NET 6 Web API and MVC Web Application provide a secure, user-friendly platform for managing user profiles. With the use of JWT tokens, CORS, versioning, and robust logging and error handling mechanisms, the applications are designed for scalability, maintainability, and security. Unit testing using xUnit ensures the reliability of key functionalities like user login, while Entity Framework provides efficient interaction with the SQL Server database.

# 7. Areas for Improvements
  + UX/UI
  + Implement more advanced user role-based authorization.
  + Use PUT call instead of POST call when updating the user information. 
