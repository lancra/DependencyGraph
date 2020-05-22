using System;
using Moq;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Testing
{
    public class SequenceVerifier
    {
        private int _executionCount = 0;
        private int _setupCount = 0;

        public MockSequence Sequence { get; } = new MockSequence();

        public Action NextCallback()
        {
            var callNumber = _setupCount++;
            return () => AssertCallNumber(callNumber);
        }

        public void VerifyAll()
        {
            Assert.Equal(_setupCount, _executionCount);
        }

        private void AssertCallNumber(int expectedCallNumber)
        {
            Assert.Equal(_executionCount, expectedCallNumber);
            _executionCount++;
        }
    }
}
