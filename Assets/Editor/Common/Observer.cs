using System;
using System.Collections.Generic;

public class Observer
{
    Dictionary<string, List<Action<dynamic[]>>> handlerLists;

    public Observer(){
        handlerLists = new Dictionary<string, List<Action<dynamic[]>>>();
    }
    public Switch Register(string eventID){
        string _eventID = eventID;
        int cnt = 2;
        while(handlerLists.ContainsKey(_eventID)){
            _eventID = eventID + "(" + cnt.ToString() + ")";
            cnt++;
        }
        handlerLists.Add(_eventID, new List<Action<dynamic[]>>());
        return new Switch(_eventID, handlerLists[_eventID]);
    }
    public void Subscribe(string eventID, Action<dynamic[]> handler){
        if(handlerLists.ContainsKey(eventID)){
            handlerLists[eventID].Add(handler);
        }
    }
    public void Unsubscribe(string eventID, Action<dynamic[]> handler){
        if(handlerLists.ContainsKey(eventID)){
            handlerLists[eventID].Remove(handler);
        }
    }
}
public class Switch{
    string eventID;
    List<Action<dynamic[]>> handlerList;
    public Switch(string _eventID, List<Action<dynamic[]>> _handlerList){
        eventID = _eventID;
        handlerList = _handlerList;
    }
    public void Invoke(params dynamic[] args){
        handlerList.ForEach((h)=>{
            h.Invoke(args);
        });
    }
}