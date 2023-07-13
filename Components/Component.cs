using System;
using System.Collections.Generic;
using System.Text;

namespace CGTest {
	public class Component:IComparable {

		public static SortedSet<Component> updateList;
		static Component() {
			updateList=new SortedSet<Component>(Comparer<Component>.Create(
				(x,y) => {
					int res = x.executionOrder-y.executionOrder;
					if(res!=0) return res;
					return x.CompareTo(y);
				}
			));
		}

	protected virtual int executionOrder => 0;

	Guid guid;

	public Transform transform { get; private set; }
	public void SetTransform(Transform transform) {
		isEnabled=true;
		if(this.transform==null) this.transform=transform;
	}
	public void RemoveFromTransform() {
		isEnabled=false;
		transform=null;
	}

	bool _isEnabled;
	public bool isEnabled {
		get => _isEnabled;
		private set {
			if(_isEnabled==value) return;
			_isEnabled=value;
			if(value) Enable();
			else Disable();
		}
	}
	protected virtual void Enable() {
		if(guid==Guid.Empty) guid=Guid.NewGuid();
		updateList.Add(this);
	}
	protected virtual void Disable() {
		updateList.Remove(this);
	}

	public virtual void Update(float deltaTime) { }

	public int CompareTo(object obj) {
		if(!(obj is Component)) return -1;
		return guid.CompareTo((obj as Component).guid);
	}
}
}
