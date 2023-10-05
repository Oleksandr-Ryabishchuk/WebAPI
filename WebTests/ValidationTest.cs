using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.DTOs;
using WebAPI.Entities;
using WebAPI.Services;

namespace WebTests
{
    [TestClass]
    public class NodeServiceTests
    {
        [TestMethod]
        public void CheckIfValid_NoChildren_ReturnsValidResponse()
        {
            // Arrange
            var tree = new Tree { Children = new List<Node>() };
            var dto = new NodeCreateDto { ParentId = 1, Name = "Node1" };
            var nodeService = new NodeService();

            // Act
            var result = nodeService.CheckIfValid(tree, dto);

            // Assert
            Assert.IsTrue(result.Valid);
            Assert.AreEqual("", result.Reason);
        }

        [TestMethod]
        public void CheckIfValid_ValidInput_ReturnsValidResponse()
        {
            // Arrange
            var tree = new Tree
            {
                Name = "TestTree",
                Children = new List<Node>
            {
                new Node { Id = 1, ParentId = null, Name = "RootNode" }
            }
            };
            var dto = new NodeCreateDto { ParentId = 1, Name = "Node1" };
            var nodeService = new NodeService();

            // Act
            var result = nodeService.CheckIfValid(tree, dto);

            // Assert
            Assert.IsTrue(result.Valid);
            Assert.AreEqual("", result.Reason);
        }

        [TestMethod]
        public void CheckIfValid_ParentHasChild_ReturnsInvalidResponse()
        {
            // Arrange
            var tree = new Tree
            {
                Name = "TestTree",
                Children = new List<Node>
            {
                new Node { Id = 1, ParentId = null, Name = "RootNode" },
                new Node { Id = 2, ParentId = 1, Name = "ChildNode" }
            }
            };
            var dto = new NodeCreateDto { ParentId = 1, Name = "Node1" };
            var nodeService = new NodeService();

            // Act
            var result = nodeService.CheckIfValid(tree, dto);

            // Assert
            Assert.IsFalse(result.Valid);
            Assert.AreEqual("Node with Id=1 already has a child", result.Reason);
        }

        [TestMethod]
        public void CheckIfValid_NodeWithNameAlreadyExists_ReturnsInvalidResponse()
        {
            // Arrange
            var tree = new Tree { Id = 1, Name = "Tree" };
            var existingNode = new Node { Id = 1, ParentId = null, TreeId = 1, Tree = tree, Name = "DuplicateNodeName" };
            tree.Children.Add(existingNode);

            var dto = new NodeCreateDto { Name = "DuplicateNodeName", ParentId = 1, TreeName = "Tree"}; // Node with the same name already exists.

            var nodeService = new NodeService();

            // Act
            var result = nodeService.CheckIfValid(tree, dto);

            // Assert
            Assert.IsFalse(result.Valid);
            Assert.AreEqual($"Node with Name={dto.Name} already exists in current tree", result.Reason);
        }
    }
}
