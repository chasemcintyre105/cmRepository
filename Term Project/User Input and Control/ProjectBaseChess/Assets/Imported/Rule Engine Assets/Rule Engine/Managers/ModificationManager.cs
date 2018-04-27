using System.Collections.Generic;

namespace RuleEngine {

    public class ModificationManager : IManager {

        protected readonly object _modLock;

        protected ModificationStack<Modification> ModStack;

        public ModificationManager() {
            _modLock = new object();
        }

        public override void Preinit() {
            ModStack = new ModificationStack<Modification>();
        }

        public override void Init() {
        }

        // Threadsafe ModStack functions
        public virtual void ApplyModification_TS(Modification mod) {
            lock (_modLock) {
                ModStack.ApplyModification(mod);
            }
        }

        public virtual void UndoAllModifications_TS() {
            lock (_modLock) {
                ModStack.UndoAllModifications();
            }
        }

        public virtual void UndoModificationsUpToAndIncluding_TS(Modification mod) {
            lock (_modLock) {
                ModStack.UndoModificationsUpToAndIncluding(mod);
            }
        }

        public virtual void UndoModificationsUpTo_TS(Modification mod) {
            lock (_modLock) {
                ModStack.UndoModificationsUpTo(mod);
            }
        }

        public virtual void ClearModifications_TS() {
            lock (_modLock) {
                ModStack.ClearModifications();
            }
        }

        public virtual LinkedList<Modification> GetClearedModifications_TS() {
            lock (_modLock) {
                return ModStack.GetClearedModifications();
            }
        }

        public virtual Modification PeekLastModification_TS() {
            lock (_modLock) {
                return ModStack.Peek();
            }
        }

        public virtual void SetUndoBlock_TS(Modification mod) {
            lock (_modLock) {
                ModStack.SetUndoBlock(mod);
            }
        }

        public virtual void ClearUndoBlock_TS() {
            lock (_modLock) {
                ModStack.ClearUndoBlock();
            }
        }

    }

}
