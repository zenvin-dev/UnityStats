using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.Stats {
	/// <summary>
	/// Component for holding stat values of a single entity.
	/// </summary>
	[DisallowMultipleComponent]
	public class StatContainer : MonoBehaviour {
		private Dictionary<Stat, StatInstance> statDict;

		private List<StatInstance> stats = new List<StatInstance> ();


		/// <summary>
		/// Attempts to retrieve the instance associated with a given <see cref="Stat{T}"/>.
		/// </summary>
		/// <typeparam name="TValue"> The type of value that should be wrapped by both <paramref name="stat"/> and <paramref name="instance"/>. </typeparam>
		/// <param name="stat"> The stat whose associated instance to look for. </param>
		/// <param name="instance">
		/// The instance associated with <paramref name="stat"/> (if any)
		/// as a not further defined implementation of <see cref="StatInstance{T}"/>, where T is <typeparamref name="TValue"/>.
		/// </param>
		public bool TryGet<TValue> (Stat<TValue> stat, out StatInstance<TValue> instance) {
			return TryGet<StatInstance<TValue>, Stat<TValue>, TValue> (stat, out instance);
		}

		/// <summary>
		/// Attempts to retrieve the instance associated with a given <see cref="Stat{T}"/>.
		/// </summary>
		/// <typeparam name="TStatInstance"> The specific type of <see cref="StatInstance{T}"/> to look for. </typeparam>
		/// <typeparam name="TValue"> The type of value that should be wrapped by both <paramref name="stat"/> and <paramref name="instance"/>. </typeparam>
		/// <param name="stat"> The stat whose associated instance to look for. </param>
		/// <param name="instance"> The instance associated with <paramref name="stat"/> (if any). </param>
		public bool TryGet<TStatInstance, TValue> (Stat<TValue> stat, out TStatInstance instance)
			where TStatInstance : StatInstance<TValue> {

			return TryGet<TStatInstance, Stat<TValue>, TValue> (stat, out instance);
		}

		/// <summary>
		/// Attempts to retrieve the instance associated with a given <see cref="Stat{T}"/>.
		/// </summary>
		/// <typeparam name="TStatInstance"> The specific type of <see cref="StatInstance{T}"/> to look for. </typeparam>
		/// <typeparam name="TStat"> The specific type of <see cref="Stat{T}"/> to use as input. </typeparam>
		/// <typeparam name="TValue"> The type of value that should be wrapped by both <paramref name="stat"/> and <paramref name="instance"/>. </typeparam>
		/// <param name="stat"> The stat whose associated instance to look for. </param>
		/// <param name="instance"> The instance associated with <paramref name="stat"/> (if any). </param>
		public bool TryGet<TStatInstance, TStat, TValue> (TStat stat, out TStatInstance instance)
			where TStatInstance : StatInstance<TValue>
			where TStat : Stat<TValue> {

			InitStatDict ();
			instance = null;
			if (statDict.TryGetValue (stat, out var ins) && ins is TStatInstance tIns) {
				instance = tIns;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Performs a linear lookup to retrieve the instance associated with a stat by the stat's <see cref="Stat.Identifier"/>.
		/// </summary>
		/// <typeparam name="TStatInstance"> The specific type of <see cref="StatInstance{T}"/> to look for. </typeparam>
		/// <param name="identifier"> A stat identifier to look for. </param>
		/// <param name="instance"> The instance associated with the found stat (if any). </param>
		public bool TryGet<TStatInstance> (string identifier, out TStatInstance instance)
			where TStatInstance : StatInstance {

			InitStatDict ();
			instance = null;
			foreach (var ins in statDict) {
				if (ins.Key.Identifier == identifier && ins.Value is TStatInstance tIns) {
					instance = tIns;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Retrieves the instance associated with a given <see cref="Stat{T}"/>.
		/// </summary>
		/// <typeparam name="TValue"> The type of value that should be wrapped by both <paramref name="stat"/> and the returned instance. </typeparam>
		/// <param name="stat"> The stat whose associated instance to look for. </param>
		public StatInstance<TValue> Get<TValue> (Stat<TValue> stat) {
			return TryGet (stat, out var ins) ? ins : null;
		}

		/// <summary>
		/// Retrieves the instance associated with a given <see cref="Stat{T}"/>.
		/// </summary>
		/// <typeparam name="TStatInstance"> The specific type of <see cref="StatInstance{T}"/> to look for. </typeparam>
		/// <typeparam name="TValue"> The type of value that should be wrapped by both <paramref name="stat"/> and the returned instance. </typeparam>
		/// <param name="stat"> The stat whose associated instance to look for. </param>
		public TStatInstance Get<TStatInstance, TValue> (Stat<TValue> stat)
			where TStatInstance : StatInstance<TValue> {

			return TryGet<TStatInstance, TValue> (stat, out var val) ? val : null;
		}

		/// <summary>
		/// Retrieves the instance associated with a given <see cref="Stat{T}"/>.
		/// </summary>
		/// <typeparam name="TStatInstance"> The specific type of <see cref="StatInstance{T}"/> to look for. </typeparam>
		/// <typeparam name="TStat"> The specific type of <see cref="Stat{T}"/> to look for. </typeparam>
		/// <typeparam name="TValue"> The type of value that should be wrapped by both <paramref name="stat"/> and the returned instance. </typeparam>
		/// <param name="stat"> The stat whose associated instance to look for. </param>
		public TStatInstance Get<TStatInstance, TStat, TValue> (TStat stat)
			where TStatInstance : StatInstance<TValue>
			where TStat : Stat<TValue> {

			return TryGet<TStatInstance, TStat, TValue> (stat, out var val) ? val : null;
		}

		/// <summary>
		/// Performs a linear lookup to retrieve the instance associated with a stat by the stat's <see cref="Stat.Identifier"/>.
		/// </summary>
		/// <typeparam name="TStatInstance"> The specific type of <see cref="StatInstance{T}"/> to look for. </typeparam>
		/// <param name="identifier"> A stat identifier to look for. </param>
		public TStatInstance Get<TStatInstance> (string identifier)
			where TStatInstance : StatInstance {

			return TryGet<TStatInstance> (identifier, out var val) ? val : null;
		}


		private void InitStatDict () {
			if (statDict != null)
				return;

			statDict = new Dictionary<Stat, StatInstance> ();
			foreach (var statInstance in stats) {
				var stat = statInstance.GetStat ();
				if (statInstance.IsValid && !statDict.ContainsKey (stat)) {
					statDict.Add (stat, statInstance);
				}
			}
		}
	}
}
