using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCase: MonoBehaviour
{
   
}
public class GameCaseClass
{
   
    public int Rarity ;
    public string Name ;
    public int OpeningTime ;
    public double SkinChance ;
    public double PerkChance ;
}


public class NewItems
{
    public int Gold ;
    public int Gems ;
    public string Skin ;
    public string Perk ;
}

public class Game
{
    private System.Random random = new System.Random();
    public List<GameCaseClass> cases = new List<GameCaseClass>
    {
        new GameCaseClass { Rarity = 1, Name = "Gray", OpeningTime = 120, SkinChance = 0.0, PerkChance = 0.1 },
        new GameCaseClass { Rarity = 2, Name = "Orange", OpeningTime = 240, SkinChance = 0.0, PerkChance = 0.25 },
        new GameCaseClass { Rarity = 3, Name = "Blue", OpeningTime = 480, SkinChance = 0.0, PerkChance = 0.5 },
        new GameCaseClass { Rarity = 4, Name = "Purple", OpeningTime = 720, SkinChance = 0.0, PerkChance = 0.7 }
    };

    private List<string> skins = new List<string> { "Bronze", "Silver", "Gold" };
    private List<string> perks = new List<string> { "ActiveRole", "Wiretapping", "LieDetector", "Revenge", "Radio","DoubleVoice", "DIsguise", "Helicopter", "MineDetector", "Mine" };

   public GameCaseClass GiveCase()
{
    double randomValue = random.NextDouble();
 
    if (randomValue < 0.7)
    {
        return cases[0]; // 0 варик с шансом 50%
    }
    else if (randomValue < 0.85)
    {
        return cases[1]; // 1 варик с шансом 35%
    }
    else if (randomValue < 0.98)
    {
        return cases[2]; // 2 варик с шансом 10%
    }
    else
    {
        return cases[3]; // 3 варик с шансом 5%
    }
}

     public GameCaseClass GiveCaseByRarity(int rarity)
    {
        
        return cases[rarity];
    }

    public NewItems GenerateCaseDrop(int rarity)
    {
        NewItems newItems = new NewItems();

        switch (rarity)
        {
            case 1:
                newItems.Gold = random.Next(100, 501);
                newItems.Gems = random.NextDouble() < 0.5 ? 1 : 0;
                break;
            case 2:
                newItems.Gold = random.Next(500, 1001);
                newItems.Gems = random.NextDouble() < 0.3 ? 1 : 0;
                break;
            case 3:
                newItems.Gold = random.Next(1000, 2001);
                newItems.Gems = random.NextDouble() < 0.2 ? 2 : 0;
                break;
            case 4:
                newItems.Gold = random.Next(2000, 4001);
                newItems.Gems = random.NextDouble() < 0.3 ? 2 : 0;
                break;
        }

        if (random.NextDouble() < cases[rarity-1].SkinChance)
        {
            newItems.Skin = skins[random.Next(skins.Count)];
        }

        if (random.NextDouble() < cases[rarity-1].PerkChance)
        {
            newItems.Perk = perks[random.Next(perks.Count)];
        }
        
        return newItems;
    }
}
