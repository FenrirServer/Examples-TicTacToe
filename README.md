# Tic-Tac-Toe Example

This project contains example Tic-Tac-Toe game implemented using Fenrir Multiplayer.

## Multiplayer Networking

Please refer to the main [Fenrir.Multiplayer](https://github.com/FenrirServer/Fenrir.Multiplayer) repository for the general documentation.

This project uses several Requests, Responses and Events (see: [Networking Basics](https://github.com/FenrirServer/Fenrir.Multiplayer/blob/master/docs/NetworkingBasics.md))

- TicTacToeGameStartEvent - broadcast when all players are connecte
- TicTacToeGameFinishEvent - broadcast when the game is over
- TicTacToeMoveRequest - sent by the player to make a move
- TicTacToeMoveResponse - sent back as a response to the TicTacToeMoveRequest (valid/invalid move etc)
- TicTacToeMoveEvent - broadcast when other player makes a move


## Project Structure

```
📂TicTacToe
 ┣ 📂 Packages
 ┣ 📂 Library
 ┣ 📂 ProjectSettings
 ┣ 📂 Assets
   ┗ 📂 Source
     ┗ 📄 TicTacToeBehaviour.cs             ← Tic-tac-toe Unity Client implementation
 ┣ 📁Server
   ┣ 📂MyGame.Server
      ┣ 📄 TicTacToe.Server.csproj          ← Server .NET Project
      ┣ 📄 Program.cs                       ← Program Entry-point  
      ┣ 📄 Application.cs                   ← Main Server class (room management)
      ┣ 📄 TicTacToeServerPlayer.cs         ← Server Player class (represents a connected player, stores network peer)
      ┣ 📄 TicTacToeTicTacToeServerRoom.cs  ← Server Room - main gameplay logic class
      ┣ 📄 FenrirLogger.cs                  ← Server logger
   ┣ 📂 MyGame.Shared
      ┣ 📄 TicTacToe.Shared.csproj          ← Shared Library .NET Project
      ┣ 📄 TicTacToeGameState.cs            ← Game State enum
      ┣ 📄 TicTacToeGameStartEvent.cs       ← Game Start Event
      ┣ 📄 TicTacToeGameFinishEvent.cs      ← Game Finish Event
      ┣ 📄 TicTacToeMoveRequest.cs          ← Player move request
      ┣ 📄 TicTacToeMoveResponse.cs         ← Player move response
      ┣ 📄 TicTacToeMoveEvent.cs            ← Player move event (broadcast to enemy player)
      ┣ 📄 TicTacToePieceType.cs            ← Piece type (X or O)
      ┣ 📄 TicTacToePlayerReference.cs      ← Player reference (player name, piece type etc)
   ┣ 📄 ServerApplication.sln               ← Server solution file
   ┗ 📄 Dockerfile                          ← Dockerfile that defines how the docker image for your server is built
```

## Building and Running Server

```bash
cd TicTacToe/Server
docker build . -t tictactoe:latest
docker run -p 27016:27016/tcp -p 27016:27016/udp tictactoe:latest
```

You can also build and run the [TicTacToe/Server/ServerApplication.sln](https://github.com/FenrirServer/Examples-TicTacToe/blob/master/TicTacToe/Server/ServerApplication.sln) visual studio solution.

## Connecting to a Server

Open Unity project and run TicTacToe.scene. Enter server url (`http://localhost:27016` by default), server room id (1 by default) and player name, and click connect.
The game starts when 2 players are connected to the same room.
