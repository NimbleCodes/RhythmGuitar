using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace kgh.Signals
{
    public class Exchange
    {
        Dictionary<string, Signal> signals;
        public Exchange(){
            signals = new Dictionary<string, Signal>();
        }
        public Switch Register(string sigId, Type callbackFormat){
            if(signals.ContainsKey(sigId)){
                throw new ArgumentException();
            }
            signals.Add(sigId, new Signal(callbackFormat));
            return new Switch(signals[sigId].callbacks.AsReadOnly());
        }
        public void Subscribe(string sigId, object implInst, string methodId){
            if(!signals.ContainsKey(sigId)){
                throw new KeyNotFoundException();
            }
            Delegate temp = Delegate.CreateDelegate(signals[sigId].callbackFormat,implInst, methodId);
            signals[sigId].callbacks.Add(temp);
        }
        public void Unsubscribe(string sigId, object implInst, string methodId){
            if(!signals.ContainsKey(sigId)){
                throw new KeyNotFoundException();
            }
            for(int i = 0; i < signals[sigId].callbacks.Count; i++){
                if(signals[sigId].callbacks[i].Target.Equals(implInst) && signals[sigId].callbacks[i].Method.Name == methodId){
                    signals[sigId].callbacks.RemoveAt(i);
                    break;
                }
            }
        }
    }
    public class Signal
    {
        internal Type callbackFormat;
        internal List<Delegate> callbacks;
        public Signal(Type _callbackFormat){
            callbackFormat = _callbackFormat;
            callbacks = new List<Delegate>();
        }
    }
    public class Switch
    {
        public ReadOnlyCollection<Delegate> callbacks;
        public Switch(ReadOnlyCollection<Delegate> _callbacks){
            callbacks = _callbacks;
        }
        public dynamic[] Invoke(params dynamic[] args){
            dynamic[] retVals = new dynamic[callbacks.Count];
            for(int i = 0; i < callbacks.Count; i++){
                retVals[i] = callbacks[i].DynamicInvoke(args);
            }
            return retVals;
        }
    }
}