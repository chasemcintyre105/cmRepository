using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class HookAndAnchorFramework {

        private abstract class HookAndAnchorProfile {
            public abstract List<Hook> GetHooksList();
            public abstract Anchor GetAnchor();
            public abstract void Init();
            public abstract void Process();
            public abstract bool IsProcessed();
        }
        private class HookAndAnchorProfile<A> : HookAndAnchorProfile where A : Anchor {
            public List<Hook> hooks = new List<Hook>();
            public A anchor;
            public bool processed = false;
            public override List<Hook> GetHooksList() {
                return hooks;
            }
            public override Anchor GetAnchor() {
                return anchor;
            }
            public override void Init() {
                anchor.Init();
            }
            public override void Process() {
                processed = true;
            }
            public override bool IsProcessed() {
                return processed;
            }
        }

        private Dictionary<Type, HookAndAnchorProfile> ProfilesByAnchorType;

        public HookAndAnchorFramework() {
            ProfilesByAnchorType = new Dictionary<Type, HookAndAnchorProfile>();
        }

        public void RegisterAnchor<A>(A anchor) where A : Anchor {
            Assert.False("Anchor is not yet registered", ProfilesByAnchorType.ContainsKey(typeof(A)));
            HookAndAnchorProfile<A> profile = new HookAndAnchorProfile<A>();
            profile.anchor = anchor;
            ProfilesByAnchorType.Add(typeof(A), profile);
        }

        public void RegisterHook<A>(Hook<A> hook) where A : Anchor {
            Assert.True("Anchor is registered", ProfilesByAnchorType.ContainsKey(typeof(A)));
            ProfilesByAnchorType[typeof(A)].GetHooksList().Add(hook);
        }

        public void InitAllAnchors() {
            foreach (HookAndAnchorProfile profile in ProfilesByAnchorType.Values) {
                profile.Init();
            }
        }

        public void Process<A>() where A : Anchor {
            HookAndAnchorProfile profile = null;
            Assert.True("Anchor is registered", ProfilesByAnchorType.TryGetValue(typeof(A), out profile));
            A anchor = (A) profile.GetAnchor();
            foreach (Hook hook in profile.GetHooksList()) { 
                ((Hook<A>) hook).Process(anchor);
            }
            profile.Process();
        }

        public bool HasUnprocessedAnchors() {
            foreach (HookAndAnchorProfile profile in ProfilesByAnchorType.Values) {
                if (!profile.IsProcessed())
                    return true;
            }
            return false;
        }

    }

}