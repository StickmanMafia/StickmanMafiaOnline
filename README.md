# Stickman Mafia Online

<p align="center" style="box-shadow: 4px 4px;">
      <a href="https://stickman-mafia.online/">
            <img
                src="https://i.ibb.co/TWjdL9C/stickman-mafia.webp"
                alt="144796728"
                width="300px"
              />
      </a>
</p>

## Overview

Welcome to Stickman Mafia Online, a blockchain-integrated social deduction game built on Unity and Solana. Stickman Mafia brings the classic Mafia gameplay to the digital world, allowing players to strategize, deceive, and collaborate in a virtual setting. Our mission is to provide a seamless and engaging Mafia experience that connects traditional game mechanics with the power of blockchain technology, offering unique rewards and opportunities for all players.
Game Type: Social deduction game, built with Unity and Solana blockchain for seamless integration of crypto elements.
Platform: Android and Desktop (cross-platform Unity experience).

## Game Features

- Classic Mafia Gameplay: Play the well-known game of Mafia in an immersive digital environment. Engage in rounds of deception, discussions, and vote-based eliminations as either a member of the Mafia or an innocent Townsperson.
- Blockchain Integration: Powered by the Solana blockchain, enjoy a unique gaming economy that rewards players for achievements and contributions to the game’s ecosystem.
- Play-to-Earn Mechanics: Win rounds, complete challenges, and participate in tournaments to earn cryptocurrency rewards, emphasizing both skill and social strategy.
- Tournament SDK: Organize and participate in community-driven tournaments with rewards. Our open-source Tournament SDK empowers developers within the Solana ecosystem to create unique events that boost engagement and foster competition.
- NFT Assets: Players can earn, trade, and collect in-game NFTs, including exclusive character skins and accessories that enhance the visual experience.
- Multiplayer Social Experience: Engage with other players through in-game chat, discussions, and decision-making processes, bringing the social aspect of Mafia to life.

## Solana Program: Multiplayer Mafia

This Solana program manages a multiplayer game where four players deposit funds into an escrow account. After all players deposit and somebody/group of people won, money will be transferred to the winner/winners.

### Key Features:
- Deposits: Each player sends a specified amount to the escrow account.
- Winner Payout: The total deposited amount is transferred to the winner.
- Future Integration: The winner logic will be updated to use a game API.

## Gameplay

Players join a virtual table where they are randomly assigned roles as either Mafia Members or Townspersons. The goal for the Mafia is to eliminate all Townspersons without being caught, while the Townspersons must work together to identify and vote out the Mafia members.

## Highlights:

- Role-Based Gameplay: Players take on unique roles such as Mafia, Doctor, Detective, and more, each with specific abilities that affect the outcome of the game.
- Vote and Discuss: Use the chat to discuss with other players and vote on suspects. Use deduction and persuasion to ensure your survival.
- Tournament Play: Participate in organized tournaments to earn cryptocurrency rewards, with fair and verifiable outcomes powered by smart contracts.

## Technology Stack

- Unity: Our core engine for creating an engaging 3D social experience with optimized performance across mobile devices.
- Solana Blockchain: High-speed blockchain used for token transactions, NFT ownership, and secure in-game trading.
- Web3 Integration: We leverage Web3 technologies to connect players’ wallets, enabling seamless on-chain activities.
- Rust & Python: Backend services that support blockchain interactions, tournament logic, and real-time player data.
- AWS & Docker: Scalable cloud services for backend support, hosting, and continuous deployment to ensure smooth performance.
- Tournament SDK: Developed for community use, this SDK simplifies organizing decentralized tournaments with verifiable rewards, making it accessible for beginner developers to create engaging events.

## Game Architecture

- Client-Server Model: The game follows a client-server model where Unity clients communicate with Solana-based backend services.
- WebSocket & NativeWebSocket: Real-time game data is exchanged via WebSocket connections, ensuring smooth multiplayer interactions.
- AWS Lambda Functions: For handling in-game blockchain transactions efficiently and scaling during peak load.
- Smart Contracts: Solana-based smart contracts facilitate secure asset transfers, NFT minting, and tournament rewards distribution.

## Installation & Setup

### Clone the Repository: Clone the repository from GitHub:
- git clone https://github.com/StickmanMafia/StickmanMafiaOnline.git
- Unity Setup: Ensure you have Unity 2021.3 or later installed. Open the project in Unity Hub.
- Dependencies: Install the required dependencies using Unity's Package Manager, including Web3 libraries and Solana SDK.
- Build & Run: Once setup is complete, build the game for Android using Unity's build options.

## How to Play

### Getting Started
- **Character Customization**: After logging in, customize your stickman character in the lobby. You can change their appearance, adjust graphic settings, and select your preferred language.
- **Game Setup**: When ready, press the "Play" button on the right side of the screen. Choose to either "Join" an existing room or "Create" your own.

### Room Creation
- **Create a Room**: If you decide to create your own room, you can set a custom room name, define the number of players, choose the player type (friends, password-protected, or open to all), and set a bet for the match.
- **Game Start**: Once settings are chosen, press "Play" to create the room. When enough players join, the game will begin, and roles will be distributed.

### Game Modes
- **The Raft (Current Map)**: Currently, the first available map is "The Raft," where players are randomly assigned roles. The goal of the game is to survive, strategize, and win as a team.
  - **Night Phase**: During the night, the Mafia (or the Maniac) selects their target.
  - **Day Phase**: During the day, the innocent townspeople must work together to discuss and vote for who they believe the culprit is, attempting to imprison them and keep the town safe.

### Game Flow
- **Role Distribution**: Once enough players have joined the room, the game starts and roles are distributed randomly. You could be a Mafia member, Maniac, Doctor, Detective, or a simple Townsperson.
- **Day & Night Cycles**: The game alternates between night and day cycles:
  - **Night**: The Mafia or Maniac makes their move.
  - **Day**: Players debate and vote on who to imprison. The goal is to eliminate all Mafia members before they overpower the innocents.

Stickman Mafia Online offers an exciting blend of strategy, social deduction, and player customization. Dive into the action, outwit your opponents, and ensure your team’s survival!

### Installation:

PC (Windows): [here](https://drive.google.com/file/d/11VdWUYANTvzBgSbaUhtSjWrxvYfFo9ks/view?usp=sharing)
Download from the release page.
Extract the file.
Run StickmanMafia.exe.

Android: [here](https://drive.google.com/file/d/1oUON4kwreyE2BvX3TO4zbaYlQ-Gd92eU/view?usp=sharing)
Download the APK from Google Play.
Install and launch the game.

Telegram: [here](https://t.me/mafia_stickman_bot)
Join via our Telegram bot.
Follow the bot's instructions to start playing.

- Join a Game: Enter a game room and get randomly assigned a role.
- Engage in Rounds: Participate in discussions, vote on suspects, and use your role abilities to influence the game.

## Business Model

- Stickman Mafia Online follows a play-to-earn model, enabling players to earn Solana-based tokens through in-game activities, tournaments, and trading NFTs. Our Tournament SDK is also available to the community, allowing developers to create new revenue opportunities by hosting and monetizing tournaments.
- In-Game Purchases: Players can buy NFTs, exclusive character skins, and role-specific abilities.
- Marketplace Trading: Trade your earned NFTs and tokens in a decentralized marketplace.

## Community

### We believe in building a vibrant community of players, developers, and blockchain enthusiasts.

- [Telegram](https://t.me/StickmanMafia): Join our Telegram to connect with the community, share feedback, and participate in events.
- [Twitter](https://x.com/StickmanMafia): Follow us on Twitter for news, updates, and giveaways. 
- Developer Collaboration: We encourage other developers to contribute and use our Tournament SDK to create unique gaming experiences within the Solana ecosystem.

# License

This project is licensed under a proprietary license - see the [LICENSE.md](LICENSE.md) file for details.

We are always looking for collaborators to join our journey. Whether you’re a developer, gamer, or blockchain enthusiast, Stickman Mafia Online offers something for everyone. Dive into our virtual table, and let’s build the future of decentralized gaming together!

Copyright ©️2024
