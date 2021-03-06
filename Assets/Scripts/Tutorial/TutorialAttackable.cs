﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Extension;

namespace Tutorial {
	public class TutorialAttackable : MonoBehaviour {
		public TutorialUnit unit;
		public List<GameObject> targets;
		public List<GameObject> removeList;
		public TutorialAttackManager attackManager;
		public bool canExamineArea;
		public bool isOrderedToMove;

		void Start() {
			this.unit= this.GetComponent<TutorialUnit>();
			this.targets = new List<GameObject>();
			this.removeList = new List<GameObject>();
			this.canExamineArea = false;
			this.isOrderedToMove = false;
			if (this.attackManager == null) {
				Debug.LogError("Cannot find attack manager.");
			}
		}

		void Update() {
			if (this.canExamineArea && this.isOrderedToMove) {
				this.isOrderedToMove = false;
			}
			if (this.canExamineArea) {
				ExamineArea(this.unit.attackRadius);
			}
			else if (this.isOrderedToMove) {
				NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
				if (agent.ReachedDestination()) {
					this.canExamineArea = true;
					this.isOrderedToMove = false;
				}
			}
			Attack();
		}

		private void ExamineArea(float radius) {
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius);
			if (colliders.Length > 0) {
				foreach (Collider col in colliders) {
					GameObject obj = col.gameObject;
					if (obj.name.Equals("Floor")) {
						continue;
					}
					if (obj.tag.Equals("Tutorial_Unit")) {
						if (!this.targets.Contains(obj)) {
							this.targets.Add(obj);
						}
					}
				}
			}
		}

		private void Attack() {
			if (this.targets.Count > 0) {
				NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
				agent.SetDestination(findStoppingPoint(this.targets[0]));
			}
		}

		private Vector3 findStoppingPoint(GameObject b) {
			Vector3 start = this.transform.position;
			Vector3 end = b.transform.position;
			Vector3 unitVector = (end - start).normalized;
			return (this.transform.position - ((getRadius(this.gameObject) + this.unit.attackRadius)) * unitVector);
		}

		private float getRadius(GameObject obj) {
			Renderer renderer = obj.GetComponent<Renderer>();
			return Vector3.Distance(obj.transform.position, (obj.transform.position) + (renderer.bounds.size / 2f));
		}
	}
}
