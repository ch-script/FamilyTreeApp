using Xunit;
using FamilyTreeApp.Core.DataStructures;
using FamilyTreeApp.Core.Models;
using System;
using System.Linq;

namespace FamilyTreeApp.Tests.DataStructures
{
    public class GraphTests
    {
        // ============================================
        // PRUEBAS DE AGREGAR NODOS
        // ============================================

        [Fact]
        public void AddNode_WithNewPersonId_AddsNode()
        {
            // Arrange
            var graph = new Graph();
            var coords = new GeoCoordinates(9.9281, -84.0907, "San José, Costa Rica");

            // Act
            graph.AddNode("person-1", coords);

            // Assert
            var nodes = graph.GetAllNodes();
            Assert.Single(nodes);
            Assert.True(nodes.ContainsKey("person-1"));
        }

        [Fact]
        public void AddNode_WithDuplicateId_DoesNotDuplicate()
        {
            // Arrange
            var graph = new Graph();
            var coords1 = new GeoCoordinates(9.9281, -84.0907);
            var coords2 = new GeoCoordinates(10.0, -85.0);

            // Act
            graph.AddNode("person-1", coords1);
            graph.AddNode("person-1", coords2); // Intento de duplicar

            // Assert
            var nodes = graph.GetAllNodes();
            Assert.Single(nodes);
        }

        [Fact]
        public void AddNode_CreatesAdjacencyList()
        {
            // Arrange
            var graph = new Graph();
            var coords = new GeoCoordinates(9.9281, -84.0907);

            // Act
            graph.AddNode("person-1", coords);
            var distances = graph.GetDistancesFrom("person-1");

            // Assert
            Assert.NotNull(distances);
            Assert.Empty(distances); // No tiene vecinos aún
        }

        // ============================================
        // PRUEBAS DE AGREGAR ARISTAS
        // ============================================

        [Fact]
        public void AddEdge_BetweenExistingNodes_AddsEdge()
        {
            // Arrange
            var graph = new Graph();
            var coords1 = new GeoCoordinates(9.9281, -84.0907);
            var coords2 = new GeoCoordinates(10.0, -85.0);

            graph.AddNode("person-1", coords1);
            graph.AddNode("person-2", coords2);

            // Act
            graph.AddEdge("person-1", "person-2", 50.5);

            // Assert
            double distance = graph.GetDistance("person-1", "person-2");
            Assert.Equal(50.5, distance);
        }

        [Fact]
        public void AddEdge_WithNonExistentNode_DoesNotAdd()
        {
            // Arrange
            var graph = new Graph();
            var coords = new GeoCoordinates(9.9281, -84.0907);
            graph.AddNode("person-1", coords);

            // Act
            graph.AddEdge("person-1", "person-nonexistent", 100.0);

            // Assert
            var distances = graph.GetDistancesFrom("person-1");
            Assert.Empty(distances);
        }

        [Fact]
        public void AddEdge_CreatesSymmetricEdge()
        {
            // Arrange
            var graph = new Graph();
            var coords1 = new GeoCoordinates(9.9281, -84.0907);
            var coords2 = new GeoCoordinates(10.0, -85.0);

            graph.AddNode("person-1", coords1);
            graph.AddNode("person-2", coords2);

            // Act
            graph.AddEdge("person-1", "person-2", 75.3);

            // Assert
            double distance1to2 = graph.GetDistance("person-1", "person-2");
            double distance2to1 = graph.GetDistance("person-2", "person-1");
            Assert.Equal(75.3, distance1to2);
            Assert.Equal(75.3, distance2to1);
        }

        // ============================================
        // PRUEBAS DE CÁLCULO DE DISTANCIAS
        // ============================================

        [Fact]
        public void CalculateDistance_WithSameCoordinates_ReturnsZero()
        {
            // Arrange
            var coord1 = new GeoCoordinates(9.9281, -84.0907);
            var coord2 = new GeoCoordinates(9.9281, -84.0907);

            // Act
            double distance = Graph.CalculateDistance(coord1, coord2);

            // Assert
            Assert.Equal(0, distance, precision: 2);
        }

        [Fact]
        public void CalculateDistance_WithKnownCoordinates_ReturnsExpectedDistance()
        {
            // Arrange
            // San José, Costa Rica a Cartago, Costa Rica (~20 km)
            var sanJose = new GeoCoordinates(9.9281, -84.0907);
            var cartago = new GeoCoordinates(9.8644, -83.9186);

            // Act
            double distance = Graph.CalculateDistance(sanJose, cartago);

            // Assert
            // La distancia real es aproximadamente 18-22 km
            Assert.True(distance > 15 && distance < 30);
        }

        // ============================================
        // PRUEBAS DE OBTENER DISTANCIAS
        // ============================================

        [Fact]
        public void GetDistancesFrom_WithValidId_ReturnsAllDistances()
        {
            // Arrange
            var graph = new Graph();
            graph.AddNode("person-1", new GeoCoordinates(9.9281, -84.0907));
            graph.AddNode("person-2", new GeoCoordinates(10.0, -85.0));
            graph.AddNode("person-3", new GeoCoordinates(10.5, -84.5));
            graph.RebuildEdges();

            // Act
            var distances = graph.GetDistancesFrom("person-1");

            // Assert
            Assert.Equal(2, distances.Count); // Debería tener 2 vecinos
            Assert.True(distances.ContainsKey("person-2"));
            Assert.True(distances.ContainsKey("person-3"));
        }

        [Fact]
        public void GetDistancesFrom_WithInvalidId_ReturnsEmptyDictionary()
        {
            // Arrange
            var graph = new Graph();

            // Act
            var distances = graph.GetDistancesFrom("invalid-id");

            // Assert
            Assert.NotNull(distances);
            Assert.Empty(distances);
        }

        [Fact]
        public void GetDistance_BetweenNonConnectedNodes_ReturnsMinusOne()
        {
            // Arrange
            var graph = new Graph();
            graph.AddNode("person-1", new GeoCoordinates(9.9281, -84.0907));
            graph.AddNode("person-2", new GeoCoordinates(10.0, -85.0));
            // No se agrega arista ni se reconstruyen

            // Act
            double distance = graph.GetDistance("person-1", "person-2");

            // Assert
            Assert.Equal(-1, distance);
        }

        // ============================================
        // PRUEBAS DE ELIMINAR NODOS
        // ============================================

        [Fact]
        public void RemoveNode_WithValidId_RemovesNode()
        {
            // Arrange
            var graph = new Graph();
            graph.AddNode("person-1", new GeoCoordinates(9.9281, -84.0907));
            graph.AddNode("person-2", new GeoCoordinates(10.0, -85.0));

            // Act
            graph.RemoveNode("person-1");

            // Assert
            var nodes = graph.GetAllNodes();
            Assert.Single(nodes);
            Assert.False(nodes.ContainsKey("person-1"));
        }

        [Fact]
        public void RemoveNode_RemovesFromAllAdjacencyLists()
        {
            // Arrange
            var graph = new Graph();
            graph.AddNode("person-1", new GeoCoordinates(9.9281, -84.0907));
            graph.AddNode("person-2", new GeoCoordinates(10.0, -85.0));
            graph.AddNode("person-3", new GeoCoordinates(10.5, -84.5));
            graph.RebuildEdges();

            // Act
            graph.RemoveNode("person-2");

            // Assert
            var distancesFrom1 = graph.GetDistancesFrom("person-1");
            var distancesFrom3 = graph.GetDistancesFrom("person-3");
            
            Assert.DoesNotContain("person-2", distancesFrom1.Keys);
            Assert.DoesNotContain("person-2", distancesFrom3.Keys);
        }
    }
}