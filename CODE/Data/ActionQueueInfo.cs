#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

public struct ActionQueueInfo
{
    private readonly int[]? _allIds;

    public IEnumerable<int>? AllIds => _allIds!.ToArray();
    public int CurrentId { get; private set; }

    public bool IsLast => CurrentId == _allIds!.Last();
    
    // info string example = "all-ids=2,3,4;current-id=1"
    public ActionQueueInfo(string infoString)
    {
        if (infoString is Strings.NoActiveQueues or Strings.None)
        {
            _allIds = null;
            CurrentId = Numerics.NoId;
            return;
        }

        const int allIdsIndex = 0;
        const int currentIdIndex = 1;
        
        var splitString = infoString.Split(Strings.DefaultSeparator);
        
        var allIdsStrings = splitString[allIdsIndex].Split("=")[1].Split(Strings.AdditionalSeparator);
        _allIds = allIdsStrings.Select(n => Convert.ToInt32(n)).ToArray();

        var currentIdString = splitString[currentIdIndex].Split("=")[1];
        CurrentId = Convert.ToInt32(currentIdString);
    }
    
    public ActionQueueInfo(IEnumerable<int> allIds, int currentId)
    {
        _allIds = allIds.ToArray();
        CurrentId = currentId;
    }
    
    public string ConvertIntoString()
    {
        var finalString = "all-ids=";
        
        foreach (var id in _allIds!)
        {
            if (id == _allIds.Last())
                finalString += id + Strings.DefaultSeparator;
            else
                finalString += id + Strings.AdditionalSeparator;
        }

        finalString += "current-id=" + CurrentId;

        return finalString;
    }
    
    public void MoveForward()
    {
        if (_allIds == null)
            return;
        
        for (int i = 0; i < _allIds.Length; i++)
        {
            if (_allIds.ElementAt(i) != CurrentId) continue;
            
            CurrentId = _allIds.ElementAt(i + 1);
            return;
        }
    }

    public int GetPlayerPosition(int actorId) => Array.IndexOf(_allIds, _allIds.First(n => n == actorId)); 
    
    public int GetCurrentPlayerPosition() => Array.IndexOf(_allIds, CurrentId);
    
    public bool AnyPlayersWithActorId(int actorId) => _allIds != null && _allIds.Any(n => n == actorId);
}