using Xunit;
using FamilyTreeApp.Core.Services;
using FamilyTreeApp.Core.Models;
using System;

namespace FamilyTreeApp.Tests.Services
{
    public class FamilyTreeServiceTests
    {
        // ============================================
        // PRUEBAS DE AGREGAR PERSONAS
        // ============================================

        [Fact]
        public void AddPerson_WithValidData_AddsToTreeAndGraph()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person = new Person
            {
                FullName = "Juan Pérez",
                IdNumber = "123456789",
                BirthDate = new DateTime(1990, 5, 15),
                Residence = new GeoCoordinates(9.9281, -84.0907, "San José")
            };

            // Act
            bool result = service.AddPerson(person);

            // Assert
            Assert.True(result);
            Assert.Equal(1, service.GetMemberCount());
            
            var retrievedPerson = service.GetPerson(person.Id);
            Assert.NotNull(retrievedPerson);
            Assert.Equal("Juan Pérez", retrievedPerson.FullName);
        }

        [Fact]
        public void AddPerson_WithDuplicateId_ReturnsFalse()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person1 = new Person
            {
                FullName = "María López",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            var person2 = new Person
            {
                Id = person1.Id, // Mismo ID
                FullName = "Ana García",
                Residence = new GeoCoordinates(10.0, -85.0)
            };

            // Act
            service.AddPerson(person1);
            bool result = service.AddPerson(person2);

            // Assert
            Assert.False(result);
            Assert.Equal(1, service.GetMemberCount());
        }

        [Fact]
        public void AddPerson_UpdatesGraphEdges()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person1 = new Person
            {
                FullName = "Persona 1",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            var person2 = new Person
            {
                FullName = "Persona 2",
                Residence = new GeoCoordinates(10.0, -85.0)
            };

            // Act
            service.AddPerson(person1);
            service.AddPerson(person2);
            var distances = service.GetDistancesFrom(person1.Id);

            // Assert
            Assert.Single(distances);
            Assert.True(distances.ContainsKey(person2.Id));
            Assert.True(distances[person2.Id] > 0);
        }

        // ============================================
        // PRUEBAS DE ACTUALIZAR PERSONAS
        // ============================================

        [Fact]
        public void UpdatePerson_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person = new Person
            {
                FullName = "Carlos Ramírez",
                IdNumber = "111222333",
                BirthDate = new DateTime(1985, 3, 10),
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            service.AddPerson(person);

            // Act
            var updatedPerson = new Person
            {
                Id = person.Id,
                FullName = "Carlos Alberto Ramírez",
                IdNumber = "111222333",
                BirthDate = new DateTime(1985, 3, 10),
                Age = 39,
                IsAlive = true,
                PhotoPath = "/path/to/photo.jpg",
                Residence = new GeoCoordinates(10.0, -85.0, "Nuevo lugar")
            };
            bool result = service.UpdatePerson(updatedPerson);

            // Assert
            Assert.True(result);
            var retrieved = service.GetPerson(person.Id);
            Assert.Equal("Carlos Alberto Ramírez", retrieved.FullName);
            Assert.Equal(10.0, retrieved.Residence.Latitude);
        }

        [Fact]
        public void UpdatePerson_WithNonExistentPerson_ReturnsFalse()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person = new Person
            {
                Id = "non-existent-id",
                FullName = "Fantasma",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };

            // Act
            bool result = service.UpdatePerson(person);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdatePerson_RecalculatesGraphEdges()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person1 = new Person
            {
                FullName = "P1",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            var person2 = new Person
            {
                FullName = "P2",
                Residence = new GeoCoordinates(10.0, -85.0)
            };
            service.AddPerson(person1);
            service.AddPerson(person2);

            var oldDistances = service.GetDistancesFrom(person1.Id);
            double oldDistance = oldDistances[person2.Id];

            // Act - Mover person1 mucho más lejos
            var updatedPerson1 = new Person
            {
                Id = person1.Id,
                FullName = person1.FullName,
                IdNumber = person1.IdNumber,
                BirthDate = person1.BirthDate,
                Age = person1.Age,
                IsAlive = person1.IsAlive,
                PhotoPath = person1.PhotoPath,
                Residence = new GeoCoordinates(15.0, -90.0) // Muy lejos
            };
            service.UpdatePerson(updatedPerson1);

            var newDistances = service.GetDistancesFrom(person1.Id);
            double newDistance = newDistances[person2.Id];

            // Assert
            Assert.NotEqual(oldDistance, newDistance);
            Assert.True(newDistance > oldDistance);
        }

        // ============================================
        // PRUEBAS DE ELIMINAR PERSONAS
        // ============================================

        [Fact]
        public void RemovePerson_RemovesFromTreeAndGraph()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person = new Person
            {
                FullName = "Para Eliminar",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            service.AddPerson(person);

            // Act
            bool result = service.RemovePerson(person.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(0, service.GetMemberCount());
            Assert.Null(service.GetPerson(person.Id));
        }

        [Fact]
        public void RemovePerson_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var service = new FamilyTreeService();

            // Act
            bool result = service.RemovePerson("invalid-id-xyz");

            // Assert
            Assert.False(result);
        }

        // ============================================
        // PRUEBAS DE OBTENER INFORMACIÓN
        // ============================================

        [Fact]
        public void GetPerson_WithValidId_ReturnsPerson()
        {
            // Arrange
            var service = new FamilyTreeService();
            var person = new Person
            {
                FullName = "Elena Morales",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            service.AddPerson(person);

            // Act
            var result = service.GetPerson(person.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal("Elena Morales", result.FullName);
        }

        [Fact]
        public void GetAllMembers_ReturnsAllMembers()
        {
            // Arrange
            var service = new FamilyTreeService();
            var p1 = new Person { FullName = "P1", Residence = new GeoCoordinates(9.9, -84.0) };
            var p2 = new Person { FullName = "P2", Residence = new GeoCoordinates(10.0, -85.0) };
            var p3 = new Person { FullName = "P3", Residence = new GeoCoordinates(10.5, -84.5) };

            service.AddPerson(p1);
            service.AddPerson(p2);
            service.AddPerson(p3);

            // Act
            var members = service.GetAllMembers();

            // Assert
            Assert.Equal(3, members.Count);
        }

        [Fact]
        public void GetMemberCount_ReturnsCorrectCount()
        {
            // Arrange
            var service = new FamilyTreeService();

            // Act & Assert - Vacío
            Assert.Equal(0, service.GetMemberCount());

            // Agregar personas
            service.AddPerson(new Person { FullName = "P1", Residence = new GeoCoordinates(9.9, -84.0) });
            Assert.Equal(1, service.GetMemberCount());

            service.AddPerson(new Person { FullName = "P2", Residence = new GeoCoordinates(10.0, -85.0) });
            Assert.Equal(2, service.GetMemberCount());
        }

        // ============================================
        // PRUEBAS DE ESTADÍSTICAS
        // ============================================

        [Fact]
        public void GetStatistics_WithNoMembers_ReturnsDefaultStats()
        {
            // Arrange
            var service = new FamilyTreeService();

            // Act
            var stats = service.GetStatistics();

            // Assert
            Assert.Null(stats.FarthestPair.Person1);
            Assert.Null(stats.FarthestPair.Person2);
            Assert.Null(stats.ClosestPair.Person1);
            Assert.Null(stats.ClosestPair.Person2);
        }

        [Fact]
        public void GetStatistics_WithOneMember_ReturnsDefaultStats()
        {
            // Arrange
            var service = new FamilyTreeService();
            service.AddPerson(new Person
            {
                FullName = "Solo",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            });

            // Act
            var stats = service.GetStatistics();

            // Assert
            Assert.Null(stats.FarthestPair.Person1);
        }

        [Fact]
        public void GetStatistics_WithMultipleMembers_CalculatesCorrectly()
        {
            // Arrange
            var service = new FamilyTreeService();
            
            var p1 = new Person
            {
                FullName = "San José",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            var p2 = new Person
            {
                FullName = "Cartago",
                Residence = new GeoCoordinates(9.8644, -83.9186)
            };
            var p3 = new Person
            {
                FullName = "Limón",
                Residence = new GeoCoordinates(9.9904, -83.0320)
            };

            service.AddPerson(p1);
            service.AddPerson(p2);
            service.AddPerson(p3);

            // Act
            var stats = service.GetStatistics();

            // Assert
            Assert.NotNull(stats.FarthestPair.Person1);
            Assert.NotNull(stats.FarthestPair.Person2);
            Assert.NotNull(stats.ClosestPair.Person1);
            Assert.NotNull(stats.ClosestPair.Person2);
            Assert.True(stats.MaxDistance > 0);
            Assert.True(stats.MinDistance > 0);
            Assert.True(stats.AverageDistance > 0);
            Assert.True(stats.MaxDistance >= stats.MinDistance);
        }

        // ============================================
        // PRUEBAS DE DISTANCIAS
        // ============================================

        [Fact]
        public void GetDistancesFrom_WithValidId_ReturnsDistances()
        {
            // Arrange
            var service = new FamilyTreeService();
            var p1 = new Person
            {
                FullName = "Centro",
                Residence = new GeoCoordinates(9.9281, -84.0907)
            };
            var p2 = new Person
            {
                FullName = "Vecino",
                Residence = new GeoCoordinates(10.0, -85.0)
            };

            service.AddPerson(p1);
            service.AddPerson(p2);

            // Act
            var distances = service.GetDistancesFrom(p1.Id);

            // Assert
            Assert.Single(distances);
            Assert.True(distances.ContainsKey(p2.Id));
            Assert.True(distances[p2.Id] > 0);
        }

        [Fact]
        public void GetDistancesFrom_WithInvalidId_ReturnsEmpty()
        {
            // Arrange
            var service = new FamilyTreeService();

            // Act
            var distances = service.GetDistancesFrom("invalid-id");

            // Assert
            Assert.NotNull(distances);
            Assert.Empty(distances);
        }
    }
}