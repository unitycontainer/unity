using System;
using System.Collections.Generic;
using Unity.Tests.TestObjects;

namespace Tests.Unity.MSTests.TestObjects
{
    public class FakeDisposeCallback
    {
        public List<object> Disposed { get; } = new List<object>();
    }

    public interface IFakeDisposableCallbackService
    {
        int ID { get; }
    }


    public class FakeDisposableCallbackService : IFakeDisposableCallbackService, IDisposable
    {
        private static int _globalId;
        private readonly int _id;
        private readonly FakeDisposeCallback _callback;

        public FakeDisposableCallbackService(FakeDisposeCallback callback)
        {
            _id = _globalId++;
            _callback = callback;
        }

        public int ID => _id;

        public void Dispose()
        {
            _callback.Disposed.Add(this);
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }

    public interface IFakeMultipleService : IService
    {
    }

    public class FakeDisposableCallbackInnerService : FakeDisposableCallbackService, IFakeMultipleService
    {
        public FakeDisposableCallbackInnerService(FakeDisposeCallback callback) 
            : base(callback)
        {
        }
    }

    public class FakeDisposableCallbackOuterService : FakeDisposableCallbackService, IService
    {
        public FakeDisposableCallbackOuterService(FakeDisposeCallback callback) 
            : base(callback)
        {
        }
    }
}
