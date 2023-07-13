using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace CGTest {

	public class Transform {

		Vector3 _localPosition;
		Vector3 _localScale = Vector3.One;
		Quaternion _localRotation = Quaternion.Identity;

		public Vector3 LocalPosition { get => _localPosition; set { MakeDirty(true); _localPosition=value; } }
		public Vector3 LocalScale { get => _localScale; set { MakeDirty(true); _localScale=value; } }
		public Quaternion LocalRotation { get => _localRotation; set { MakeDirty(true); _localRotation=value; } }
		public Vector3 LocalRotationEuler {
			get => LocalRotation.ToEulerAngles();
			set => LocalRotation=new Quaternion(value);
		}

		public Vector3 WorldPosition => LocalToWorldMatrix.ExtractTranslation();
		public Vector3 WorldScale => LocalToWorldMatrix.ExtractScale();
		public Quaternion WorldRotation => LocalToWorldMatrix.ExtractRotation();
		public Vector3 WorldRotationEuler => WorldRotation.ToEulerAngles();

		public Vector3 Forward => (new Vector4(1,0,0,0)*LocalToWorldMatrix).Xyz;

		Transform _parent;
		public Transform parent {
			get => _parent;
			set {
				//判断合法性
				bool canChangeParent = true;
				for(Transform i = value;i!=null;i=i._parent)
					if(i==this) { canChangeParent=false; break; }
				if(!canChangeParent) return;

				MakeDirty(false);

				//移除现有
				if(_parent!=null) {
					_parent.children.Remove(this);
					_parent=null;
				}

				//更改父节点
				_parent=value;
				value.children.Add(this);

			}
		}
		List<Transform> children = new List<Transform>();

		public Transform GetChildren(int index) => index<children.Count ? null : children[index];
		public int ChildrenCount => children.Count;

		Matrix4 _worldModel = Matrix4.Identity;
		Matrix4 _localModel = Matrix4.Identity;

		bool isWorldDirty;
		bool isLocalDirty;
		void MakeDirty(bool localDirty) {
			if(localDirty) isLocalDirty=true;
			isWorldDirty=true;
			foreach(var i in children) {
				if(!i.isWorldDirty) i.MakeDirty(false);
			}
		}

		public Matrix4 LocalToWorldMatrix {
			get {
				if(!isWorldDirty&&!isLocalDirty) return _worldModel;
				isWorldDirty=false;

				Matrix4 parentModel = Matrix4.Identity;
				if(parent!=null) parentModel=parent.LocalToWorldMatrix;
				_worldModel=LocalToParentMatrix*parentModel;
				return _worldModel;
			}
		}
		public Matrix4 LocalToParentMatrix {
			get {
				if(!isLocalDirty) return _localModel;
				isLocalDirty=false;

				_localModel=Matrix4.Identity;
				_localModel*=Matrix4.CreateScale(LocalScale);
				_localModel*=Matrix4.CreateFromQuaternion(LocalRotation);
				_localModel*=Matrix4.CreateTranslation(LocalPosition);

				return _localModel;
			}
		}

		HashSet<Component> components = new HashSet<Component>();
		public void AddComponent(Component newComponent) {
			components.Add(newComponent);
			newComponent.SetTransform(this);
		}

		public T GetComponent<T>() where T : Component {
			foreach(var i in components) if(i is T) return i as T;
			return null;
		}

		public void RemoveComponent(Component toRemove) {
			components.Remove(toRemove);
		}
	}

}