using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FamilyTreeApp.Core.Models;
using FamilyTreeApp.Core.DataStructures;

namespace FamilyTreeApp.Core.Services
{
    public class FamilyTreeService
    {
        private FamilyTree familyTree;
        private Graph locationGraph;

        public FamilyTreeService()
        {
            familyTree = new FamilyTree();
            locationGraph = new Graph();
        }
        public bool AddPerson(Person person, string fatherId = null, string motherId = null)
        {
            if (familyTree.AddPerson(person, fatherId, motherId))
            {
                locationGraph.AddNode(person.Id, person.Residence);
                locationGraph.RebuildEdges();
                return true;
            }
            return false;
        }
        public bool UpdatePerson(Person updatedPerson) // Upgradea los datos y el grafo
        {
            var existingPerson = familyTree.GetPerson(updatedPerson.Id);
            if (existingPerson == null)
                return false;
            existingPerson.FullName = updatedPerson.FullName;
            existingPerson.IdNumber = updatedPerson.IdNumber;
            existingPerson.BirthDate = updatedPerson.BirthDate;
            existingPerson.Age = updatedPerson.Age;
            existingPerson.IsAlive = updatedPerson.IsAlive;
            existingPerson.PhotoPath = updatedPerson.PhotoPath;
            existingPerson.Residence = updatedPerson.Residence;

            locationGraph.RemoveNode(existingPerson.Id);
            locationGraph.AddNode(existingPerson.Id, existingPerson.Residence);
            locationGraph.RebuildEdges();

            return true;
        }

        // Elimina una persona de to
        public bool RemovePerson(string personId)
        {
            locationGraph.RemoveNode(personId);
            return familyTree.RemovePerson(personId);
        }

        // Obtiene una persona (hay q mandar id)
        public Person GetPerson(string personId)
        {
            return familyTree.GetPerson(personId);
        }

        // Obtiene todos los miembros
        public List<Person> GetAllMembers()
        {
            return familyTree.GetAllMembers();
        }

        // distancias
        public Dictionary<string, double> GetDistancesFrom(string personId)
        {
            return locationGraph.GetDistancesFrom(personId);
        }

        public FamilyStatistics GetStatistics()
        {
            var stats = new FamilyStatistics();
            var members = familyTree.GetAllMembers();

            if (members.Count < 2)
                return stats;

            double maxDistance = double.MinValue;
            double minDistance = double.MaxValue;
            double totalDistance = 0;
            int pairCount = 0;

            for (int i = 0; i < members.Count; i++)
            {
                for (int j = i + 1; j < members.Count; j++)
                {
                    double distance = Graph.CalculateDistance(
                        members[i].Residence,
                        members[j].Residence
                    );

                    totalDistance += distance;
                    pairCount++;

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        stats.FarthestPair = (members[i], members[j]);
                        stats.MaxDistance = distance;
                    }

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        stats.ClosestPair = (members[i], members[j]);
                        stats.MinDistance = distance;
                    }
                }
            }

            stats.AverageDistance = pairCount > 0 ? totalDistance / pairCount : 0;

            return stats;
        }

        public List<Person> GetChildren(string personId)
        {
            return familyTree.GetChildren(personId);
        }


        public List<Person> GetParents(string personId)
        {
            return familyTree.GetParents(personId);
        }

        public List<Person> GetRoots()
        {
            return familyTree.GetRoots();
        }

        public int GetMemberCount()
        {
            return familyTree.Count;
        }

        public List<Person> GetSiblings(string personId)
        {
            return familyTree.GetSiblings(personId);
        }

        public Person GetSpouse(string personId)
        {
            return familyTree.GetSpouse(personId);
        }

        public bool SetMarriage(string personId1, string personId2)
        {
            return familyTree.SetMarriage(personId1, personId2);
        }

        public bool RemoveMarriage(string personId)
        {
            return familyTree.RemoveMarriage(personId);
        }

        public List<Person> GetGrandparents(string personId)
        {
            return familyTree.GetGrandparents(personId);
        }

        public List<Person> GetGrandchildren(string personId)
        {
            return familyTree.GetGrandchildren(personId);
        }

        public List<Person> GetUnclesAndAunts(string personId)
        {
            return familyTree.GetUnclesAndAunts(personId);
        }

        public List<Person> GetNephewsAndNieces(string personId)
        {
            return familyTree.GetNephewsAndNieces(personId);
        }

        public List<Person> GetCousins(string personId)
        {
            return familyTree.GetCousins(personId);
        }
    }

    public class FamilyStatistics
    {
        // par lejano
        public (Person Person1, Person Person2) FarthestPair { get; set; }
        public double MaxDistance { get; set; }

        // Par de familiares ma cercano
        public (Person Person1, Person Person2) ClosestPair { get; set; }

        public double MinDistance { get; set; }

        //Distancia promedio entre todos los pares de familiares
        public double AverageDistance { get; set; }
    }
}