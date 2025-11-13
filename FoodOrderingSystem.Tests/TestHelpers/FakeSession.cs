using System;
using System.Collections.Generic;
using System.Web;

namespace FoodOrderingSystem.Tests.TestHelpers
{
    /// <summary>
    /// Đơn giản hóa HttpSessionStateBase cho unit test.
    /// Lưu dữ liệu vào Dictionary thay vì session thật.
    /// </summary>
    public class FakeSession : HttpSessionStateBase
    {
        private readonly Dictionary<string, object> _store =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        private bool _abandoned;

        public override object this[string name]
        {
            get => _store.TryGetValue(name, out var value) ? value : null;
            set => _store[name] = value;
        }

        public override int Count => _store.Count;

        public override void Add(string name, object value) => _store[name] = value;

        public override void Remove(string name) => _store.Remove(name);

        public override void RemoveAll() => _store.Clear();

        public override void Abandon() { _abandoned = true; }

        public bool IsAbandoned => _abandoned;
    }
}
