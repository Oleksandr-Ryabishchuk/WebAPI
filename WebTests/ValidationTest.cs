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
    }
}
