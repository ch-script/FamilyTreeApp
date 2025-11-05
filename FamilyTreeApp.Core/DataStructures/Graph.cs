using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FamilyTreeApp.Core.Models;

namespace FamilyTreeApp.Core.DataStructures
{

    public class Graph
    {
        private Dictionary<string, GraphNode> nodes;
        private Dictionary<string, Dictionary<string, double>> adjacencyList;

        public Graph()
        {
            nodes = new Dictionary<string, GraphNode>();
            adjacencyList = new Dictionary<string, Dictionary<string, double>>();
        }

        public void AddNode(string personId, GeoCoordinates coordinates)
        {
            if (!nodes.ContainsKey(personId))
            {
                nodes[personId] = new GraphNode(personId, coordinates);
                adjacencyList[personId] = new Dictionary<string, double>();
            }
        }

        // arista con peso (distancia) entre dos nodos
        public void AddEdge(string personId1, string personId2, double distance)
        {
            if (!adjacencyList.ContainsKey(personId1) || !adjacencyList.ContainsKey(personId2))
                return;

            adjacencyList[personId1][personId2] = distance;
            adjacencyList[personId2][personId1] = distance;
        }

        // uso de formula de haverstine para distancias
        public static double CalculateDistance(GeoCoordinates coord1, GeoCoordinates coord2)
        {
            const double earthRadius = 6371;
            double lat1Rad = DegreesToRadians(coord1.Latitude);
            double lat2Rad = DegreesToRadians(coord2.Latitude);
            double deltaLat = DegreesToRadians(coord2.Latitude - coord1.Latitude);
            double deltaLon = DegreesToRadians(coord2.Longitude - coord1.Longitude);
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                      Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                      Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earthRadius * c;
            return distance;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        // Reconstruye ps
        public void RebuildEdges()
        {
            var nodeIds = nodes.Keys.ToList();

            foreach (var id1 in nodeIds)
            {
                adjacencyList[id1].Clear();

                foreach (var id2 in nodeIds)
                {
                    if (id1 != id2)
                    {
                        double distance = CalculateDistance(
                            nodes[id1].Coordinates,
                            nodes[id2].Coordinates
                        );
                        adjacencyList[id1][id2] = distance;
                    }
                }
            }
        }

        // obtiene todas las distancias desde un nodo
        public Dictionary<string, double> GetDistancesFrom(string personId)
        {
            if (!adjacencyList.ContainsKey(personId))
                return new Dictionary<string, double>();

            return new Dictionary<string, double>(adjacencyList[personId]);
        }
        // Trae todos los nodos
        public Dictionary<string, GraphNode> GetAllNodes()
        {
            return new Dictionary<string, GraphNode>(nodes);
        }

        // Elimina
        public void RemoveNode(string personId)
        {
            if (!nodes.ContainsKey(personId))
                return;

            nodes.Remove(personId);
            adjacencyList.Remove(personId);

            foreach (var neighbors in adjacencyList.Values)
            {
                neighbors.Remove(personId);
            }
        }

        // Obtiene distancias
        public double GetDistance(string personId1, string personId2)
        {
            if (adjacencyList.ContainsKey(personId1) &&
                adjacencyList[personId1].ContainsKey(personId2))
            {
                return adjacencyList[personId1][personId2];
            }
            return -1;
        }
    }


    // Estructura de nodo
    public class GraphNode
    {
        public string PersonId { get; set; }
        public GeoCoordinates Coordinates { get; set; }

        public GraphNode(string personId, GeoCoordinates coordinates)
        {
            PersonId = personId;
            Coordinates = coordinates;
        }
    }
}