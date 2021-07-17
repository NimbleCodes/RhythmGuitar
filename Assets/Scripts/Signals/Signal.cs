using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace KusoGame.Signals{
    public class SignalManager{
        Dictionary<string, Signal> signals;
        public SignalManager(){
            signals = new Dictionary<string, Signal>();
        }
        public Switch Register(string signalId, Type callbackFormat){
            if(signals.ContainsKey(signalId)){
                throw new ArgumentException();
            }
            signals.Add(signalId, new Signal(callbackFormat));
            return new Switch(signals[signalId].handlerList.AsReadOnly());
        }
        public void Subscribe(string signalId, object listener, string method){
            if(!signals.ContainsKey(signalId)){
                throw new KeyNotFoundException();
            }
            try{
                Delegate handler = Delegate.CreateDelegate(signals[signalId].callbackFormat, listener, method);
                signals[signalId].handlerList.Add(handler);
                signals[signalId].handlerImplInst.Add(listener);
            }
            catch(ArgumentException){
                Debug.LogError(method + "does not match the callback format of " + signalId);
            }
        }
        public void Unsubscribe(string signalId, object listener){
            if(!signals.ContainsKey(signalId)){
                throw new KeyNotFoundException();
            }
            for(int i = 0; i < signals[signalId].handlerList.Count; i++){
                if(signals[signalId].handlerImplInst[i] == listener){
                    signals[signalId].handlerImplInst.RemoveAt(i);
                    break;
                }
            }
        }
    }
    class Signal{
        internal Type callbackFormat;
        internal List<Delegate> handlerList;
        internal List<object> handlerImplInst;
        public Signal(Type _callbackFormat){
            callbackFormat = _callbackFormat;
            handlerList = new List<Delegate>();
            handlerImplInst = new List<object>();
        }
    }
}
