// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. 
    /// Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates. If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    //      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //          dependents("a") = {"b", "c"}
    //          dependents("b") = {"d"}
    //          dependents("c") = {}
    //          dependents("d") = {"d"}
    //          dependees("a") = {}
    //          dependees("b") = {"a"}
    //          dependees("c") = {"a"}
    //          dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // Sets up two dictionaries for the handling of Dependents and Dependees separately and effiecently
        Dictionary<string, HashSet<string>> dependents;
        Dictionary<string, HashSet<string>> dependees;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            // Initializes the three built-in variables
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            Size = 0;
        }

        /// <summary> 
        /// The number of ordered pairs in the DependencyGraph. 
        /// </summary> 
        public int Size
        {
            get; private set;
        }

        /// <summary> 
        /// The size of dependees(s).
        /// This property is an example of an indexer.  
        /// If dg is a DependencyGraph, you would invoke it like this: dg["a"]
        /// It should return the size of dependees("a") 
        /// </summary>
        /// <param name="s"> the dependent </param>
        /// <returns> the amount of dependees the dependent contains </returns>
        public int this[string s]
        {
            get
            {
                if (dependees.ContainsKey(s))
                    return dependees[s].Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty. 
        /// </summary>
        /// <param name="s"> The variable to check for dependents of </param>
        /// <returns> a bool variable </returns>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
                return dependents[s].Count > 0;
            else
                return false;
        }

        /// <summary> 
        /// Reports whether dependees(s) is non-empty. 
        /// </summary>
        /// <param name="s"> The variable to check for dependees of </param>
        /// <returns> a bool variable </returns>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s))
                return dependees[s].Count > 0;
            else
                return false;
        }

        /// <summary> 
        /// Enumerates dependents(s). 
        /// </summary>
        /// <param name="s"> The variable to get the dependents of </param>
        /// <returns> an IEnumerable of the dependents (an empty one if the key doesn't exist) </returns>
        public IEnumerable<string> GetDependents(string s)
        {
            if (dependents.ContainsKey(s))
                return dependents[s];
            else
                return new HashSet<string>();
        }

        /// <summary> 
        /// Enumerates dependees(s). 
        /// </summary>
        /// <param name="s"> The variable to get the dependees for </param>
        /// <returns> an IEnumerable of the dependents (an empty one if the key doesn't exist) </returns>
        public IEnumerable<string> GetDependees(string s)
        {
            if (dependees.ContainsKey(s))
                return dependees[s];
            else
                return new HashSet<string>();
        }

        /// <summary> 
        /// <para> Adds the ordered pair (s,t), if it doesn't exist </para>
        /// <para> This should be thought of as: </para> t depends on s 
        /// </summary>
        /// <param name="s"> s must be evaluated first. t depends on s </param>
        /// <param name="t"> t cannot be evaluated until s is </param>
        public void AddDependency(string s, string t)
        {
            HashSet<string> dependencies;

            if (!dependents.TryGetValue(s, out dependencies))
            {
                dependencies = new HashSet<String>();
                dependents.Add(s, dependencies);
            }
            if (dependencies.Add(t)) Size++;

            if (!dependees.TryGetValue(t, out dependencies))
            {
                dependencies = new HashSet<String>();
                dependees.Add(t, dependencies);
            }
            dependencies.Add(s);
        }

        /// <summary> 
        /// Removes the ordered pair (s,t), if it exists 
        /// </summary>
        /// <param name="s"> The dependee to remove </param>
        /// <param name="t"> The dependent to remove </param>
        public void RemoveDependency(string s, string t)
        {
            HashSet<string> dependencies;

            if (dependents.TryGetValue(s, out dependencies))
            {
                if (dependencies.Remove(t)) Size--;
            }
            if (dependees.TryGetValue(t, out dependencies))
            {
                dependencies.Remove(s);
            }

            if (dependents.ContainsKey(s) && dependents[s].Count == 0)
                dependents.Remove(s);

            if (dependees.ContainsKey(t) && dependees[t].Count == 0)
                dependees.Remove(t);
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  
        /// Then, for each t in newDependents, adds the ordered pair (s,t). 
        /// </summary>
        /// <param name="s"> the dependee of the dependents being replaced </param>
        /// <param name="newDependents"> the replacement dependent(s) </param>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            foreach (String r in GetDependents(s))
            {
                RemoveDependency(s, r);
            }
            foreach (String t in newDependents)
            {
                AddDependency(s, t);
            }
        }

        /// <summary> 
        /// Removes all existing ordered pairs of the form (r,s).  
        /// Then, for each t in newDependees, adds the ordered pair (t,s). 
        /// </summary>
        /// <param name="s"> the dependent of the dependees being replaced </param>
        /// <param name="newDependents"> the replacement dependee(s) </param>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            foreach (String r in GetDependees(s))
            {
                RemoveDependency(r, s);
            }
            foreach (String t in newDependees)
            {
                AddDependency(t, s);
            }
        }
    }
}