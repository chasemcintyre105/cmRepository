using RuleEngine;
using System;
using System.Collections.Generic;

namespace RuleEngineAddons.RulePanel {

	/*
	 * Determines what and how many rule changes can be made before the players turn is up.
	 */
	public class LimitedRuleJudge : RuleJudge {

        private PanelManager _PanelManager;
        private PanelManager PanelManager {
            get {
                if (_PanelManager == null)
                    _PanelManager = E.GetManager<PanelManager>();

                return _PanelManager;
            }
        }
        
        private Judgement _judgement;
		private Judgement judgement {
			get {
				return _judgement;
			}
			set {
				_judgement = value;
                PanelManager.SetJudgementText(_judgement.ToString());

                if (value == Judgement.Finished) {

                    PanelManager.ShowAddRuleAvailableNotification(false);
                    PanelManager.ShowRemoveRuleAvailableNotification(false);
                    PanelManager.ShowAddObjectAvailableNotification(false);
                    PanelManager.ShowRemoveObjectAvailableNotification(false);
                    PanelManager.ShowSwapObjectAvailableNotification(false);
                    PanelManager.ShowReplaceObjectAvailableNotification(false);

                }
			}
		}

		private bool _newRuleAllowed;
		private bool newRuleAllowed {
			get {
				return _newRuleAllowed;
			}
			set {
				_newRuleAllowed = value;

                // Update possible action indicator in GUI
                PanelManager.ShowAddRuleAvailableNotification(IsNewRuleAllowed());
            }
		}

		private bool _removingRuleAllowed;
		private bool removingRuleAllowed {
			get {
				return _removingRuleAllowed;
			}
			set {
				_removingRuleAllowed = value;

                // Update possible action indicator in GUI
                PanelManager.ShowRemoveRuleAvailableNotification(IsRemovingARuleAllowed());
            }
		}

		private bool _newStatementIsAllowed;
		private bool newStatementIsAllowed {
			get {
				return _newStatementIsAllowed;
			}
			set {
				_newStatementIsAllowed = value;

                // Update possible action indicator in GUI
                PanelManager.ShowAddObjectAvailableNotification(IsNewStatementAllowed());
            }
		}

		private bool _removingObjectIsAllowed;
		private bool removingObjectIsAllowed {
			get {
				return _removingObjectIsAllowed;
			}
			set {
				_removingObjectIsAllowed = value;

                // Update possible action indicator in GUI
                PanelManager.ShowRemoveObjectAvailableNotification(IsRemovingAnObjectAllowed());
            }
		}
		
		private bool _swappingObjectsIsAllowed;
		private bool swappingObjectsIsAllowed {
			get {
				return _swappingObjectsIsAllowed;
			}
			set {
				_swappingObjectsIsAllowed = value;

                // Update possible action indicator in GUI
                PanelManager.ShowSwapObjectAvailableNotification(IsSwappingObjectsAllowed());
            }
		}
		
		private bool _replacingObjectIsAllowed;
		private bool replacingObjectIsAllowed {
			get {
				return _replacingObjectIsAllowed;
			}
			set {
				_replacingObjectIsAllowed = value;

                // Update possible action indicator in GUI
                PanelManager.ShowReplaceObjectAvailableNotification(IsReplacingAnObjectAllowed());
            }
		}

		private bool HasAddedBlockStatement;

		private Rule AssociatedRule;
        private RuleComponent BaseObjectOfChanges;

        private Engine E;

        public LimitedRuleJudge(Engine E) {
            this.E = E;
		}

		// Check the latest modification and determine whether the player has finished their turn or not.
		public override void JudgeStack(Stack<RuleModification> stack) {

			Modification mod = stack.Peek();
            
			if (mod.GetType() == typeof(RemoveRuleModification)) {
                SelectiveDebug.LogJudge("RemoveRuleModification");

                if (!IsRemovingARuleAllowed())
                    throw new Exception("Removing a rule is not allowed at this point");

                if (BaseObjectOfChanges != null)
                    throw new Exception("BaseObjectOfChanges must be null at this point");

                // Once a rule has been removed it is the end of the turn
                judgement = Judgement.Finished;

			} else if (mod.GetType() == typeof(AddRuleModification)) {
                SelectiveDebug.LogJudge("AddRuleModification");

                if (!IsNewRuleAllowed())
                    throw new Exception("Adding a new rule is not allowed at this point");

                // This is not the end of the turn because the player must add something to the rule
                judgement = Judgement.StillGoing;
				
				// Register this rule and the block statement that comes with it
				AssociatedRule = ((AddRuleModification) mod).AssociatedRule;

                if (AssociatedRule == null)
                    throw new Exception("AssociatedRule must not be null at this point");

                if (BaseObjectOfChanges != null)
                    throw new Exception("BaseObjectOfChanges must be null at this point");

                BaseObjectOfChanges = AssociatedRule.Block.ArgumentList.argsByOrder[0].reference.Instance();

                if (BaseObjectOfChanges == null)
                    throw new Exception("BaseObjectOfChanges must not be null at this point");

                // Only one new rule is permitted each turn
                newRuleAllowed = false;

				// After that, removing a rule is no longer possible
				removingRuleAllowed = false;

                // A single void statement is automatically added to the rule so adding a new statement is not needed
                newStatementIsAllowed = false;

                // Once a rule is added, no objects may be removed or swapped
                removingObjectIsAllowed = false;
				swappingObjectsIsAllowed = false;

			} else if (mod.GetType() == typeof(AddStatementModification)) {
                SelectiveDebug.LogJudge("AddStatementModification");

                if (!IsNewStatementAllowed())
                    throw new Exception("Adding a new statement is not allowed at this point");

                // Otherwise the player is still playing their turn
                judgement = Judgement.StillGoing;
                    
				// Once a statement is added, adding or removing rules is not permitted
				newRuleAllowed = false;
				removingRuleAllowed = false;

                // Nor is adding more statement, removing object or swapping them
				newStatementIsAllowed = false;
				removingObjectIsAllowed = false;
				swappingObjectsIsAllowed = false;
                
			} else if (mod.GetType() == typeof(RemoveRuleComponentModification)) {
                SelectiveDebug.LogJudge("RemoveObjectModification");

                if (!IsRemovingAnObjectAllowed())
                    throw new Exception("Removing an object is not allowed at this point");

                RemoveRuleComponentModification removeObjectModification = (RemoveRuleComponentModification) mod;

                if (removeObjectModification.associatedObject is Statement) {

                    // Once a statement has been removed it is the end of the turn
                    judgement = Judgement.Finished;

                } else {

                    // Set the base object to the parent of the remove object, if not already set
                    if (BaseObjectOfChanges == null) { 
                        BaseObjectOfChanges = removeObjectModification.associatedObjectAccessor.obj;

                        if (BaseObjectOfChanges == null)
                            throw new Exception("BaseObjectOfChanges must not be null at this point");

                    }

                    // If an expression or something has been removed then there is likely a null left over
                    if (AreAllValuesCompleted()) {
                        judgement = Judgement.Finished;
                    } else {
                        judgement = Judgement.StillGoing;

                        // Once a rule component is removed and something still remains to be edited, only replacing is a valid option
                        newRuleAllowed = false;
                        removingRuleAllowed = false;
                        newStatementIsAllowed = false;
                        removingObjectIsAllowed = false;
                        swappingObjectsIsAllowed = false;

                    }

                }

            } else if (mod.GetType() == typeof(ReplaceRuleComponentModification)) {
                SelectiveDebug.LogJudge("ReplaceRuleComponentModification");

                if (!IsReplacingAnObjectAllowed())
                    throw new Exception("Replacing an object is not allowed at this point");
                
				ReplaceRuleComponentModification replaceMod = ((ReplaceRuleComponentModification) mod);

                // Set the base object to replacing object
                if (BaseObjectOfChanges == null || replaceMod.OldObject == BaseObjectOfChanges) {
                    BaseObjectOfChanges = replaceMod.NewObject;

                    if (BaseObjectOfChanges == null)
                        throw new Exception("BaseObjectOfChanges must not be null at this point");

                }

                // Once an object has been replaced and all values have been completed, it is the end of the turn
                if (AreAllValuesCompleted()) {
                    judgement = Judgement.Finished;
                } else {
                    judgement = Judgement.StillGoing;

                    // Once a rule component has been replaced and something still remains to be edited, only replacing is a valid option
                    newRuleAllowed = false;
                    removingRuleAllowed = false;
                    newStatementIsAllowed = false;
                    removingObjectIsAllowed = false;
                    swappingObjectsIsAllowed = false;

                }

            } else if (mod.GetType() == typeof(SwapRuleComponentsModification)) {
                SelectiveDebug.LogJudge("SwapRuleComponentsModification");

                if (!IsSwappingObjectsAllowed())
                    throw new Exception("Swapping an object is not allowed at this point");

                // Once objects has been swapped it is the end of the turn since they should both be complete
                judgement = Judgement.Finished;

			} else {
				Assert.Never("Unimplemented for modification: " + mod.GetType());
			}

		}

		public override bool AreAllValuesCompleted() {

            if (BaseObjectOfChanges == null)
                throw new Exception("AreAllValuesCompleted requires BaseObjectOfChanges to be not null");

            return ContainsNoNullOrVoidRecursive(BaseObjectOfChanges);
		}

		private bool ContainsNoNullOrVoidRecursive(RuleComponent obj) {
			
			if (obj is NullValue || obj is VoidStatement)
				return false;

			foreach (Argument a in obj.ArgumentList.argsByOrder) {

				if (ContainsNoNullOrVoidRecursive(a.reference.Instance()) == false)
					return false;

			}

			return true;
		}

		public override bool IsNewRuleAllowed() {

			// Once a player has finished their turn, they are no longer allowed to add new rules
			if (judgement == Judgement.Finished)
				return false;

			return newRuleAllowed;
		}

		public override bool IsRemovingARuleAllowed() {
			
			// Once a player has finished their turn, they are no longer allowed to remove rules
			if (judgement == Judgement.Finished)
				return false;
			
			return removingRuleAllowed;
		}

		public override bool IsNewStatementAllowed() {
			
			// Once a player has finished their turn, they are no longer allowed to add new statements
			if (judgement == Judgement.Finished)
				return false;
			
			return newStatementIsAllowed;
		}
		
		public override bool IsRemovingAnObjectAllowed() {
			
			// Once a player has finished their turn, they are no longer allowed to remove statements
			if (judgement == Judgement.Finished)
				return false;
			
			return removingObjectIsAllowed;
		}
		
		public override bool IsReplacingAnObjectAllowed() {
			
			// Once a player has finished their turn, they are no longer allowed to replace objects
			if (judgement == Judgement.Finished)
				return false;
			
			return replacingObjectIsAllowed;
		}
		
		public override bool IsSwappingObjectsAllowed() {
			
			// Once a player has finished their turn, they are no longer allowed to swap objects
			if (judgement == Judgement.Finished)
				return false;
			
			return swappingObjectsIsAllowed;
		}

		public override bool IsAccessingARule() {
			return AssociatedRule != null;
		}

		public override Rule GetAccessedRule() {
			return AssociatedRule;
		}

		public override bool IsOnlyOneStatementAccessible() {
			return BaseObjectOfChanges != null;
		}

		public override RuleComponent GetAccessibleRuleComponent() {
			return BaseObjectOfChanges;
		}

		public override bool AreBlockContainingStatementsAllowed() {
			return !HasAddedBlockStatement;
		}

		public override bool IsRuleComponentAllowed(Rule currentRule, RuleComponent objectInQuestion) {

			// Once a player has finished their turn, they are no longer allowed to edit values
			if (judgement == Judgement.Finished)
				return false;

			// Null and void values are always changeable
			if (objectInQuestion is VoidStatement || objectInQuestion is NullValue)
				return true;

			// Only values that are within the associated statement are accessible
			if (BaseObjectOfChanges != null) {
                
				// Otherwise return whether the value is an argument within the associated statement
				return SearchArgumentsRecursively(BaseObjectOfChanges, objectInQuestion);

			}

			// Only values within the associated rule are accessible
			if (AssociatedRule != null)
				return AssociatedRule == currentRule;

			return true;
		}

		private bool SearchArgumentsRecursively(RuleComponent potentialParent, RuleComponent potentialChild) {
			foreach (Argument a in potentialParent.ArgumentList.argsByOrder) {
				if (a.reference.Instance() == potentialChild)
					return true;
				if (SearchArgumentsRecursively(a.reference.Instance(), potentialChild))
					return true;
			}
			return false;
		}

		public override Judgement GetJudgement() {
			return judgement;
		}
		
		public override void ClearJudgement() {
            SelectiveDebug.LogJudge("ClearJudgement");

            judgement = Judgement.NothingChanged;
			newRuleAllowed = true;
			removingRuleAllowed = true;
			newStatementIsAllowed = true;
			removingObjectIsAllowed = true;
			swappingObjectsIsAllowed = true;
			replacingObjectIsAllowed = true;
			HasAddedBlockStatement = false;
			AssociatedRule = null;
            BaseObjectOfChanges = null;

		}

	}

}

