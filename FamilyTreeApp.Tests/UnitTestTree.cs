using Xunit;
using FamilyTreeApp.Core.DataStructures;
using FamilyTreeApp.Core.Models;
using System;
using System.Linq;

namespace FamilyTreeApp.Tests.DataStructures
{
    public class FamilyTreeTests
    {

        [Fact]
        public void AddPerson_WithValidPerson_ReturnsTrue()
        {
            var tree = new FamilyTree();
            var person = new Person
            {
                FullName = "Juan Pérez",
                IdNumber = "123456789",
                BirthDate = new DateTime(1990, 1, 1)
            };

            bool result = tree.AddPerson(person);

            Assert.True(result);
            Assert.Equal(1, tree.Count);
        }

        [Fact]
        public void AddPerson_WithDuplicateId_ReturnsFalse()
        {
            var tree = new FamilyTree();
            var person1 = new Person { FullName = "María López" };
            var person2 = new Person
            {
                Id = person1.Id, // Mismo ID
                FullName = "Ana García"
            };

            tree.AddPerson(person1);
            bool result = tree.AddPerson(person2);

            Assert.False(result);
            Assert.Equal(1, tree.Count);
        }

        [Fact]
        public void AddPerson_WithFather_AddsChildToFather()
        {
            var tree = new FamilyTree();
            var father = new Person { FullName = "Pedro Sánchez" };
            var child = new Person { FullName = "Carlos Sánchez" };

            tree.AddPerson(father);
            tree.AddPerson(child, fatherId: father.Id);

            var children = tree.GetChildren(father.Id);
            Assert.Single(children);
            Assert.Equal(child.Id, children[0].Id);
        }

        [Fact]
        public void AddPerson_WithMother_AddsChildToMother()
        {
            var tree = new FamilyTree();
            var mother = new Person { FullName = "Laura Ramírez" };
            var child = new Person { FullName = "Sofia Ramírez" };

            tree.AddPerson(mother);
            tree.AddPerson(child, motherId: mother.Id);

            var children = tree.GetChildren(mother.Id);
            Assert.Single(children);
            Assert.Equal(child.Id, children[0].Id);
        }

        [Fact]
        public void AddPerson_WithBothParents_AddsChildToBothParents()
        {
            var tree = new FamilyTree();
            var father = new Person { FullName = "Roberto Díaz" };
            var mother = new Person { FullName = "Carmen Vega" };
            var child = new Person { FullName = "Luis Díaz" };

            tree.AddPerson(father);
            tree.AddPerson(mother);
            tree.AddPerson(child, fatherId: father.Id, motherId: mother.Id);

            var fatherChildren = tree.GetChildren(father.Id);
            var motherChildren = tree.GetChildren(mother.Id);
            Assert.Single(fatherChildren);
            Assert.Single(motherChildren);
            Assert.Equal(child.Id, fatherChildren[0].Id);
            Assert.Equal(child.Id, motherChildren[0].Id);
        }

        [Fact]
        public void AddPerson_WithoutParents_AddsToRoots()
        {
            var tree = new FamilyTree();
            var person = new Person { FullName = "Miguel Torres" };

            tree.AddPerson(person);

            var roots = tree.GetRoots();
            Assert.Single(roots);
            Assert.Equal(person.Id, roots[0].Id);
        }

        [Fact]
        public void GetPerson_WithValidId_ReturnsPerson()
        {
            var tree = new FamilyTree();
            var person = new Person { FullName = "Elena Morales" };
            tree.AddPerson(person);

            var result = tree.GetPerson(person.Id);

            Assert.NotNull(result);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal("Elena Morales", result.FullName);
        }

        [Fact]
        public void GetPerson_WithInvalidId_ReturnsNull()
        {
            var tree = new FamilyTree();

            var result = tree.GetPerson("invalid-id-12345");

            Assert.Null(result);
        }

        [Fact]
        public void GetAllMembers_EmptyTree_ReturnsEmptyList()
        {
            var tree = new FamilyTree();

            var result = tree.GetAllMembers();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllMembers_WithMembers_ReturnsAllMembers()
        {
            var tree = new FamilyTree();
            var person1 = new Person { FullName = "Persona 1" };
            var person2 = new Person { FullName = "Persona 2" };
            var person3 = new Person { FullName = "Persona 3" };

            tree.AddPerson(person1);
            tree.AddPerson(person2);
            tree.AddPerson(person3);
            var result = tree.GetAllMembers();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetChildren_WithNoChildren_ReturnsEmptyList()
        {
            var tree = new FamilyTree();
            var person = new Person { FullName = "Sin Hijos" };
            tree.AddPerson(person);

            var result = tree.GetChildren(person.Id);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetChildren_WithInvalidId_ReturnsEmptyList()
        {
            var tree = new FamilyTree();

            var result = tree.GetChildren("invalid-id");

            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public void GetParents_WithBothParents_ReturnsBothParents()
        {
            var tree = new FamilyTree();
            var father = new Person { FullName = "Padre Test" };
            var mother = new Person { FullName = "Madre Test" };
            var child = new Person { FullName = "Hijo Test" };

            tree.AddPerson(father);
            tree.AddPerson(mother);
            tree.AddPerson(child, fatherId: father.Id, motherId: mother.Id);

            var parents = tree.GetParents(child.Id);

            Assert.Equal(2, parents.Count);
            Assert.Contains(parents, p => p.Id == father.Id);
            Assert.Contains(parents, p => p.Id == mother.Id);
        }

        [Fact]
        public void GetParents_WithNoParents_ReturnsEmptyList()
        {
            var tree = new FamilyTree();
            var person = new Person { FullName = "Huérfano Test" };
            tree.AddPerson(person);

            var parents = tree.GetParents(person.Id);

            Assert.NotNull(parents);
            Assert.Empty(parents);
        }

        [Fact]
        public void RemovePerson_WithValidId_ReturnsTrue()
        {
            var tree = new FamilyTree();
            var person = new Person { FullName = "Para Eliminar" };
            tree.AddPerson(person);

            bool result = tree.RemovePerson(person.Id);

            Assert.True(result);
            Assert.Equal(0, tree.Count);
        }

        [Fact]
        public void RemovePerson_WithInvalidId_ReturnsFalse()
        {
            var tree = new FamilyTree();

            bool result = tree.RemovePerson("invalid-id");

            Assert.False(result);
        }

        [Fact]
        public void RemovePerson_NullsChildrenParentIds()
        {
            var tree = new FamilyTree();
            var parent = new Person { FullName = "Padre a Eliminar" };
            var child = new Person { FullName = "Hijo Huérfano" };

            tree.AddPerson(parent);
            tree.AddPerson(child, fatherId: parent.Id);

            tree.RemovePerson(parent.Id);
            var retrievedChild = tree.GetPerson(child.Id);

            Assert.Null(retrievedChild.FatherId);
        }

        [Fact]
        public void Count_EmptyTree_ReturnsZero()
        {
            var tree = new FamilyTree();

            Assert.Equal(0, tree.Count);
        }

        [Fact]
        public void Count_AfterAddingMembers_ReturnsCorrectCount()
        {
            var tree = new FamilyTree();

            tree.AddPerson(new Person { FullName = "P1" });
            tree.AddPerson(new Person { FullName = "P2" });
            tree.AddPerson(new Person { FullName = "P3" });

            Assert.Equal(3, tree.Count);
        }

        [Fact]
        public void IsEmpty_EmptyTree_ReturnsTrue()
        {
            var tree = new FamilyTree();

            Assert.True(tree.IsEmpty);
        }

        [Fact]
        public void IsEmpty_WithMembers_ReturnsFalse()
        {
            var tree = new FamilyTree();
            tree.AddPerson(new Person { FullName = "Test" });

            Assert.False(tree.IsEmpty);
        }
    }
}