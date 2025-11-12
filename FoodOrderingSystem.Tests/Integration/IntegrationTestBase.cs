using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoodOrderingSystem.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        private TransactionScope _scope;

        [TestInitialize]
        public void BeginTransaction()
        {
            _scope = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);
        }

        [TestCleanup]
        public void RollbackTransaction()
        {
            _scope?.Dispose(); // không gọi Complete() => rollback
        }
    }
}
