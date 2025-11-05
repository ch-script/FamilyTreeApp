using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FamilyTreeApp.Core.Models;

namespace FamilyTreeApp.Core.DataStructures // arbol genealogico de una familia
{
    public class FamilyTree
    {
        private Dictionary<string, Person> members;
        private List<string> rootIds;

        public FamilyTree()
        {
            members = new Dictionary<string, Person>();
            rootIds = new List<string>();
        }

        public bool AddPerson(Person person, string fatherId = null, string motherId = null)
        {
            if (members.ContainsKey(person.Id))
                return false;

            person.FatherId = fatherId;
            person.MotherId = motherId;

            if (!string.IsNullOrEmpty(fatherId) && members.ContainsKey(fatherId))
            {
                if (!members[fatherId].ChildrenIds.Contains(person.Id))
                    members[fatherId].ChildrenIds.Add(person.Id);
            }

            if (!string.IsNullOrEmpty(motherId) && members.ContainsKey(motherId))
            {
                if (!members[motherId].ChildrenIds.Contains(person.Id))
                    members[motherId].ChildrenIds.Add(person.Id);
            }

            members[person.Id] = person;

            // Si no tiene padres, es una raiz
            if (string.IsNullOrEmpty(fatherId) && string.IsNullOrEmpty(motherId))
            {
                rootIds.Add(person.Id);
            }

            return true;
        }

        // Obtiene una persona por su ID
        public Person GetPerson(string personId)
        {
            return members.ContainsKey(personId) ? members[personId] : null;
        }

        // Obtiene todas todas las personas
        public List<Person> GetAllMembers()
        {
            return members.Values.ToList();
        }

        // Obtiene los hijos de una persona
        public List<Person> GetChildren(string personId)
        {
            if (!members.ContainsKey(personId))
                return new List<Person>();

            return members[personId].ChildrenIds
                .Where(id => members.ContainsKey(id))
                .Select(id => members[id])
                .ToList();
        }

        // Obtiene los padres de una persona
        public List<Person> GetParents(string personId)
        {
            var parents = new List<Person>();

            if (!members.ContainsKey(personId))
                return parents;

            var person = members[personId];

            if (!string.IsNullOrEmpty(person.FatherId) && members.ContainsKey(person.FatherId))
                parents.Add(members[person.FatherId]);

            if (!string.IsNullOrEmpty(person.MotherId) && members.ContainsKey(person.MotherId))
                parents.Add(members[person.MotherId]);

            return parents;
        }

        // Obtiene las personas raiz del arbo
        public List<Person> GetRoots()
        {
            return rootIds
                .Where(id => members.ContainsKey(id))
                .Select(id => members[id])
                .ToList();
        }

        // Elimina una persona
        public bool RemovePerson(string personId)
        {
            if (!members.ContainsKey(personId))
                return false;

            var person = members[personId];

            // Remover referencias
            if (!string.IsNullOrEmpty(person.FatherId) && members.ContainsKey(person.FatherId))
            {
                members[person.FatherId].ChildrenIds.Remove(personId);
            }

            if (!string.IsNullOrEmpty(person.MotherId) && members.ContainsKey(person.MotherId))
            {
                members[person.MotherId].ChildrenIds.Remove(personId);
            }
            foreach (var childId in person.ChildrenIds)
            {
                if (members.ContainsKey(childId))
                {
                    var child = members[childId];
                    if (child.FatherId == personId)
                        child.FatherId = null;
                    if (child.MotherId == personId)
                        child.MotherId = null;
                }
            }

            rootIds.Remove(personId);
            members.Remove(personId);

            return true;
        }

        // Obtiene el numero d miembros
        public int Count => members.Count;
        // Ve si el arbol está vacio
        public bool IsEmpty => members.Count == 0;

        // Tipo de recorrido
        public List<Person> DepthFirstTraversal(string startPersonId)
        {
            var visited = new HashSet<string>();
            var result = new List<Person>();

            if (members.ContainsKey(startPersonId))
            {
                DFSHelper(startPersonId, visited, result);
            }

            return result;
        }

        private void DFSHelper(string personId, HashSet<string> visited, List<Person> result)
        {
            if (visited.Contains(personId) || !members.ContainsKey(personId))
                return;

            visited.Add(personId);
            result.Add(members[personId]);

            // Visitar hijos
            foreach (var childId in members[personId].ChildrenIds)
            {
                DFSHelper(childId, visited, result);
            }
        }
    }
}