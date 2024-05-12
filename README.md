# Tic Tac Toe Game - Backend

## Overview
This project is the backend API for a Tic Tac Toe game, providing endpoints for user authentication, game creation, game joining, and game moves. It serves as the server-side component for the Tic Tac Toe frontend application.

## Technology Stack
- **C#**: Primary programming language.
- **ASP.NET Core**: Framework for building web APIs.
- **JSON**: Data format for API communication.

## Prerequisites
- **.NET SDK**: Ensure you have the .NET SDK installed on your machine. You can download it from the [official .NET website](https://dotnet.microsoft.com/download).

## Setup Instructions

### Option 1: Build the Project from Source

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/Grzybol/GWSH_PO_Projekt_API-Backend.git
   cd GWSH_PO_Projekt_API-Backend
   ```

2. **Build the Project**:
   - Navigate to the project directory:
     ```bash
     cd TicTacToeServer
     ```
   - Restore dependencies and build:
     ```bash
     dotnet restore
     dotnet build
     ```

3. **Run the Server**:
   ```bash
   dotnet run
   ```
   The server will start and listen on the default port 5000.

### Option 2: Use Pre-built Release

1. **Download `backend.zip`**:
   - Download the file from the [repository's release section](https://github.com/Grzybol/GWSH_PO_Projekt_API-Backend).

2. **Extract the Zip File**:
   - Extract the contents of `backend.zip` to a directory of your choice.

3. **Run the Executable**:
   - Navigate to the extracted directory:
     ```bash
     cd path/to/extracted/directory
     ```
   - Run the executable:
     ```bash
     ./TicTacToeServer
     ```
   The server will start and listen on the default port 5000.

## API Endpoints

### Authentication
- **Register**: `POST /auth/register`
  - Request Body: `{ "username": "string", "password": "string" }`
  - Response: `200 OK` on success, `400 Bad Request` if the user already exists.

- **Login**: `POST /auth/login`
  - Request Body: `{ "username": "string", "password": "string" }`
  - Response: `200 OK` on success, `401 Unauthorized` on failure.

### Game Management
- **Get Active Games**: `GET /game/active`
  - Response: List of active games.

- **Get Completed Games**: `GET /game/completed`
  - Response: List of completed games.

- **Create New Game**: `POST /game/new`
  - Request Body: `string` (player name)
  - Response: `200 OK` with game details, `500 Internal Server Error` on failure.

- **Join Game**: `POST /game/join?gameId={id}`
  - Request Body: `string` (player name)
  - Response: `200 OK` with game details, `400 Bad Request` if the game is full or not found.

- **Make Move**: `POST /game/move`
  - Request Body: `{ "row": "int", "col": "int", "player": "string", "gameId": "int" }`
  - Response: `200 OK` with updated game state, `400 Bad Request` on invalid move.

- **Get Game Status**: `GET /game/status?gameId={id}`
  - Response: `200 OK` with game details, `404 Not Found` if no active game is found.

## File Structure
- `Program.cs`: Main entry point of the application.
- `AuthController.cs`: Handles user registration and login.
- `GameController.cs`: Manages game creation, joining, and moves.
- `Game.cs`: Model for game data.
- `Move.cs`: Model for move data.

## Contributing
Feel free to fork this repository and contribute by submitting pull requests. Any improvements, bug fixes, or suggestions are welcome.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.

## Contact
For any queries or issues, please contact the project maintainer at [your-email@example.com].
