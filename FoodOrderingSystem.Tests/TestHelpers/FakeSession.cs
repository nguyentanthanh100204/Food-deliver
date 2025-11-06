using System.Collections.Generic;
using System.Web;

namespace FoodOrderingSystem.Tests.TestHelpers
{
    public class FakeSession : HttpSessionStateBase
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public override object this[string name]
        {
            get => _data.ContainsKey(name) ? _data[name] : null;
            set => _data[name] = value;
        }
    }
}
