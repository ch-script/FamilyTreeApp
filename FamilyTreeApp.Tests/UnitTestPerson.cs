using Xunit;
using FamilyTreeApp.Core.Models;
using System;

namespace FamilyTreeApp.Tests.Models
{
    public class PersonTests
    {
        // ============================================
        // PRUEBAS DEL CONSTRUCTOR
        // ============================================

        [Fact]
        public void Constructor_InitializesWithDefaultValues()
        {
            // Arrange & Act
            var person = new Person();

            // Assert
            Assert.NotNull(person.Id);
            Assert.NotEqual(string.Empty, person.Id);
            Assert.NotNull(person.ChildrenIds);
            Assert.Empty(person.ChildrenIds);
            Assert.True(person.IsAlive);
            Assert.NotNull(person.Residence);
        }

        [Fact]
        public void Constructor_GeneratesUniqueId()
        {
            // Arrange & Act
            var person1 = new Person();
            var person2 = new Person();

            // Assert
            Assert.NotEqual(person1.Id, person2.Id);
        }

        // ============================================
        // PRUEBAS DE CÁLCULO DE EDAD
        // ============================================

        [Fact]
        public void CalculateAge_WithBirthDateInPast_CalculatesCorrectAge()
        {
            // Arrange
            var person = new Person
            {
                BirthDate = new DateTime(1990, 6, 15)
            };

            // Act
            person.CalculateAge();

            // Assert
            int expectedAge = DateTime.Today.Year - 1990;
            if (DateTime.Today < new DateTime(DateTime.Today.Year, 6, 15))
            {
                expectedAge--;
            }

            Assert.Equal(expectedAge, person.Age);
        }

        [Fact]
        public void CalculateAge_WithBirthdayNotYetThisYear_CalculatesCorrectly()
        {
            // Arrange
            var today = new DateTime(2025, 11, 30);
            var person = new Person
            {
                BirthDate = new DateTime(1995, 12, 31)
            };

            int expected_age = today.Year - person.BirthDate.Year;

            if (today < person.BirthDate.AddYears(expected_age))
                expected_age--;

            // Act
            person.CalculateAge();

            // Assert
            Assert.Equal(expected_age, person.Age);
        }
    }
}