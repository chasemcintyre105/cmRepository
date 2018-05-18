using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngine {

    // This factory associates interfaces with implmentations that are otherwise unavailable due to scope.
    public class EffectFactory {

        private Dictionary<Type, Type> InterfaceToImplementationMap;
        private bool registrationFinalised = false;

        public EffectFactory() {
            InterfaceToImplementationMap = new Dictionary<Type, Type>();
        }

        // Note that it is faster to create the effect directly if the implementation is available
        public Effect NewEffect<E>() where E : Effect {

            if (!registrationFinalised)
                throw new Exception("Registration has been finalised");

            Type t = null;
            Assert.True("The effect interface is registered: " + typeof(E), InterfaceToImplementationMap.TryGetValue(typeof(E), out t));

            if (t == null)
                throw new Exception("Implementation of effect interface has been registered");

            try { 
                return (Effect) Activator.CreateInstance(t);
            } catch (Exception e) {
                throw new Exception("Failed to create instance of effect: " + e.Message);
            }
        }

        public void EnqueueNewEffect<E>(params object[] parameters) where E : Effect {
            NewEffect<E>().Init(parameters).Enqueue();
        }

        public void EnqueueNewEffectWithDelay<E>(float delayInSeconds, params object[] parameters) where E : Effect {
            NewEffect<E>().Init(parameters).EnqueueWithDelay(delayInSeconds);
        }

        public void RegisterEffectInterface<EffectInterface>() where EffectInterface : Effect {
            Assert.False("Registration is not yet finalised", registrationFinalised);
            Assert.False("Effect interface is not yet registered", InterfaceToImplementationMap.ContainsKey(typeof(EffectInterface)));

            InterfaceToImplementationMap.Add(typeof(EffectInterface), null);
        }

        public void RegisterEffectImplementation<EffectInterface, EffectImplementation>() where EffectInterface : Effect where EffectImplementation : EffectInterface { 
            Assert.False("Registration is not yet finalised", registrationFinalised);
            Assert.True("Effect interface is already registered: " + typeof(EffectInterface).Name, InterfaceToImplementationMap.ContainsKey(typeof(EffectInterface)));
            Assert.Null("Implementation of effect interface is not yet registered: " + typeof(EffectInterface).Name, InterfaceToImplementationMap[typeof(EffectInterface)]);
            Assert.True("Implementation is a subclass of the interface: " + typeof(EffectImplementation).Name, typeof(EffectImplementation).IsSubclassOf(typeof(EffectInterface)));

            InterfaceToImplementationMap[typeof(EffectInterface)] = typeof(EffectImplementation);
        }

        public void OverrideEffectImplementation<EffectInterface, EffectImplementation>() where EffectInterface : Effect where EffectImplementation : EffectInterface {
            Assert.False("Registration is not yet finalised", registrationFinalised);
            Assert.True("Effect interface is already registered: " + typeof(EffectInterface).Name, InterfaceToImplementationMap.ContainsKey(typeof(EffectInterface)));
            Assert.NotNull("Implementation of effect interface is already registered: " + typeof(EffectInterface).Name, InterfaceToImplementationMap[typeof(EffectInterface)]);
            Assert.True("Implementation is a subclass of the interface: " + typeof(EffectImplementation).Name, typeof(EffectImplementation).IsSubclassOf(typeof(EffectInterface)));

            InterfaceToImplementationMap[typeof(EffectInterface)] = typeof(EffectImplementation);
        }

        public void FinaliseRegistration() {
            Assert.False("Registration is not yet finalised", registrationFinalised);
            registrationFinalised = true;
            List<Type> typesToFill = new List<Type>();
            foreach (Type InterfaceType in InterfaceToImplementationMap.Keys) {
                if (InterfaceToImplementationMap[InterfaceType] == null) {
                    typesToFill.Add(InterfaceType);
                }
            }
            foreach (Type InterfaceType in typesToFill) {
                InterfaceToImplementationMap[InterfaceType] = typeof(Effect);
            }
        }

    }

}
