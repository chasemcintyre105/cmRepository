using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public abstract class DragAndDropEventable<Draggable, Droppable> {

		public bool AlwaysDrop = false;
		public bool IsDragging { get; private set; }
		private bool HasDroppable;

		protected Draggable draggable;
		protected Droppable droppable;

		protected DragAndDropActionable draggableActionable;
		protected DragAndDropActionable droppableActionable;
        
		protected abstract bool CanDrag(Draggable draggable);
		protected abstract void StartedDragging();
		protected abstract void StoppedDragging();
		protected abstract void Dropped();
		protected abstract bool IsDropPermitted();

		public DragAndDropEventable() {
			IsDragging = false;
			HasDroppable = false;
		}

		public void StartDragging(Draggable draggable, DragAndDropActionable draggableActionable, GameObject draggableGameObject) {
			if (!IsDragging && CanDrag(draggable)) {
				SelectiveDebug.LogDragDrop("StartDragging");

				IsDragging = true;

				this.draggable = draggable;
				this.draggableActionable = draggableActionable;

				draggableActionable.Hover();

				StartedDragging();
                
			}
		}
		
		public void CancelDragging() {
			if (IsDragging && CanDrag(draggable)) {
				SelectiveDebug.LogDragDrop("CancelDragging");
				draggableActionable.Unhover();

				StoppedDragging();

				IsDragging = false;
				HasDroppable = false;
                
			}
		}
		
		public void StopDragging() {
			if (IsDragging && CanDrag(draggable)) {
				draggableActionable.Unhover();

				StoppedDragging();

				bool permitted = IsDropPermitted();
				SelectiveDebug.LogDragDrop("StopDragging && " + ((permitted) ? "" : "!") + "DropPermitted");

				if (HasDroppable && (AlwaysDrop || permitted)) {
					droppableActionable.Unhover();
					Dropped();
				}

				HasDroppable = false;
				IsDragging = false;
                
			}
		}
		
		public void MouseEnteredDroppableObject(Droppable droppable, DragAndDropActionable droppableActionable) {
			if (IsDragging) {
				this.droppable = droppable;
				this.droppableActionable = droppableActionable;
				HasDroppable = true;

				droppableActionable.Hover();

			}
		} 
		
	}
}

