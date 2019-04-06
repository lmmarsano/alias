using SCG = System.Collections.Generic;
using System.Linq;

namespace Alias.ConfigurationData {
	/**
	 * <summary>
	 * Operation to fold over a recursive data structure containing <c cref='SCG.IEnumerable{T}'>enumerables over T</c>.
	 * </summary>
	 * <param name="container">Element to replace</param>
	 * <param name="sequence">Replacement children for the element</param>
	 * <typeparam name="T">Recursive data type</typeparam>
	 * <returns>Replacement value</returns>
	 */
	public delegate T Factory<T>(T container, SCG.IEnumerable<T> sequence);
	/**
	 * <summary>
	 * Predicate for filtering items of an <c cref='SCG.IEnumerable{T}'>enumerable</c>.
	 * </summary>
	 * <param name="item">Candidate for inclusion</param>
	 * <typeparam name="T">Recursive data type</typeparam>
	 * <returns>True for inclusion</returns>
	 */
	public delegate bool Filter<T>(T item);
	/**
	 * <summary>
	 * Instances that can decompose, filter, and recompose a recursively enumerable data structure.
	 * </summary>
	 * <typeparam name="T">Recursive data type</typeparam>
	 */
	public class Pruner<T> {
		/**
		 * <summary>
		 * Function for recomposing the data structure.
		 * </summary>
		 * <value></value>
		 */
		public Factory<T> Factory { get; }
		/**
		 * <summary>
		 * Predicate for filtering enumerables in the data structure before recomposing.
		 * </summary>
		 * <value></value>
		 */
		public Filter<T> Filter { get; }
		/**
		 * <summary>
		 * Instantiate an object that recursively filters with <c cref='filter>filter</c> and rebuilds with <c cref='factory'>factory</c>.
		 * </summary>
		 * <param name="factory">Function to rebuild an item from the original and its replacement children</param>
		 * <param name="filter">Function to recursively filter enumerables.</param>
		 */
		public Pruner(Factory<T> factory, Filter<T> filter) {
			Factory = factory;
			Filter = filter;
		}
		/**
		 * <summary>
		 * Recursively filter and rebuild data structure.
		 * </summary>
		 * <param name="item">Data structure to recursively filter and rebuild</param>
		 * <returns>Replacement data structure</returns>
		 */
		public T Transform(T item)
		=> item is SCG.IEnumerable<T> enumerable
		 ? Factory
		   ( item
		   , enumerable.Select(Transform).Where(x => Filter(x))
		   )
		 : item;
	}
}
